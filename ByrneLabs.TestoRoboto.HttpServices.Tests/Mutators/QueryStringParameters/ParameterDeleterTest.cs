using ByrneLabs.TestoRoboto.HttpServices.Mutators.QueryStringParameters;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.QueryStringParameters
{
    public class ParameterDeleterTest : MutatorTestBase
    {
        [Fact]
        public void TestParameterDeleter()
        {
            TestJsonMessageCountReturned<ParameterDeleter>(2);
        }
    }
}
