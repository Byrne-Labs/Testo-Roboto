using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    public class FormUrlEncodedBody : Body, ICloneable<FormUrlEncodedBody>
    {
        [Key(0)]
        public override string Fingerprint => string.Join(", ", FormData.Select(parameter => parameter.Key));

        [Key(1)]
        public IList<KeyValue> FormData { get; } = new List<KeyValue>();

        public static FormUrlEncodedBody GetFromBodyText(string bodyText)
        {
            var body = new FormUrlEncodedBody();
            var parameters = bodyText.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parameterText in parameters)
            {
                var key = parameterText.SubstringBeforeLast("=");
                var value = parameterText.SubstringAfterLast("=");
                var parameter = new KeyValue { Key = key, Value = value };
                body.FormData.Add(parameter);
            }

            return body;
        }

        public new FormUrlEncodedBody Clone(CloneDepth depth = CloneDepth.Deep) => (FormUrlEncodedBody) base.Clone(depth);
    }
}
