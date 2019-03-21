using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class RawBody : Body, IEntity<RawBody>
    {
        public override string Fingerprint => Text;

        public string Text { get; set; }

        public static RawBody GetFromBodyText(string body) => new RawBody { Text = body };

        public new RawBody Clone(CloneDepth depth = CloneDepth.Deep) => (RawBody) base.Clone(depth);

        public override string ToString() => Text;
    }
}
