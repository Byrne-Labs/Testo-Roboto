using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
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
