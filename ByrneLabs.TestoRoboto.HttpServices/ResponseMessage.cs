using System;
using System.Collections.Generic;
using System.Net;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class ResponseMessage : HandyObject<ResponseMessage>
    {
        public string Content { get; set; }

        public IList<Cookie> Cookies { get; set; } = new List<Cookie>();

        public Exception Exception { get; set; }

        public IList<Header> Headers { get; set; } = new List<Header>();

        public Version HttpVersion { get; set; }

        public DateTime Received { get; set; }

        public DateTime RequestSent { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
