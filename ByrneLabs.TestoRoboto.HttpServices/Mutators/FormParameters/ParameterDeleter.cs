using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.FormParameters
{
    public class ParameterDeleter : Mutator
    {
        public override IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            if (!(requestMessage.Body is FormDataBody formDataBody))
            {
                return Enumerable.Empty<FuzzedRequestMessage>();
            }

            var fuzzedRequestMessages = new List<FuzzedRequestMessage>();
            foreach (var parameter in formDataBody.FormData)
            {
                var fuzzedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                var fuzzedFormDataBody = fuzzedRequestMessage.Body as FormDataBody;
                var unfuzzedParameter = fuzzedFormDataBody.FormData.Single(p => p.Key == parameter.Key);
                fuzzedFormDataBody.FormData.Remove(unfuzzedParameter);
                fuzzedRequestMessages.Add(fuzzedRequestMessage);
            }

            return fuzzedRequestMessages;
        }
    }
}
