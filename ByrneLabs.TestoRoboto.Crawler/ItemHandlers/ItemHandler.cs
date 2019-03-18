using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public abstract class ItemHandler
    {
        public abstract string Identifier { get; }

        public abstract bool CanHandle(PageItem input);

        public IWebElement FindElement(RemoteWebDriver webDriver, PageItem pageItem) => throw new NotImplementedException();
    }
}
