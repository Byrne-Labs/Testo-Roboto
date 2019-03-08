using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class Mutator
    {
        public virtual IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = MutateMessage1(requestMessage);
            foreach (var fuzzedRequestMessage in fuzzedRequestMessages)
            {
                fuzzedRequestMessage.FuzzedMessage = true;
            }

            return fuzzedRequestMessages;
        }

        protected abstract IEnumerable<RequestMessage> MutateMessage1(RequestMessage requestMessage);
    }
}
