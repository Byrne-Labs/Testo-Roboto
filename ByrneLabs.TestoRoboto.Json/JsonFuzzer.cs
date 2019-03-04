using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.TestoRoboto.Common;
using ByrneLabs.TestoRoboto.Json.Mutators;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.Json
{
    public class JsonFuzzer : Fuzzer
    {
        public override IEnumerable<string> Fuzz(string message)
        {
            var jsonObject = JObject.Parse(message);
            var mutatorTypes = GetType().Assembly.GetTypes().Where(type => typeof(Mutator).IsAssignableFrom(type) && !type.IsAbstract).ToArray();
            var mutatorInstances = mutatorTypes.Select(Activator.CreateInstance).Cast<Mutator>().ToArray();
            var mutatedMessages = mutatorInstances.SelectMany(mutator => mutator.MutateMessage(jsonObject)).Select(json => json.ToString()).ToArray();

            return mutatedMessages;
        }
    }
}
