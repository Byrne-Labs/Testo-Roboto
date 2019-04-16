using System;
using System.Collections.Generic;
using System.Net;
using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class ResponseMessage : HandyObject<ResponseMessage>
    {
        [Key(0)]
        public string Content { get; set; }

        [Key(1)]
        public List<Cookie> Cookies { get; } = new List<Cookie>();

        [Key(2)]
        public Exception Exception { get; set; }

        [Key(3)]
        public List<Header> Headers { get; } = new List<Header>();

        [Key(4)]
        public Version HttpVersion { get; set; }

        [Key(5)]
        public DateTime Received { get; set; }

        [Key(6)]
        public DateTime RequestSent { get; set; }

        [Key(7)]
        public HttpStatusCode StatusCode { get; set; }
    }
}
