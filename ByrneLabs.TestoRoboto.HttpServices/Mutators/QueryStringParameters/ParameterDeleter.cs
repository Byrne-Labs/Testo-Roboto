using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.QueryStringParameters
{
    public class ParameterDeleter : Mutator
    {
        protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<RequestMessage>();
            foreach (var parameter in requestMessage.QueryStringParameters)
            {
                var fuzzedRequestMessage = requestMessage.Clone();
                var unfuzzedParameter = fuzzedRequestMessage.QueryStringParameters.Single(p => p.Key == parameter.Key);
                fuzzedRequestMessage.QueryStringParameters.Remove(unfuzzedParameter);
                fuzzedRequestMessages.Add(fuzzedRequestMessage);
            }

            return fuzzedRequestMessages;
        }
    }
}
