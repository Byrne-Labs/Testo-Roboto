﻿using System.Collections.Generic;
using JetBrains.Annotations;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler
{
    [PublicAPI]
    public class CrawlOptions
    {
        public IList<string> AllowedUrlPatterns { get; } = new List<string>();

        public bool DisableImageDownloading { get; set; } = true;

        public bool HeadlessBrowsing { get; set; } = true;

        public int MaximumChainLength { get; set; } = 12;

        public int MaximumThreads { get; set; }

        public IList<Cookie> SessionCookies { get; } = new List<Cookie>();

        public IList<string> StartingUrls { get; } = new List<string>();
    }
}