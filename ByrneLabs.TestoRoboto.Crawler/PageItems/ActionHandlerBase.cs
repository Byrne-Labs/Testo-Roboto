using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public abstract class ActionHandlerBase : ItemHandlerBase
    {
        public abstract void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem);

        public abstract IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver);
    }
}
