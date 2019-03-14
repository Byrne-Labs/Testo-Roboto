using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class QueryStringParameterMutator : Mutator
    {
        protected abstract IEnumerable<QueryStringParameter> MutateQueryStringParameter(QueryStringParameter queryStringParameter);

        protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<RequestMessage>();
            foreach (var queryStringParameter in requestMessage.QueryStringParameters)
            {
                var queryStringParameterIndex = requestMessage.QueryStringParameters.IndexOf(queryStringParameter);
                var fuzzedParameters = MutateQueryStringParameter(queryStringParameter.Clone());
                foreach (var fuzzedParameter in fuzzedParameters)
                {
                    var fuzzedRequestMessage = requestMessage.Clone();
                    var unfuzzedParameter = fuzzedRequestMessage.QueryStringParameters.Single(p => p.Key == queryStringParameter.Key);
                    fuzzedRequestMessage.QueryStringParameters.Remove(unfuzzedParameter);
                    fuzzedRequestMessage.QueryStringParameters.Insert(queryStringParameterIndex, fuzzedParameter);
                    fuzzedRequestMessages.Add(fuzzedRequestMessage);
                }
            }

            return fuzzedRequestMessages;
        }
    }
}
