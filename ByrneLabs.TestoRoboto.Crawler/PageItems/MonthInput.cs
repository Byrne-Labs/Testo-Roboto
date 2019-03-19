using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class MonthInput : InputElementHandler
    {
        public override string Identifier => "MonthInput";

        public override IEnumerable<string> InputTypes => new[] { "month" };

        protected override string GetSampleText(IWebElement webElement) => "January\t2019";
    }
}
