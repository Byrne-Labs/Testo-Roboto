using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.Json.Mutators
{
    public class ArrayShrinker : JsonMutator
    {
        private static JObject ShrinkArray(JArray originalArray, int countToRemove)
        {
            var clonedMessage = originalArray.Root.DeepClone();
            var clonedArray = clonedMessage.SelectToken(originalArray.Path);
            while (clonedArray.Count() > originalArray.Count - countToRemove && clonedArray.Any())
            {
                clonedArray.RandomItem().Remove();
            }

            return (JObject) clonedMessage;
        }

        public override IEnumerable<JObject> MutateJsonMessage(JObject message)
        {
            var mutatedMessages = new List<JObject>();
            foreach (var array in message.Descendants().OfType<JArray>())
            {
                mutatedMessages.Add(ShrinkArray(array, 1));
                if (array.Count > 1)
                {
                    mutatedMessages.Add(ShrinkArray(array, int.MaxValue));
                }
            }

            return mutatedMessages.Distinct();
        }
    }
}
