using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers
{
    public class HeaderDeleter : Mutator
    {
        public override IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = new List<FuzzedRequestMessage>();
            foreach (var header in requestMessage.Headers)
            {
                var fuzzedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                var unfuzzedHeader = fuzzedRequestMessage.Headers.Single(p => p.Key == header.Key);
                fuzzedRequestMessage.Headers.Remove(unfuzzedHeader);
                fuzzedRequestMessages.Add(fuzzedRequestMessage);
            }

            return fuzzedRequestMessages;
        }
    }
}
