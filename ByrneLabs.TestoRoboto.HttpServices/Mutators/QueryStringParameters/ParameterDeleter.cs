using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.QueryStringParameters
{
    public class ParameterDeleter : Mutator
    {
        public override IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<FuzzedRequestMessage>();
            foreach (var parameter in requestMessage.QueryStringParameters)
            {
                var fuzzedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                var unfuzzedParameter = fuzzedRequestMessage.QueryStringParameters.First(p => p.Key == parameter.Key);
                fuzzedRequestMessage.QueryStringParameters.Remove(unfuzzedParameter);
                fuzzedRequestMessages.Add(fuzzedRequestMessage);
            }

            return fuzzedRequestMessages;
        }
    }
}
