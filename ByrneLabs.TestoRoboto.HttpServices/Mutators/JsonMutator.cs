using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class JsonMutator : Mutator
    {
        public override IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            var mutatedRequestMessages = new List<FuzzedRequestMessage>();
            if (requestMessage.Body is RawBody rawBody)
            {
                var mutateMessages = MutateMessage(rawBody.Text);
                foreach (var mutatedMessage in mutateMessages)
                {
                    var mutatedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                    mutatedRequestMessage.Name = mutatedRequestMessage.Name + " -- Fuzzed";
                    mutatedRequestMessage.ExpectedStatusCode = null;
                    ((RawBody) mutatedRequestMessage.Body).Text = mutatedMessage;
                    mutatedRequestMessages.Add(mutatedRequestMessage);
                }
            }

            return mutatedRequestMessages;
        }

        protected abstract IEnumerable<string> MutateMessage(string message);
    }
}
