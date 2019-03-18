using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public abstract class ActionHandler : ItemHandler
    {
        public abstract void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem);

        public abstract IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver);
    }
}
