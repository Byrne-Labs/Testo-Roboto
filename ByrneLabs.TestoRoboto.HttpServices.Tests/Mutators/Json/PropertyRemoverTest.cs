using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class PropertyRemoverTest : MutatorTestBase
    {
        [Fact]
        public void TestPropertyRemover()
        {
            TestJsonMessageCountReturned<PropertyRemover>(13);
        }
    }
}
