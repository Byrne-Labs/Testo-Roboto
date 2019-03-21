using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class AnchorHandler : ActionHandlerBase
    {
        private readonly IList<string> _returnedHrefs = new List<string>();

        public override string Identifier => "Anchor";

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "a";

        public override void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var webElement = FindElement(webDriver, pageItem);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver)
        {
            var anchors = FindElementsByXPath(webDriver, "//a");
            var newAnchors = new List<IWebElement>();
            lock (_returnedHrefs)
            {
                foreach (var anchor in anchors)
                {
                    var href = anchor.GetProperty("href");
                    if (!_returnedHrefs.Contains(href) && string.IsNullOrWhiteSpace(anchor.GetAttribute("onclick")))
                    {
                        newAnchors.Add(anchor);
                        _returnedHrefs.Add(href);
                    }
                }
            }

            return newAnchors.Select(webElement =>
                PageItem.CreatePageItem(
                    webElement.GetProperty("class"),
                    webElement.GetProperty("href"),
                    webElement.GetProperty("id"),
                    webElement.GetProperty("name"),
                    webElement.GetAttribute("onclick"),
                    webElement.GetProperty("title"),
                    webElement.TagName,
                    webElement.GetProperty("type"),
                    webElement.GetProperty("value"),
                    Identifier)
            ).Where(pageItem => pageItem != null).ToArray();
        }
    }
}
