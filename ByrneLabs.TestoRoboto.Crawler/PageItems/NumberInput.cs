using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class NumberInput : InputElementHandler
    {
        public override string Identifier => "NumberInput";

        public override IEnumerable<string> InputTypes => new[] { "number" };

        protected override string GetSampleText(IWebElement webElement)
        {
            var maxValueString = webElement.GetAttribute("max");
            var minValueString = webElement.GetAttribute("min");
            string sampleText;
            if (!string.IsNullOrWhiteSpace(maxValueString))
            {
                sampleText = maxValueString;
            }
            else if (!string.IsNullOrWhiteSpace(minValueString))
            {
                sampleText = minValueString;
            }
            else
            {
                sampleText = "123";
            }

            return sampleText;
        }
    }
}
