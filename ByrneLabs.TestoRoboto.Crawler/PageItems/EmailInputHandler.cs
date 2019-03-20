using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class EmailInputHandler : InputHandlerBase
    {
        public override string Identifier => "EmailInput";

        public override IEnumerable<string> InputTypes => new[] { "email" };

        protected override string GetSampleText(IWebElement webElement) => "first.last@somemadeupwebsitethatdoesnotexist.com";
    }
}
