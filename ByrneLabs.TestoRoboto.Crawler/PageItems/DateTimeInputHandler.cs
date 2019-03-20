using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class DateTimeInputHandler : InputElementHandlerBase
    {
        public override string Identifier => "DateTimeInput";

        public override IEnumerable<string> InputTypes => new[] { "datetime-local" };

        protected override string GetSampleText(IWebElement webElement) => "01/01/2019\t12:00PM";
    }
}
