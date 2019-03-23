using System.Web;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class QueryStringParameter : HandyObject<QueryStringParameter>
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string UriEncodedValue { get; set; }

        public string Value
        {
            get => HttpUtility.UrlDecode(UriEncodedValue);
            set => UriEncodedValue = HttpUtility.UrlEncode(value);
        }
    }
}
