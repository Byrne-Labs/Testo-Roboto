using System.Collections.Generic;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class UrlEncodedBody : Body, IEntity<UrlEncodedBody>
    {
        public IList<KeyValue> FormData { get; } = new List<KeyValue>();

        public new UrlEncodedBody Clone(CloneDepth depth = CloneDepth.Deep) => (UrlEncodedBody)base.Clone(depth);
    }
}
