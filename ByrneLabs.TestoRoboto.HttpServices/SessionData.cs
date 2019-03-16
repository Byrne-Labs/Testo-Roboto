using System.Collections.Generic;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class SessionData : Entity<SessionData>
    {
        public IList<Cookie> Cookies { get; } = new List<Cookie>();

        public IList<KeyValue> FormParameters { get; } = new List<KeyValue>();

        public IList<Header> Headers { get; } = new List<Header>();

        public IList<QueryStringParameter> QueryStringParameters { get; } = new List<QueryStringParameter>();
    }
}
