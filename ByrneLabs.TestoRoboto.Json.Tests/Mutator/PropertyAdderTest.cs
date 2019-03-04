using ByrneLabs.TestoRoboto.Json.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
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
