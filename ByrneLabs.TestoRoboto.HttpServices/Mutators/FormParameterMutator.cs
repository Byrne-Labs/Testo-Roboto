using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class FormParameterMutator : Mutator
    {
        protected abstract IEnumerable<KeyValue> MutateParameter(KeyValue parameter);

        protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
        {
            if (!(requestMessage.Body is FormDataBody formDataBody))
            {
                return Enumerable.Empty<RequestMessage>();
            }

            var fuzzedRequestMessages = new List<RequestMessage>();
            foreach (var parameter in formDataBody.FormData)
            {
                var parameterIndex = formDataBody.FormData.IndexOf(parameter);
                var fuzzedParameters = MutateParameter(parameter.Clone());
                foreach (var fuzzedParameter in fuzzedParameters)
                {
                    var fuzzedRequestMessage = requestMessage.Clone();
                    var fuzzedFormDataBody = fuzzedRequestMessage.Body as FormDataBody;
                    var unfuzzedParameter = fuzzedFormDataBody.FormData.Single(p => p.Key == parameter.Key);
                    fuzzedFormDataBody.FormData.Remove(unfuzzedParameter);
                    fuzzedFormDataBody.FormData.Insert(parameterIndex, fuzzedParameter);
                    fuzzedRequestMessages.Add(fuzzedRequestMessage);
                }
            }

            return fuzzedRequestMessages;
        }
    }
}
