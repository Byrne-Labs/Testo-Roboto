using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
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
