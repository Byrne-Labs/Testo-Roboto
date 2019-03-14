using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources
{
    public class Blns : JsonStringFileResource
    {
        static Blns()
        {
            Values = LoadStringsFromResource(typeof(Blns).Assembly, "ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources.blns.txt");
        }

        protected Blns()
        {
        }

        public static IEnumerable<string> Values { get; set; }
    }
}
