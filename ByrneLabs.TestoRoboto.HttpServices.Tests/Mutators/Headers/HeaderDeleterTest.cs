using ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Headers
{
    public class HeaderDeleterTest : MutatorTestBase
    {
        [Fact]
        public void TestParameterDeleter()
        {
            TestJsonMessageCountReturned<HeaderDeleter>(2);
        }
    }
}
