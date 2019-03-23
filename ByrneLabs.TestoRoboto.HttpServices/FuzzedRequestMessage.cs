using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class FuzzedRequestMessage : RequestMessage, ICloneable<FuzzedRequestMessage>
    {
        public RequestMessage SourceRequestMessage { get; set; }

        public new FuzzedRequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (FuzzedRequestMessage) base.Clone(depth);
    }
}
