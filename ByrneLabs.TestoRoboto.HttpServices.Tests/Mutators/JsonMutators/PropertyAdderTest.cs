using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
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
