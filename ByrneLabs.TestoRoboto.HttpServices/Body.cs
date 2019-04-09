using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    [Union(0, typeof(FormDataBody))]
    [Union(1, typeof(FormUrlEncodedBody))]
    [Union(2, typeof(RawBody))]
    public abstract class Body : HandyObject<Body>
    {
        [IgnoreMember]
        public abstract string Fingerprint { get; }
    }
}
