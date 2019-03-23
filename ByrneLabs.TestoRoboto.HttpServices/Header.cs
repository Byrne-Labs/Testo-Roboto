using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Header : HandyObject<Header>
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
