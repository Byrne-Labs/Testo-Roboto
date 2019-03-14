using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.FormParameters
{
    public abstract class ValueChanger : FormParameterMutator
    {
        protected abstract IEnumerable<object> TestValues { get; }

        protected override IEnumerable<KeyValue> MutateParameter(KeyValue parameter)
        {
            var mutatedParameters = new List<KeyValue>();
            foreach (var testValue in TestValues)
            {
                var mutatedParameter = parameter.Clone();
                mutatedParameter.Value = testValue.ToString();
                mutatedParameters.Add(mutatedParameter);
            }

            return mutatedParameters;
        }
    }
}
