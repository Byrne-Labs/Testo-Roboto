using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class HeaderMutator : Mutator
    {
        protected abstract IEnumerable<Header> MutateHeader(Header header);

        protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<RequestMessage>();
            foreach (var header in requestMessage.Headers)
            {
                var headerIndex = requestMessage.Headers.IndexOf(header);
                var fuzzedHeaders = MutateHeader(header.Clone());
                foreach (var fuzzedHeader in fuzzedHeaders)
                {
                    var fuzzedRequestMessage = requestMessage.Clone();
                    var unfuzzedParameter = fuzzedRequestMessage.Headers.Single(h => h.Key == header.Key);
                    fuzzedRequestMessage.Headers.Remove(unfuzzedParameter);
                    fuzzedRequestMessage.Headers.Insert(headerIndex, fuzzedHeader);
                    fuzzedRequestMessages.Add(fuzzedRequestMessage);
                }
            }

            return fuzzedRequestMessages;
        }
    }
}
