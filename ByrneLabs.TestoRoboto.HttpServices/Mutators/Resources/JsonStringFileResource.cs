using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources
{
    public class JsonStringFileResource
    {
        protected JsonStringFileResource()
        {
        }

        protected static IEnumerable<string> LoadStringsFromResource(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var list = JObject.Parse(json);
                return list["values"].Select(v => v.ToObject<string>()).ToArray();
            }
        }
    }
}
