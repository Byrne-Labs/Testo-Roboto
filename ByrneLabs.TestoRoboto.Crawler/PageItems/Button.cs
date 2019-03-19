﻿using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class Button : ActionHandler
    {
        public override string Identifier => "InputButton";

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "input" && new[] { "button", "search", "submit" }.Contains(pageItem.Type);

        public override void ExecuteAction(RemoteWebDriver remoteWebDriver, PageItem input)
        {
            var webElement = FindElement(remoteWebDriver, input);
            webElement.Click();
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            FindElementsByXPath(webDriver, "//pageItem[@type='button' or @type='submit' or @type='search']").Where(webElement => webElement.Displayed && webElement.Enabled).Select(webElement =>
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
