using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class Anchor : ActionHandler
    {
        public override string Identifier => "Anchor";

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "a";

        public override void ExecuteAction(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var webElement = FindElement(webDriver, pageItem);
            try
            {
                webElement.Click();
            }
            catch (WebDriverException exception)
            {
                if (!Regex.IsMatch(exception.Message, @"unknown error: Element .+ is not clickable at point \(\d+, \d+\)\. Other element would receive the click: "))
                {
                    throw;
                }
                // Else the link is probably hidden by some sort of modal div or some such nonsense.  We can ignore it and pretend nothing happened and the action chain will terminate because it is looped.
            }
        }

        public override IEnumerable<PageItem> FindActions(RemoteWebDriver webDriver) =>
            FindElementsByXPath(webDriver, "//a").Where(webElement => webElement.Displayed && webElement.Enabled).Select(webElement =>
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
