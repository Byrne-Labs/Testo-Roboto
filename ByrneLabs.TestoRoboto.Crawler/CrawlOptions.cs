using System.Collections.Generic;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler
{
    public class CrawlOptions
    {
        public IList<string> AllowedUrlPatterns { get; } = new List<string>();

        public int MaximumChainLength { get; set; } = 12;

        public int MaximumThreads { get; set; }

        public IList<Cookie> SessionCookies { get; } = new List<Cookie>();

        public IList<string> StartingUrls { get; } = new List<string>();
    }
}
