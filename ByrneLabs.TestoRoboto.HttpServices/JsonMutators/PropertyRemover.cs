using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.JsonMutators
{
    public class PropertyRemover : Mutator
    {
        public override IEnumerable<string> MutateMessage(string message)
        {
            var jObject = JObject.Parse(message);
            var mutatedMessages = new List<JObject>();
            foreach (var property in jObject.Descendants().OfType<JProperty>().Where(p => p.HasValues))
            {
                var clonedMessage = jObject.DeepClone();
                var clonedProperty = clonedMessage.SelectToken(property.Path).Parent;
                clonedProperty.Remove();
                mutatedMessages.Add((JObject) clonedMessage);
            }

            return mutatedMessages.Select(mutatedMessage => mutatedMessage.ToString()).ToArray();
        }
    }
}
