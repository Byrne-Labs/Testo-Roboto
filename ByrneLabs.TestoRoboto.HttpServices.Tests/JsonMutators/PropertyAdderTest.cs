using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
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
