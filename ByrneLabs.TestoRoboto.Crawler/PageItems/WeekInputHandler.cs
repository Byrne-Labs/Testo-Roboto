﻿using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class WeekInputHandler : InputElementHandlerBase
    {
        public override string Identifier => "WeekInput";

        public override IEnumerable<string> InputTypes => new[] { "week" };

        protected override string GetSampleText(IWebElement webElement) => "012019";
    }
}