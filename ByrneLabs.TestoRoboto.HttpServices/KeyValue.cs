using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class KeyValue : HandyObject<KeyValue>
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
