using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using HtmlAgilityPack;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class SmartWebElement : IWebElement //, IEquatable<FastWebElement>
    {
        private readonly IWebElement _webElement;

        public SmartWebElement(IWebElement webElement)
        {
            _webElement = webElement;
            PropertyValues = new Dictionary<string, string>();
            var outerHtml = GetAttribute("outerHTML");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(outerHtml);
            foreach (var attribute in htmlDocument.DocumentNode.FirstChild.Attributes)
            {
                PropertyValues.Add(attribute.Name, attribute.Value);
            }
        }

        public bool Displayed => _webElement.Displayed;

        public bool Enabled => _webElement.Enabled;

        public Point Location => _webElement.Location;

        public IDictionary<string, string> PropertyValues { get; }

        public bool Selected => _webElement.Selected;

        public Size Size => _webElement.Size;

        public string TagName => _webElement.TagName;

        public string Text => _webElement.Text;

        private static T TryAction<T>(Func<T> func)
        {
            var staleCount = 0;
            while (true)
            {
                try
                {
                    return func();
                }
                catch (StaleElementReferenceException)
                {
                    if (staleCount >= 100)
                    {
                        throw;
                    }

                    staleCount++;
                    Thread.Sleep(100);
                }
            }
        }

        private static void TryAction(Action func)
        {
            var staleCount = 0;
            var success = false;
            while (!success)
            {
                try
                {
                    func();
                    success = true;
                }
                catch (StaleElementReferenceException)
                {
                    if (staleCount >= 100)
                    {
                        throw;
                    }

                    staleCount++;
                    Thread.Sleep(100);
                }
            }
        }

        public void Clear()
        {
            _webElement.Clear();
        }

        public void Click()
        {
            _webElement.Click();
        }

        public IWebElement FindElement(By by) => TryAction(() => _webElement.FindElement(by));

        public ReadOnlyCollection<IWebElement> FindElements(By by) => TryAction(() => _webElement.FindElements(by));

        public string GetAttribute(string attributeName) => TryAction(() => _webElement.GetAttribute(attributeName));

        public string GetCssValue(string propertyName) => TryAction(() => _webElement.GetCssValue(propertyName));

        public string GetProperty(string propertyName)
        {
            if (propertyName == "href")
            {
                return GetAttribute(propertyName);
            }

            return PropertyValues.ContainsKey(propertyName) ? PropertyValues[propertyName] : null;
        }

        public void SendKeys(string text)
        {
            TryAction(() => _webElement.SendKeys(text));
        }

        public void Submit()
        {
            TryAction(() => _webElement.Submit());
        }
    }
}
