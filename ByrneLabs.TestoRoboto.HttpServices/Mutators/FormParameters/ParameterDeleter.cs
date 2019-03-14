using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.FormParameters
{
    public class ParameterDeleter : Mutator
    {
        protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            if (!(requestMessage.Body is FormDataBody formDataBody))
            {
                return Enumerable.Empty<RequestMessage>();
            }

            var fuzzedRequestMessages = new List<RequestMessage>();
            foreach (var parameter in formDataBody.FormData)
            {
                var fuzzedRequestMessage = requestMessage.Clone();
                var fuzzedFormDataBody = fuzzedRequestMessage.Body as FormDataBody;
                var unfuzzedParameter = fuzzedFormDataBody.FormData.Single(p => p.Key == parameter.Key);
                fuzzedFormDataBody.FormData.Remove(unfuzzedParameter);
                fuzzedRequestMessages.Add(fuzzedRequestMessage);
            }

            return fuzzedRequestMessages;
        }
    }
}
