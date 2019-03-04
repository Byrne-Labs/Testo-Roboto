using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace ByrneLabs.TestoRoboto
{
    public class Request
    {
        public AuthenticationHeaderValue AuthenticationHeader { get; set; }

        public string LoggingDirectory { get; set; }

        public string Message { get; set; }

        public IList<HttpStatusCode> StatusCodesToLog { get; set; }

        public string Uri { get; set; }
    }
}
