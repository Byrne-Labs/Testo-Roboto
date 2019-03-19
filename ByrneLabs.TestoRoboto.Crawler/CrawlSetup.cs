using System.Collections.Generic;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class CrawlSetup
    {
        public IList<ActionHandler> ActionHandlers { get; } = new List<ActionHandler>();

        public CrawlManager CrawlManager { get; set; }

        public IList<DataInputHandler> DataInputHandlers { get; } = new List<DataInputHandler>();

        public int MaxChainLength { get; set; } = 12;

        public RemoteWebDriver WebDriver { get; set; }
    }
}
