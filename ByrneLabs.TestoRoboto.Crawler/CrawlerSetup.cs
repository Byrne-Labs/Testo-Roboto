using System.Collections.Generic;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class CrawlerSetup
    {
        public IList<ActionHandler> ActionHandlers { get; } = new List<ActionHandler>();

        public CrawlManager CrawlManager { get; set; }

        public IList<DataInputHandler> DataInputHandlers { get; } = new List<DataInputHandler>();

        public int MaximumChainLength { get; set; } = 12;

        public IList<Cookie> SessionCookies { get; } = new List<Cookie>();

        public RemoteWebDriver WebDriver { get; set; }
    }
}
