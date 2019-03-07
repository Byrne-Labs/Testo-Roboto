using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class TestRequest
    {
        public IList<Item> Items { get; set; } = new List<Item>();

        public int TimeBetweenRequests { get; set; }
    }
}
