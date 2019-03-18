using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;

namespace ByrneLabs.TestoRoboto.Crawler.ItemHandlers
{
    public abstract class InputElementHandler : DataInputHandler
    {
        public abstract IEnumerable<string> InputTypes { get; }

        public override bool CanHandle(PageItem input) => input.Tag == "input" && InputTypes.Contains(input.Type) && !(string.IsNullOrWhiteSpace(input.Id) && string.IsNullOrWhiteSpace(input.Name));

        public override IEnumerable<PageItem> FindDataInputs(RemoteWebDriver webDriver)
        {
            var typeCheck = string.Join(" or ", InputTypes.Select(inputType => $"@type='{inputType}'"));
            return webDriver.FindElementsByXPath("//input[" + typeCheck + "]").Select(webElement =>
                 new PageItem(
                     webElement.GetAttribute("id"),
                     webElement.GetAttribute("name"),
                     webElement.GetAttribute("onclick"),
                     webElement.GetAttribute("src"),
                     webElement.TagName,
                     null,
                     webElement.GetAttribute("type"),
                     Identifier)
            ).ToArray();
        }
    }
}
