using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class JsonMutator : Mutator
    {
        protected abstract IEnumerable<string> MutateMessage(string message);

        protected override IEnumerable<RequestMessage> MutateMessage1(RequestMessage requestMessage)
        {
            var mutatedRequestMessages = new List<RequestMessage>();
            if (requestMessage.Body is RawBody rawBody)
            {
                var mutateMessages = MutateMessage(rawBody.Text);
                foreach (var mutatedMessage in mutateMessages)
                {
                    var mutatedRequestMessage = requestMessage.Clone();
                    mutatedRequestMessage.ExpectedStatusCode = null;
                    ((RawBody) mutatedRequestMessage.Body).Text = mutatedMessage;
                    mutatedRequestMessages.Add(mutatedRequestMessage);
                }
            }

            return mutatedRequestMessages;
        }
    }
}
