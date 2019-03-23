using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class Body : HandyObject<Body>
    {
        public abstract string Fingerprint { get; }
    }
}
