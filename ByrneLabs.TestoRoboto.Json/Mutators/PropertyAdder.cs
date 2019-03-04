using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.Json.Mutators
{
    public class PropertyAdder : Mutator
    {
        public override IEnumerable<JObject> MutateMessage(JObject message)
        {
            var mutatedMessages = new List<JObject>();
            foreach (var jObject in message.Descendants().OfType<JObject>())
            {
                var clonedMessage = message.DeepClone();
                var clonedObject = clonedMessage.SelectToken(jObject.Path);
                clonedObject[BetterRandom.NextString(20, 20, BetterRandom.CharacterGroup.Alpha)] = Guid.NewGuid();
                mutatedMessages.Add((JObject) clonedMessage);
            }

            return mutatedMessages;
        }
    }
}
