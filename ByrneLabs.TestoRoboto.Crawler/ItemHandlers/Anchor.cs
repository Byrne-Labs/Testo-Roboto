using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public class Anchor : ActionHandler
    {
        public override string Identifier => "Anchor";

        public override bool CanHandle(PageItem input) => input.Tag == "a";

        public override void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var webElement = FindElement(webDriver, pageItem);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            webDriver.FindElementsByXPath("//a").Select(webElement =>
                new PageItem(
                    webElement.GetAttribute("id"),
                    webElement.GetAttribute("name"),
                    webElement.GetAttribute("onclick"),
                    webElement.GetAttribute("src"),
                    webElement.TagName,
                    null,
                    webElement.GetAttribute("type"),
                    Identifier)
            ).ToArray();
    }
}
