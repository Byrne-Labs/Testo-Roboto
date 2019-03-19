using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class UrlInput : InputElementHandler
    {
        public override string Identifier => "UrlInput";

        public override IEnumerable<string> InputTypes => new[] { "url" };

        protected override string GetSampleText(IWebElement webElement) => "http://somemadeupwebsitethatdoesnotexist.com";
    }
}
