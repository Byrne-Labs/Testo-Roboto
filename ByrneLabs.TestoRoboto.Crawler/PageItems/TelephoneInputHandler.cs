using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class TelephoneInputHandler : InputHandlerBase
    {
        public override string Identifier => "TelephoneInput";

        public override IEnumerable<string> InputTypes => new[] { "tel" };

        protected override string GetSampleText(IWebElement webElement) => "3035551212";
    }
}
