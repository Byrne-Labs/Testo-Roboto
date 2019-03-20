using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class TimeInputHandler : InputHandlerBase
    {
        public override string Identifier => "TimeInput";

        public override IEnumerable<string> InputTypes => new[] { "time" };

        protected override string GetSampleText(IWebElement webElement) => "12:00PM";
    }
}
