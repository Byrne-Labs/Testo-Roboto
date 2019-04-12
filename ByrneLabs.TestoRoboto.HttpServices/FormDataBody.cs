using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
//using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class FormDataBody : Body, ICloneable<FormDataBody>
    {
        [IgnoreMember]
        public override string Fingerprint => string.Join(", ", FormData.Select(parameter => parameter.Key));

        [Key(0)]
        public List<KeyValue> FormData { get; } = new List<KeyValue>();

        public static FormDataBody GetFromBodyText(string body) => throw new NotImplementedException();

        public new FormDataBody Clone(CloneDepth depth = CloneDepth.Deep) => (FormDataBody) base.Clone(depth);
    }
}
