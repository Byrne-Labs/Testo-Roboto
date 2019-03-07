using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Cookie : Entity<Cookie>
    {
        public string Domain { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}
