using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public class TextInput : InputElementHandler
    {
        public override string Identifier => "TextInput";

        public override IEnumerable<string> InputTypes => new[] { "text", "password" };

        public override void FillInput(RemoteWebDriver webDriver, PageItem input)
        {
            var element = FindElement(webDriver, input);
            var maxLengthString = element.GetAttribute("maxlength");
            if (int.TryParse(maxLengthString, out var maxLength))
            {
                element.SendKeys(new string('a', maxLength));
            }
            else
            {
                element.SendKeys(new string('a', 10));
            }
        }
    }
}
