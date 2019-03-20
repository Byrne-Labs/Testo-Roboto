using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace ByrneLabs.TestoRoboto.Crawler
{
    public class CrawlManager
    {
        private static readonly object _logFileLock = new object();
        private static int _nextProxyPort = 49152;
        private readonly ConcurrentQueue<ActionChain> _actionChainsToCrawl = new ConcurrentQueue<ActionChain>();
        private readonly IList<ActionChainItem> _crawledActionChainItems = new List<ActionChainItem>();
        private readonly CrawlOptions _crawlOptions;
        private int _proxyPort;

        public CrawlManager(CrawlOptions crawlOptions)
        {
            _crawlOptions = crawlOptions;
        }

        public void Start()
        {
            SetupProxyServer();

            Parallel.ForEach(_crawlOptions.StartingUrls, url =>
            {
                using (var initialCrawler = new Crawler(GetCrawlerSetup(_crawlOptions)))
                {
                    initialCrawler.Crawl(url);
                }
            });

            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = _crawlOptions.MaximumThreads;

            BetterParallel.While(parallelOptions, () => !_actionChainsToCrawl.IsEmpty, () =>
            {
                if (_actionChainsToCrawl.TryDequeue(out var actionChainToCrawl))
                {
                    using (var crawler = new Crawler(GetCrawlerSetup(_crawlOptions)))
                    {
                        crawler.Crawl(actionChainToCrawl);
                    }
                }
            });
        }

        internal void ReportCompletedActionChain(ActionChain actionChain)
        {
            lock (_logFileLock)
            {
                File.AppendAllText("completed action chains.txt", $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}\t{actionChain.TerminationReason}\t{actionChain.Items.Count}\t{actionChain}\r\n");
            }
        }

        internal void ReportDiscoveredActionChains(IEnumerable<ActionChain> discoveredActionChains)
        {
            lock (_actionChainsToCrawl)
            {
                foreach (var discoveredActionChain in discoveredActionChains)
                {
                    if (!discoveredActionChain.Items.Last().Crawled && !discoveredActionChain.IsLooped)
                    {
                        _actionChainsToCrawl.Enqueue(discoveredActionChain);
                    }
                }
            }
        }

        internal bool ShouldBeCrawled(ActionChain actionChain)
        {
            lock (_crawledActionChainItems)
            {
                if (actionChain.IsLooped)
                {
                    actionChain.TerminationReason = "Looped";
                    return false;
                }

                var lastActionChainItem = actionChain.Items.Last();
                if (_crawledActionChainItems.Contains(lastActionChainItem))
                {
                    actionChain.TerminationReason = "Already Crawled";
                    return false;
                }

                if (!_crawlOptions.AllowedUrlPatterns.Any(allowedUrlPattern => Regex.IsMatch(lastActionChainItem.Url, allowedUrlPattern)))
                {
                    actionChain.TerminationReason = "Banned URL Pattern";
                    return false;
                }

                _crawledActionChainItems.Add(lastActionChainItem);
                lastActionChainItem.Crawled = true;
                lock (_logFileLock)
                {
                    File.AppendAllText("crawled actions.txt", $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}\t{lastActionChainItem}\r\n");
                }
                return true;
            }
        }

        private CrawlerSetup GetCrawlerSetup(CrawlOptions crawlOptions)
        {
            var crawlerSetup = new CrawlerSetup();
            crawlerSetup.ActionHandlers.Add(new AnchorHandler());
            crawlerSetup.ActionHandlers.Add(new ButtonHandler());

            crawlerSetup.DataInputHandlers.Add(new DateInputHandler());
            crawlerSetup.DataInputHandlers.Add(new DateTimeInputHandler());
            crawlerSetup.DataInputHandlers.Add(new EmailInputHandler());
            crawlerSetup.DataInputHandlers.Add(new MonthInputHandler());
            crawlerSetup.DataInputHandlers.Add(new NumberInputHandler());
            crawlerSetup.DataInputHandlers.Add(new TelephoneInputHandler());
            crawlerSetup.DataInputHandlers.Add(new TextInputHandler());
            crawlerSetup.DataInputHandlers.Add(new TimeInputHandler());
            crawlerSetup.DataInputHandlers.Add(new UrlInputHandler());
            crawlerSetup.DataInputHandlers.Add(new WeekInputHandler());

            crawlerSetup.CrawlManager = this;

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.SuppressInitialDiagnosticInformation = true;

            var chromeOptions = new ChromeOptions();

            if (crawlOptions.DisableImageDownloading)
            {
                chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            }

            if (crawlOptions.HeadlessBrowsing)
            {
                chromeOptions.AddArgument("headless");
            }

            chromeOptions.AcceptInsecureCertificates = true;
            chromeOptions.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Client, LogLevel.Warning);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Warning);
            chromeOptions.SetLoggingPreference(LogType.Profiler, LogLevel.Warning);
            chromeOptions.SetLoggingPreference(LogType.Server, LogLevel.Warning);
            chromeOptions.Proxy = new Proxy { HttpProxy = "localhost:" + _proxyPort, SslProxy = "localhost:" + _proxyPort };
            crawlerSetup.WebDriver = new ChromeDriver(driverService, chromeOptions);

            foreach (var cookie in crawlOptions.SessionCookies)
            {
                crawlerSetup.SessionCookies.Add(cookie);
            }

            crawlerSetup.MaximumChainLength = crawlOptions.MaximumChainLength;
            return crawlerSetup;
        }

        private async Task ProxyServer_BeforeRequest(object sender, SessionEventArgs e)
        {
            await Task.Run(() => { });
        }

        private async Task ProxyServer_BeforeResponse(object sender, SessionEventArgs e)
        {
            await Task.Run(() => { });
        }

        private void SetupProxyServer()
        {
            var proxyServer = new ProxyServer();
            proxyServer.CertificateManager.TrustRootCertificate(true);
            proxyServer.BeforeRequest += ProxyServer_BeforeRequest;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;
            _proxyPort = _nextProxyPort++;
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, _proxyPort);
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();
        }
    }
}
