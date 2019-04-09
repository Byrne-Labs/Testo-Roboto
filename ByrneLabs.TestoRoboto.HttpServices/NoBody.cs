using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    public class NoBody : Body, ICloneable<NoBody>
    {
        [IgnoreMember]
        public override string Fingerprint => string.Empty;

        public new NoBody Clone(CloneDepth depth = CloneDepth.Deep) => (NoBody) base.Clone(depth);
    }
}
