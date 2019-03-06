using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class RawBody : Body, IEntity<RawBody>
    {
        public string Text { get; set; }

        public new RawBody Clone(CloneDepth depth = CloneDepth.Deep) => (RawBody) base.Clone(depth);
    }
}
