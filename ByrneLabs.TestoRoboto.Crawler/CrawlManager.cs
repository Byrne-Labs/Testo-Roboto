using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium.Chrome;

namespace ByrneLabs.TestoRoboto.Crawler
{
    public class CrawlManager
    {
        private static readonly object _logFileLock = new object();
        private readonly ConcurrentQueue<ActionChain> _actionChainsToCrawl = new ConcurrentQueue<ActionChain>();
        private readonly IList<ActionChainItem> _crawledActionChainItems = new List<ActionChainItem>();
        private readonly CrawlOptions _crawlOptions;

        public CrawlManager(CrawlOptions crawlOptions)
        {
            _crawlOptions = crawlOptions;
        }

        public void Start()
        {
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
                        try
                        {
                            crawler.Crawl(actionChainToCrawl);
                        }
                        catch (Exception exception)
                        {
                            //todo: log exception
                        }
                    }

                    lock (_logFileLock)
                    {
                        File.AppendAllText("completed action chains.txt", $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}\t{actionChainToCrawl}\r\n");
                    }
                }
            });
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

        internal bool ShouldBeCrawled(ActionChainItem actionChainItem)
        {
            lock (_crawledActionChainItems)
            {
                if (!_crawledActionChainItems.Contains(actionChainItem) && _crawlOptions.AllowedUrlPatterns.Any(allowedUrlPattern => Regex.IsMatch(actionChainItem.Url, allowedUrlPattern)))
                {
                    _crawledActionChainItems.Add(actionChainItem);
                    actionChainItem.Crawled = true;
                    return true;
                }

                return false;
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

            crawlerSetup.WebDriver = new ChromeDriver(driverService, chromeOptions);

            foreach (var cookie in crawlOptions.SessionCookies)
            {
                crawlerSetup.SessionCookies.Add(cookie);
            }

            crawlerSetup.MaximumChainLength = crawlOptions.MaximumChainLength;
            return crawlerSetup;
        }
    }
}
