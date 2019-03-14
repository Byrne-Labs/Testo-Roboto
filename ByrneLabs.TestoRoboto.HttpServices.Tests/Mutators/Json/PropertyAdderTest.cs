using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class PropertyAdderTest : MutatorTestBase
    {
        [Fact]
        public void TestPropertyAdder()
        {
            TestJsonMessageCountReturned<PropertyAdder>(4);
        }
    }
}
