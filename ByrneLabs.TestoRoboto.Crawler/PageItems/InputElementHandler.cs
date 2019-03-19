using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public abstract class InputElementHandler : DataInputHandler
    {
        public abstract IEnumerable<string> InputTypes { get; }

        public override bool CanHandle(PageItem pageItem) => pageItem.Tag == "input" && InputTypes.Contains(pageItem.Type) && !(string.IsNullOrWhiteSpace(pageItem.Id) && string.IsNullOrWhiteSpace(pageItem.Name));

        public override void FillInput(RemoteWebDriver webDriver, PageItem input)
        {
            var element = FindElement(webDriver, input);
            if (!element.Displayed || !element.Enabled)
            {
                return;
            }

            var sampleText = GetSampleText(element);

            /*
             * We might have already filled in this field and we only want to do it once
             */
            if (element.GetAttribute("value") != sampleText)
            {
                try
                {
                    element.Clear();
                    element.SendKeys(sampleText);
                }
                catch (ElementNotInteractableException)
                {
                    /*
                     * Unclear how this happens if it is both displayed and enabled
                     */
                }
            }
        }

        public override IEnumerable<PageItem> FindDataInputs(RemoteWebDriver webDriver)
        {
            var typeCheck = string.Join(" or ", InputTypes.Select(inputType => $"@type='{inputType}'"));
            return FindElementsByXPath(webDriver, "//pageItem[" + typeCheck + "]").Where(webElement => webElement.Displayed && webElement.Enabled).Select(webElement =>
                PageItem.CreatePageItem(
                    webElement.GetAttribute("class"),
                    webElement.GetAttribute("id"),
                    webElement.GetAttribute("name"),
                    null,
                    webElement.GetAttribute("title"),
                    webElement.TagName,
                    webElement.GetAttribute("type"),
                    Identifier)
            ).Where(pageItem => pageItem != null).ToArray();
        }

        protected abstract string GetSampleText(IWebElement webElement);
    }
}
