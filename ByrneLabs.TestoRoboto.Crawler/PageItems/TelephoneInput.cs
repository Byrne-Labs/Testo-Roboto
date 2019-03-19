using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class TelephoneInput : InputElementHandler
    {
        public override string Identifier => "TelephoneInput";

        public override IEnumerable<string> InputTypes => new[] { "tel" };

        protected override string GetSampleText(IWebElement webElement) => "3035551212";
    }
}
