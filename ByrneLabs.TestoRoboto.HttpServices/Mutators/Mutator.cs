using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class Mutator
    {
        public virtual IEnumerable<RequestMessage> MutateMessages(RequestMessage requestMessage)
        {
            var fuzzedRequestMessages = MutateMessage(requestMessage);
            foreach (var fuzzedRequestMessage in fuzzedRequestMessages)
            {
                fuzzedRequestMessage.FuzzedMessage = true;
            }

            return fuzzedRequestMessages;
        }

        protected abstract IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage);
    }
}
