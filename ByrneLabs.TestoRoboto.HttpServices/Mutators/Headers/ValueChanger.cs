using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers
{
    public abstract class ValueChanger : HeaderMutator
    {
        protected abstract IEnumerable<object> TestValues { get; }

        protected override IEnumerable<Header> MutateHeader(Header header)
        {
            var mutatedHeaders = new List<Header>();
            foreach (var testValue in TestValues)
            {
                var mutatedHeader = header.Clone();
                mutatedHeader.Value = testValue.ToString();
                mutatedHeaders.Add(mutatedHeader);
            }

            return mutatedHeaders;
        }
    }
}
