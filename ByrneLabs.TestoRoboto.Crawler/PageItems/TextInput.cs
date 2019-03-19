using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class TextInput : InputElementHandler
    {
        public override string Identifier => "TextInput";

        public override IEnumerable<string> InputTypes => new[] { "text", "password", string.Empty, null };

        protected override string GetSampleText(IWebElement webElement)
        {
            var maxLengthString = webElement.GetAttribute("maxlength");
            string sampleText;
            if (int.TryParse(maxLengthString, out var maxLength))
            {
                sampleText = new string('a', maxLength);
            }
            else
            {
                sampleText = new string('a', 10);
            }

            return sampleText;
        }
    }
}
