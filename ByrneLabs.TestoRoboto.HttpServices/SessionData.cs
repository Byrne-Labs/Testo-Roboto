using System.Collections.Generic;
using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class SessionData : HandyObject<SessionData>
    {
        [Key(0)]
        public List<Cookie> Cookies { get; } = new List<Cookie>();

        [Key(1)]
        public List<KeyValue> FormParameters { get; } = new List<KeyValue>();

        [Key(2)]
        public List<Header> Headers { get; } = new List<Header>();

        [Key(3)]
        public List<QueryStringParameter> QueryStringParameters { get; } = new List<QueryStringParameter>();
    }
}
