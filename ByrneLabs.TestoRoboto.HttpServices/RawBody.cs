using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class RawBody : Body, ICloneable<RawBody>
    {
        [IgnoreMember]
        public override string Fingerprint => Text;

        [Key(0)]
        public string Text { get; set; }

        public static RawBody GetFromBodyText(string body) => new RawBody { Text = body };

        public new RawBody Clone(CloneDepth depth = CloneDepth.Deep) => (RawBody) base.Clone(depth);

        public override string ToString() => Text;
    }
}
