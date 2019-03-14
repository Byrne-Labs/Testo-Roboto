using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.QueryStringParameters
{
    public abstract class ValueChanger : QueryStringParameterMutator
    {
        protected abstract IEnumerable<object> TestValues { get; }

        protected override IEnumerable<QueryStringParameter> MutateQueryStringParameter(QueryStringParameter queryStringParameter)
        {
            var mutatedQueryStringParameters = new List<QueryStringParameter>();
            foreach (var testValue in TestValues)
            {
                var mutatedQueryStringParameter = queryStringParameter.Clone();
                mutatedQueryStringParameter.Value = testValue.ToString();
                mutatedQueryStringParameters.Add(mutatedQueryStringParameter);
            }

            return mutatedQueryStringParameters;
        }
    }
}
