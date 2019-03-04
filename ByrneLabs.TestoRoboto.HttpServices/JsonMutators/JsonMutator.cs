using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.JsonMutators
{
    public abstract class JsonMutator : Mutator
    {
        public abstract IEnumerable<JObject> MutateJsonMessage(JObject message);

        public override IEnumerable<string> MutateMessage(string message) => MutateJsonMessage(JObject.Parse(message)).Select(jObject => jObject.ToString()).ToArray();
    }
}
