using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class FuzzedRequestMessage : RequestMessage, IEntity<FuzzedRequestMessage>
    {
        public RequestMessage SourceRequestMessage { get; set; }

        public new FuzzedRequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (FuzzedRequestMessage) base.Clone(depth);
    }
}
