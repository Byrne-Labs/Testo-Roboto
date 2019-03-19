using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public abstract class ItemHandler
    {
        public abstract string Identifier { get; }

        protected static IWebElement FindElement(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var attributeFilterList = new List<string>();

            if (!string.IsNullOrWhiteSpace(pageItem.Class))
            {
                attributeFilterList.Add($"@class='{HttpUtility.HtmlEncode(pageItem.Class)}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Id))
            {
                attributeFilterList.Add($"@id='{HttpUtility.HtmlEncode(pageItem.Id)}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Name))
            {
                attributeFilterList.Add($"@name='{HttpUtility.HtmlEncode(pageItem.Name)}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Title))
            {
                attributeFilterList.Add($"@title='{HttpUtility.HtmlEncode(pageItem.Title)}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Type))
            {
                attributeFilterList.Add($"@type='{HttpUtility.HtmlEncode(pageItem.Type)}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.OnClick))
            {
                attributeFilterList.Add($"@onclick='{HttpUtility.HtmlEncode(pageItem.OnClick)}'");
            }

            var attributeFilter = string.Join(" and ", attributeFilterList);

            var elements = webDriver.FindElementsByXPath($"//{pageItem.Tag}[{attributeFilter}]");
            if (!elements.Any())
            {
                throw new ArgumentException($"Could not find element {pageItem} on page");
            }

            if (elements.Count > 1)
            {
                throw new ArgumentException($"More than one element {pageItem} found on page");
            }

            return elements.Single();
        }

        public abstract bool CanHandle(PageItem input);
    }
}
