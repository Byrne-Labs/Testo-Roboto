using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium.Chrome;

namespace ByrneLabs.TestoRoboto.Crawler
{
    public class CrawlManager
    {
        private readonly ConcurrentQueue<ActionChain> _actionChainsToCrawl = new ConcurrentQueue<ActionChain>();
        private readonly IList<ActionChainItem> _crawledActionChainItems = new List<ActionChainItem>();

        private CrawlManager()
        {
        }

        public static void Crawl(string url, int maxChainLength, int maxThreads)
        {
            var crawlManager = new CrawlManager();
            crawlManager.Start(url, maxChainLength, maxThreads);
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
                if (!_crawledActionChainItems.Contains(actionChainItem))
                {
                    _crawledActionChainItems.Add(actionChainItem);
                    actionChainItem.Crawled = true;
                    return true;
                }

                return false;
            }
        }

        private CrawlSetup GetCrawlSetup(int maxChainLength)
        {
            var crawlSetup = new CrawlSetup();
            crawlSetup.ActionHandlers.Add(new Anchor());
            crawlSetup.ActionHandlers.Add(new Button());

            crawlSetup.DataInputHandlers.Add(new TextInput());
            crawlSetup.DataInputHandlers.Add(new DateInput());

            crawlSetup.CrawlManager = this;

            crawlSetup.WebDriver = new ChromeDriver();

            crawlSetup.MaxChainLength = maxChainLength;

            return crawlSetup;
        }

        private void Start(string url, int maxChainLength, int maxThreads)
        {
            var initialCrawlSetup = GetCrawlSetup(maxChainLength);
            using (var initialCrawler = new Crawler(initialCrawlSetup))
            {
                initialCrawler.Crawl(url);
            }

            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = maxThreads;

            BetterParallel.While(parallelOptions, () => !_actionChainsToCrawl.IsEmpty, () =>
            {
                if (_actionChainsToCrawl.TryDequeue(out var actionChainToCrawl))
                {
                    var crawlSetup = GetCrawlSetup(maxChainLength);
                    using (var crawler = new Crawler(crawlSetup))
                    {
                        crawler.Crawl(actionChainToCrawl);
                    }
                }
            });
        }
    }
}
