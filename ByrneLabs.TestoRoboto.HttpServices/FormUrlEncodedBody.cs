using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class FormUrlEncodedBody : Body, IEntity<FormUrlEncodedBody>
    {
        public override string Fingerprint => string.Join(", ", FormData.Select(parameter => parameter.Key));

        public IList<KeyValue> FormData { get; } = new List<KeyValue>();

        public new FormUrlEncodedBody Clone(CloneDepth depth = CloneDepth.Deep) => (FormUrlEncodedBody) base.Clone(depth);
    }
}
