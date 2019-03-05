using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Cookie : Entity<Cookie>
    {
        public string Domain { get; set; }

        public string Path { get; set; }

        public string Value { get; set; }
    }
}
