﻿using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.JsonMutators
{
    public class PropertyAdder : Mutator
    {
        public override IEnumerable<string> MutateMessage(string message)
        {
            var jObject = JObject.Parse(message);
            var mutatedMessages = new List<JObject>();
            foreach (var descendent in jObject.Descendants().OfType<JObject>())
            {
                var clonedMessage = jObject.DeepClone();
                var clonedObject = clonedMessage.SelectToken(descendent.Path);
                clonedObject[BetterRandom.NextString(20, 20, BetterRandom.CharacterGroup.Alpha)] = Guid.NewGuid();
                mutatedMessages.Add((JObject) clonedMessage);
            }

            return mutatedMessages.Select(mutatedMessage => mutatedMessage.ToString()).ToArray();
        }
    }
}