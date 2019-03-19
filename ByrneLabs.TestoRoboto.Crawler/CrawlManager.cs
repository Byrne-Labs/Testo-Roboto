﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ByrneLabs.TestoRoboto.Crawler
{
    public class CrawlManager
    {
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
                        crawler.Crawl(actionChainToCrawl);
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
                if (!_crawledActionChainItems.Contains(actionChainItem) && _crawlOptions.AllowedUrlPatterns.Any(allowedUrlPattern=> Regex.IsMatch(actionChainItem.Url, allowedUrlPattern)))
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
            crawlerSetup.ActionHandlers.Add(new Anchor());
            crawlerSetup.ActionHandlers.Add(new Button());

            crawlerSetup.DataInputHandlers.Add(new DateInput());
            crawlerSetup.DataInputHandlers.Add(new DateTimeInput());
            crawlerSetup.DataInputHandlers.Add(new EmailInput());
            crawlerSetup.DataInputHandlers.Add(new MonthInput());
            crawlerSetup.DataInputHandlers.Add(new NumberInput());
            crawlerSetup.DataInputHandlers.Add(new TelephoneInput());
            crawlerSetup.DataInputHandlers.Add(new TextInput());
            crawlerSetup.DataInputHandlers.Add(new TimeInput());
            crawlerSetup.DataInputHandlers.Add(new UrlInput());
            crawlerSetup.DataInputHandlers.Add(new WeekInput());

            crawlerSetup.CrawlManager = this;

            crawlerSetup.WebDriver = new ChromeDriver();

            foreach (var cookie in crawlOptions.SessionCookies)
            {
                crawlerSetup.SessionCookies.Add(cookie);
            }

            crawlerSetup.MaximumChainLength = crawlOptions.MaximumChainLength;
            return crawlerSetup;
        }
    }
}
