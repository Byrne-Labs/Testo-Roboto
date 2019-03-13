using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class PropertyAdderTest : MutatorTest
    {
        [Fact]
        public void TestPropertyAdder()
        {
            TestMessageCountReturned<PropertyAdder>(4);
        }
    }
}
