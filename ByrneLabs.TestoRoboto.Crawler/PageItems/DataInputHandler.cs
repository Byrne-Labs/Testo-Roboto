using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public abstract class DataInputHandler : ItemHandler
    {
        public abstract void FillInput(RemoteWebDriver webDriver, PageItem input);

        public abstract IEnumerable<PageItem> FindDataInputs(RemoteWebDriver webDriver);
    }
}
