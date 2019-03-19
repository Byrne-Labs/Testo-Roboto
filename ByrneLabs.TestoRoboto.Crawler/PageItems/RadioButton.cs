using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class RadioButton : ActionHandler
    {
        public override string Identifier => "RadioButton";

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "input" && pageItem.Type == "radio";

        public override void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var webElement = FindElement(webDriver, pageItem);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            FindElementsByXPath(webDriver, "//pageItem[@type='checkbox']").Where(webElement => webElement.Displayed && webElement.Enabled).Select(webElement =>
                PageItem.CreatePageItem(
                    webElement.GetAttribute("class"),
                    webElement.GetAttribute("id"),
                    webElement.GetAttribute("name"),
                    webElement.GetAttribute("onclick"),
                    webElement.GetAttribute("title"),
                    webElement.TagName,
                    webElement.GetAttribute("type"),
                    Identifier)
            ).Where(pageItem => pageItem != null).ToArray();
    }
}
