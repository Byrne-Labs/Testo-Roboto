using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.JsonMutators
{
    public class PropertyRemover : JsonMutator
    {
        public override IEnumerable<JObject> MutateJsonMessage(JObject message)
        {
            var mutatedMessages = new List<JObject>();
            foreach (var property in message.Descendants().OfType<JProperty>().Where(p => p.HasValues))
            {
                var clonedMessage = message.DeepClone();
                var clonedProperty = clonedMessage.SelectToken(property.Path).Parent;
                clonedProperty.Remove();
                mutatedMessages.Add((JObject) clonedMessage);
            }

            return mutatedMessages;
        }
    }
}
