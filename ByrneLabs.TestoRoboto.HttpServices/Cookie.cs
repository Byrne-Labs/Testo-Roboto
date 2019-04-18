using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class Cookie : HandyObject<Cookie>
    {
        [Key(4)]
        public string Description { get; set; } = string.Empty;

        [Key(0)]
        public string Domain { get; set; } = string.Empty;

        [Key(1)]
        public string Name { get; set; } = string.Empty;

        [Key(2)]
        public string Path { get; set; } = string.Empty;

        [Key(3)]
        public string Value { get; set; } = string.Empty;
    }
}
