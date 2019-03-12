using System.Collections.Generic;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class FormUrlEncodedBody : Body, IEntity<FormUrlEncodedBody>
    {
        public IList<KeyValue> FormData { get; } = new List<KeyValue>();

        public new FormUrlEncodedBody Clone(CloneDepth depth = CloneDepth.Deep) => (FormUrlEncodedBody) base.Clone(depth);
    }
}
