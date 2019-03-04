using ByrneLabs.TestoRoboto.Json.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
{
    public class PropertyRemoverTest : MutatorTest
    {
        [Fact]
        public void TestPropertyRemover()
        {
            TestMessageCountReturned<PropertyRemover>(13);
        }
    }
}
