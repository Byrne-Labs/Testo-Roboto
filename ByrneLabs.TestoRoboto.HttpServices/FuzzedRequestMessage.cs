using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class FuzzedRequestMessage : RequestMessage, ICloneable<FuzzedRequestMessage>
    {
        [Key(12)]
        public RequestMessage SourceRequestMessage { get; set; }

        public new FuzzedRequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (FuzzedRequestMessage) base.Clone(depth);
    }
}
