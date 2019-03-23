using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class FormDataBody : Body, ICloneable<FormDataBody>
    {
        public override string Fingerprint => string.Join(", ", FormData.Select(parameter => parameter.Key));

        public IList<KeyValue> FormData { get; } = new List<KeyValue>();

        public static FormDataBody GetFromBodyText(string body) => throw new NotImplementedException();

        public new FormDataBody Clone(CloneDepth depth = CloneDepth.Deep) => (FormDataBody) base.Clone(depth);
    }
}
