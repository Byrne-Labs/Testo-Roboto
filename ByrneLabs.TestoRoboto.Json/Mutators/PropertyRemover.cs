using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.Json.Mutators
{
    public class PropertyRemover : Mutator
    {
        public override IEnumerable<JObject> MutateMessage(JObject message)
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
