using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public class Button : ActionHandler
    {
        public override string Identifier => "InputButton";

        public override bool CanHandle(PageItem input) => input.Tag == "input" && new[] { "button", "search", "submit" }.Contains(input.Type);

        public override void ExecuteAction(RemoteWebDriver remoteWebDriver, PageItem input)
        {
            var webElement = FindElement(remoteWebDriver, input);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            webDriver.FindElementsByXPath("//input[@type='button' or @type='submit' or @type='search']").Select(webElement =>
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
