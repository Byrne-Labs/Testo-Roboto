﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Json
{
    public class ArrayShrinker : JsonMutator
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
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

        protected override IEnumerable<string> MutateMessage(string message)
        {
            var jObject = JObject.Parse(message);
            var mutatedMessages = new List<JObject>();
            foreach (var array in jObject.Descendants().OfType<JArray>())
            {
                mutatedMessages.Add(ShrinkArray(array, 1));
                if (array.Count > 1)
                {
                    mutatedMessages.Add(ShrinkArray(array, int.MaxValue));
                }
            }

            return mutatedMessages.Select(mutatedMessage => mutatedMessage.ToString()).ToArray();
        }
    }
}
