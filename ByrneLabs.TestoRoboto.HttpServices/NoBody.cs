using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class NoBody : Body, ICloneable<NoBody>
    {
        public override string Fingerprint => string.Empty;

        public new NoBody Clone(CloneDepth depth = CloneDepth.Deep) => (NoBody) base.Clone(depth);
    }
}
