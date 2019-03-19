using System;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(PageItem pageItem) : base($"Could not find element {pageItem}")
        {
        }
    }
}
