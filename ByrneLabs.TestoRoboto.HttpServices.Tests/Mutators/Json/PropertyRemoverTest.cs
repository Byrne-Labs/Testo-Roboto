using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
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
