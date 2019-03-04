using System.Collections.Generic;
using System.Net.Http;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class TestRequest
    {
        private readonly IList<KeyValuePair<string, IEnumerable<string>>> _headers = new List<KeyValuePair<string, IEnumerable<string>>>();

        public int DelayBetweenMessages { get; set; }

        public Fuzzer Fuzzer { get; set; }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers => _headers;

        public HttpMethod HttpMethod { get; set; }

        public string Message { get; set; }

        public IList<Mutator> Mutators { get; set; } = new List<Mutator>();

        public void AddHeader(string name, string value)
        {
            _headers.Add(new KeyValuePair<string, IEnumerable<string>>(name, new[] { value }));
        }

        public void AddHeader(string name, IEnumerable<string> values)
        {
            _headers.Add(new KeyValuePair<string, IEnumerable<string>>(name, values));
        }
    }
}
