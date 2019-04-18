using System.Web;
using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class QueryStringParameter : HandyObject<QueryStringParameter>
    {
        [Key(0)]
        public string Description { get; set; }

        [Key(1)]
        public string Key { get; set; }

        [Key(2)]
        public string UriEncodedValue { get; set; }

        [IgnoreMember]
        public string Value
        {
            get => HttpUtility.UrlDecode(UriEncodedValue);
            set => UriEncodedValue = HttpUtility.UrlEncode(value);
        }
    }
}
