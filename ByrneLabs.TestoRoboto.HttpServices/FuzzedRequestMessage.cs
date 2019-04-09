using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    public class FuzzedRequestMessage : RequestMessage, ICloneable<FuzzedRequestMessage>
    {
        [Key(12)]
        public RequestMessage SourceRequestMessage { get; set; }

        public new FuzzedRequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (FuzzedRequestMessage) base.Clone(depth);
    }
}
