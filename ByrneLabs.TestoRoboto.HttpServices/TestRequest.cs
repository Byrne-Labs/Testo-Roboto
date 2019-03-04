using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class TestRequest
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public IList<Mutator> Mutators { get; set; } = new List<Mutator>();

        public HttpRequestMessage RequestMessage { get; set; } = new HttpRequestMessage();

        public int TimeBetweenRequests { get; set; }
    }
}
