using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public class DateInput : InputElementHandler
    {
        public override string Identifier => "DateInput";

        public override IEnumerable<string> InputTypes => new[] { "date" };

        public override void FillInput(RemoteWebDriver webDriver, PageItem input)
        {
            var element = FindElement(webDriver, input);
            element.SendKeys("01/01/2019");
        }
    }
}
