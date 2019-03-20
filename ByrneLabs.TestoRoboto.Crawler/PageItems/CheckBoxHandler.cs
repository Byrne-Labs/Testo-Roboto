using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class CheckBoxHandler : ActionHandlerBase
    {
        public override string Identifier => "CheckBox";

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "input" && pageItem.Type == "checkbox";

        public override void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var webElement = FindElement(webDriver, pageItem);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            FindElementsByXPath(webDriver, "//input[@type='checkbox']").Select(webElement =>
                PageItem.CreatePageItem(
                    webElement.GetProperty("class"),
                    null,
                    webElement.GetProperty("id"),
                    webElement.GetProperty("name"),
                    webElement.GetProperty("onclick"),
                    webElement.GetProperty("title"),
                    webElement.TagName,
                    webElement.GetProperty("type"),
                    webElement.GetProperty("value"),
                    Identifier)
            ).Where(pageItem => pageItem != null).ToArray();
    }
}
