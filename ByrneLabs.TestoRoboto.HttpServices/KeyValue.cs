using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class KeyValue : Entity<KeyValue>
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
