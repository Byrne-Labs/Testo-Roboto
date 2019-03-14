using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources
{
    public class BlnsBase64 : JsonStringFileResource
    {
        static BlnsBase64()
        {
            Values = LoadStringsFromResource(typeof(BlnsBase64).Assembly, "ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources.blns.base64.json");
        }

        protected BlnsBase64()
        {
        }

        public static IEnumerable<string> Values { get; set; }
    }
}
