using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
{
    public class ArrayGrowerTest : MutatorTest
    {
        [Fact]
        public void TestArrayGrower()
        {
            TestMessageCountReturned<ArrayGrower>(4);
        }
    }
}
