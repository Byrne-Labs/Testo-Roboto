using System.Collections.Generic;
using System.Net;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class FuzzTestCase
    {
        public IEnumerable<HttpStatusCode> ExpectedStatusCodes { get; }

        public string ContentType { get; set; }

        public string Message { get; set; }

        public abstract void Execute();
    }
}
