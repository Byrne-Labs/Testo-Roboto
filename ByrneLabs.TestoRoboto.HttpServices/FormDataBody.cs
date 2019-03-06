using System.Collections.Generic;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class FormDataBody : Body, IEntity<FormDataBody>
    {
        public IList<KeyValue> FormData { get; } = new List<KeyValue>();

        public new FormDataBody Clone(CloneDepth depth = CloneDepth.Deep) => (FormDataBody) base.Clone(depth);
    }
}
