using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public abstract class InputElementHandlerBase : DataInputHandler
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
            if (element.GetProperty("value") != sampleText)
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
            return FindElementsByXPath(webDriver, "//input[" + typeCheck + "]").Select(webElement =>
                PageItem.CreatePageItem(
                    webElement.GetProperty("class"),
                    null,
                    webElement.GetProperty("id"),
                    webElement.GetProperty("name"),
                    null,
                    webElement.GetProperty("title"),
                    webElement.TagName,
                    webElement.GetProperty("type"),
                    webElement.GetProperty("value"),
                    Identifier)
            ).Where(pageItem => pageItem != null).ToArray();
        }

        protected abstract string GetSampleText(IWebElement webElement);
    }
}
