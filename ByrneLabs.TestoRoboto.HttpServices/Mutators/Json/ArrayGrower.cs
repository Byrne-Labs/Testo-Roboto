using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Json
{
    public class ArrayGrower : JsonMutator
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static JObject AddDuplicate(JArray originalArray, int duplicateCount)
        {
            var clonedMessage = originalArray.Root.DeepClone();
            var clonedArray = clonedMessage.SelectToken(originalArray.Path);
            while (clonedArray.Count() < originalArray.Count + duplicateCount)
            {
                var duplicate = originalArray.RandomItem().DeepClone();
                if (BetterRandom.NextBool())
                {
                    clonedArray.RandomItem().AddAfterSelf(duplicate);
                }
                else
                {
                    clonedArray.RandomItem().AddBeforeSelf(duplicate);
                }
            }

            return (JObject) clonedMessage;
        }

        protected override IEnumerable<string> MutateMessage(string message)
        {
            var jObject = JObject.Parse(message);
            var mutatedMessages = new List<JObject>();
            foreach (var array in jObject.Descendants().OfType<JArray>())
            {
                mutatedMessages.Add(AddDuplicate(array, 1));
                mutatedMessages.Add(AddDuplicate(array, 5000));
            }

            return mutatedMessages.Select(mutatedMessage => mutatedMessage.ToString()).ToArray();
        }
    }
}
