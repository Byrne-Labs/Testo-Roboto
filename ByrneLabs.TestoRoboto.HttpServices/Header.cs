using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Header : Entity<Header>
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
