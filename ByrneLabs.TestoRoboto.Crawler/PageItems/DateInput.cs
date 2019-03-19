using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class DateInput : InputElementHandler
    {
        public override string Identifier => "DateInput";

        public override IEnumerable<string> InputTypes => new[] { "date" };

        protected override string GetSampleText(IWebElement webElement) => "01/01/2019";
    }
}
