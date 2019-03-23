using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using ByrneLabs.TestoRoboto.HttpServices;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Cookie = ByrneLabs.TestoRoboto.HttpServices.Cookie;

namespace ByrneLabs.TestoRoboto.Crawler
{
    [PublicAPI]
    public class CrawlManager
    {
        private class FingerprintRequestMessageComparer : IEqualityComparer<RequestMessage>
        {
            public bool Equals(RequestMessage x, RequestMessage y) => x?.Fingerprint == y?.Fingerprint;

            public int GetHashCode(RequestMessage obj) => obj.Fingerprint.GetHashCode();
        }

        private static readonly object _logFileLock = new object();
        private static int _nextProxyPort = 49152;
        private readonly ConcurrentQueue<ActionChain> _actionChainsToCrawl = new ConcurrentQueue<ActionChain>();
        private readonly IList<ActionChainItem> _crawledActionChainItems = new List<ActionChainItem>();
        private readonly CrawlOptions _crawlOptions;
        private readonly ConcurrentBag<RequestMessage> _requestMessages = new ConcurrentBag<RequestMessage>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _initialPagesCrawled;
        private int _proxyPort;

        public CrawlManager(CrawlOptions crawlOptions)
        {
            /*
             * We clone it to make sure it doesn't get changed during the middle of the crawl being executed.
             */
            _crawlOptions = crawlOptions.Clone();
        }

        public IEnumerable<RequestMessage> DiscoveredRequestMessages => _requestMessages.Distinct(new FingerprintRequestMessageComparer()).ToList();

        private Task _crawlTask;

        public void Start()
        {
            _crawlTask = new Task(() =>
            {
                SetupProxyServer();

                var parallelOptions = new ParallelOptions();
                parallelOptions.CancellationToken = _cancellationTokenSource.Token;
                parallelOptions.MaxDegreeOfParallelism = _crawlOptions.MaximumThreads;

                if (!_initialPagesCrawled)
                {
                    Parallel.ForEach(_crawlOptions.StartingUrls, parallelOptions, url =>
                    {
                        using (var initialCrawler = new Crawler(GetCrawlerSetup(_crawlOptions)))
                        {
                            initialCrawler.Crawl(url);
                        }

                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    });
                }

                _initialPagesCrawled = true;
                BetterParallel.While(parallelOptions, () => !_actionChainsToCrawl.IsEmpty, () =>
                {
                    if (_actionChainsToCrawl.TryDequeue(out var actionChainToCrawl))
                    {
                        try
                        {
                            if (ShouldBeCrawled(actionChainToCrawl))
                            {
                                using (var crawler = new Crawler(GetCrawlerSetup(_crawlOptions)))
                                {
                                    crawler.Crawl(actionChainToCrawl);
                                }
                            }
                        }
                        catch (WebDriverException exception)
                        {
                            actionChainToCrawl.Exception = exception;
                            if (Regex.IsMatch(exception.Message, "stale element reference: element is not attached to the page document"))
                            {
                                actionChainToCrawl.TerminationReason = "Stale Element Reference";
                            }
                            else if (Regex.IsMatch(exception.Message, "The HTTP request to the remote WebDriver server for URL"))
                            {
                                actionChainToCrawl.TerminationReason = "WebDriver Server Not Responding";
                            }
                            else
                            {
                                actionChainToCrawl.TerminationReason = "Selenium Exception";
                            }

                            ReportCompletedActionChain(actionChainToCrawl);
                        }
                        catch (Exception exception)
                        {
                            actionChainToCrawl.Exception = exception;
                            actionChainToCrawl.TerminationReason = "Exception";
                            ReportCompletedActionChain(actionChainToCrawl);
                        }
                    }

                    parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                });
            }, _cancellationTokenSource.Token);
            _crawlTask.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
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
                if (lastActionChainItem == null)
                {
                    Debugger.Break();
                }

                if (_crawledActionChainItems.Contains(lastActionChainItem))
                {
                    actionChain.TerminationReason = "Already Crawled";
                    return false;
                }

                if (!_crawlOptions.AllowedUrlPatterns.Any(allowedUrlPattern => Regex.IsMatch(lastActionChainItem.Url, allowedUrlPattern, RegexOptions.IgnoreCase)))
                {
                    actionChain.TerminationReason = "Banned URL Pattern";
                    return false;
                }

                if (lastActionChainItem.Crawled)
                {
                    actionChain.TerminationReason = "No More Actions";
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
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Client, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Profiler, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Server, LogLevel.Severe);
            chromeOptions.AddArgument("log-level=3");
            chromeOptions.AddArgument("silent");
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
            await Task.Run(() =>
            {
                var requestMessage = new RequestMessage();
                requestMessage.Uri = new Uri(e.HttpClient.Request.Url);
                foreach (var tiHeader in e.HttpClient.Request.Headers.Where(th => th.Name != "Cookie"))
                {
                    var header = new Header();
                    header.Key = tiHeader.Name;
                    header.Value = tiHeader.Value;
                    requestMessage.Headers.Add(header);
                }

                var cookies = e.HttpClient.Request.Headers.HeaderExists("Cookie") ? e.HttpClient.Request.Headers.GetFirstHeader("Cookie").Value : string.Empty;
                foreach (var cookie in cookies.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var name = cookie.SubstringBeforeLast("=");
                    var value = cookie.SubstringAfterLast("=");

                    requestMessage.Cookies.Add(new Cookie { Name = name, Value = value });
                }

                requestMessage.Encoding = e.HttpClient.Request.Encoding;

                var contentType = e.HttpClient.Request.Headers.HeaderExists("Content-Type") ? e.HttpClient.Request.Headers.GetFirstHeader("Content-Type").Value : string.Empty;

                if (!e.HttpClient.Request.HasBody)
                {
                    requestMessage.Body = new NoBody();
                }
                else
                {
                    var bodyText = e.GetRequestBodyAsString().Result;
                    if (contentType == "application/x-www-form-urlencoded")
                    {
                        requestMessage.Body = FormUrlEncodedBody.GetFromBodyText(bodyText);
                    }
                    else if (contentType == "multipart/form-data")
                    {
                        requestMessage.Body = FormDataBody.GetFromBodyText(bodyText);
                    }
                    else
                    {
                        requestMessage.Body = RawBody.GetFromBodyText(bodyText);
                    }
                }

                requestMessage.HttpMethod = new HttpMethod(e.HttpClient.Request.Method);

                _requestMessages.Add(requestMessage);
            });
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
