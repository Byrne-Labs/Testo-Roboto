using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers
{
    public class HeaderDeleter : Mutator
    {
        protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<RequestMessage>();
            foreach (var header in requestMessage.Headers)
            {
                var fuzzedRequestMessage = requestMessage.Clone();
                var unfuzzedHeader = fuzzedRequestMessage.Headers.Single(p => p.Key == header.Key);
                fuzzedRequestMessage.Headers.Remove(unfuzzedHeader);
                fuzzedRequestMessages.Add(fuzzedRequestMessage);
            }

            return fuzzedRequestMessages;
        }
    }
}
