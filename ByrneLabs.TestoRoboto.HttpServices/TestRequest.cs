using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class TestRequest
    {
        public bool ExcludeDuplicateFingerprintRequests { get; set; } = true;

        public bool ExcludeUnfuzzableRequests { get; set; } = true;

        public IList<Item> Items { get; set; } = new List<Item>();

        public int TimeBetweenRequests { get; set; }
    }
}
