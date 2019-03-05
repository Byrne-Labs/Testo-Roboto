using System.Collections.Generic;
using System.Net.Http;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class RequestMessage : Item, IEntity<RequestMessage>
    {
        public string Body { get; set; }

        public List<Cookie> Cookies { get; set; } = new List<Cookie>();

        public bool FuzzedMessage { get; set; }

        public List<Header> Headers { get; set; } = new List<Header>();

        public HttpMethod HttpMethod { get; set; }

        public List<QueryStringParameter> QueryStringParameters { get; set; } = new List<QueryStringParameter>();

        public new RequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (RequestMessage) base.Clone(depth);
    }
}
