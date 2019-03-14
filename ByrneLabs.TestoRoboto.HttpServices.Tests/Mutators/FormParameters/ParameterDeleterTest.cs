using ByrneLabs.TestoRoboto.HttpServices.Mutators.FormParameters;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.FormParameters
{
    public class ParameterDeleterTest : MutatorTestBase
    {
        [Fact]
        public void TestParameterDeleter()
        {
            TestFormMessageCountReturned<ParameterDeleter>(2);
        }
    }
}
