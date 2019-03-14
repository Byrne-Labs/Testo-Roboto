using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class FormParameterMutator : Mutator
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
                var parameterIndex = formDataBody.FormData.IndexOf(parameter);
                var fuzzedParameters = MutateParameter(parameter.Clone());
                foreach (var fuzzedParameter in fuzzedParameters)
                {
                    var fuzzedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                    var fuzzedFormDataBody = fuzzedRequestMessage.Body as FormDataBody;
                    var unfuzzedParameter = fuzzedFormDataBody.FormData.Single(p => p.Key == parameter.Key);
                    fuzzedFormDataBody.FormData.Remove(unfuzzedParameter);
                    fuzzedFormDataBody.FormData.Insert(parameterIndex, fuzzedParameter);
                    fuzzedRequestMessages.Add(fuzzedRequestMessage);
                }
            }

            return fuzzedRequestMessages;
        }

        protected abstract IEnumerable<KeyValue> MutateParameter(KeyValue parameter);
    }
}
