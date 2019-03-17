using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class HeaderMutator : Mutator
    {
        public override IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<FuzzedRequestMessage>();
            foreach (var header in requestMessage.Headers.Where(h => h.Key != "Content-Type"))
            {
                var headerIndex = requestMessage.Headers.IndexOf(header);
                var fuzzedHeaders = MutateHeader(header.Clone());
                foreach (var fuzzedHeader in fuzzedHeaders)
                {
                    var fuzzedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                    var unfuzzedParameter = fuzzedRequestMessage.Headers.Single(h => h.Key == header.Key);
                    fuzzedRequestMessage.Headers.Remove(unfuzzedParameter);
                    fuzzedRequestMessage.Headers.Insert(headerIndex, fuzzedHeader);
                    fuzzedRequestMessages.Add(fuzzedRequestMessage);
                }
            }

            return fuzzedRequestMessages;
        }

        protected abstract IEnumerable<Header> MutateHeader(Header header);
    }
}
