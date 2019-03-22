using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public abstract class ItemHandlerBase
    {
        public abstract string Identifier { get; }

        protected static IWebElement FindElement(RemoteWebDriver webDriver, PageItem pageItem)
        {
            var attributeFilterList = new List<string>();

            if (!string.IsNullOrWhiteSpace(pageItem.Class))
            {
                attributeFilterList.Add($"@class='{pageItem.Class}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Id))
            {
                attributeFilterList.Add($"@id='{pageItem.Id}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Name))
            {
                attributeFilterList.Add($"@name='{pageItem.Name}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Title) && !pageItem.Title.Contains("'"))
            {
                attributeFilterList.Add($"@title='{pageItem.Title}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.Type))
            {
                attributeFilterList.Add($"@type='{pageItem.Type}'");
            }

            if (!string.IsNullOrWhiteSpace(pageItem.OnClick) && !pageItem.OnClick.Contains("'"))
            {
                attributeFilterList.Add($"@onclick='{pageItem.OnClick}'");
            }

            var attributeFilter = string.Join(" and ", attributeFilterList);

            var elements = FindElementsByXPath(webDriver, $"//{pageItem.Tag}[{attributeFilter}]");
            if (!elements.Any())
            {
                throw new ItemNotFoundException(pageItem);
            }

            if (elements.Count > 1)
            {
                //todo: it might be better to throw an exception here but this is easier
                return elements.First();
            }

            return elements.Single();
        }

        protected static IReadOnlyCollection<IWebElement> FindElementsByXPath(RemoteWebDriver webDriver, string xPath)
        {
            var elements = webDriver.FindElementsByXPath(xPath);
            var smartElements = elements.Select(element => new SmartWebElement(element)).ToList().AsReadOnly();

            return smartElements;
        }

        public abstract bool CanHandle(PageItem pageItem);
    }
}
