using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class ButtonHandler : ActionHandlerBase
    {
        public override string Identifier => "InputButton";

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "input" && new[] { "button", "search", "submit" }.Contains(pageItem.Type);

        public override void ExecuteAction(RemoteWebDriver remoteWebDriver, PageItem input)
        {
            var webElement = FindElement(remoteWebDriver, input);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            FindElementsByXPath(webDriver, "//input[@type='button' or @type='submit' or @type='search']").Select(webElement =>
                PageItem.CreatePageItem(
                    false,
                    webElement.GetProperty("class"),
                    null,
                    webElement.GetProperty("id"),
                    webElement.GetProperty("name"),
                    webElement.GetAttribute("onclick"),
                    webElement.GetProperty("title"),
                    webElement.TagName,
                    webElement.GetProperty("type"),
                    null,
                    Identifier)
            ).Where(pageItem => pageItem != null).ToArray();
    }
}
