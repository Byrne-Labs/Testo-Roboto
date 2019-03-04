using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.Json.Mutators
{
    public abstract class Mutator
    {
        public abstract IEnumerable<JObject> MutateMessage(JObject message);
    }
}
