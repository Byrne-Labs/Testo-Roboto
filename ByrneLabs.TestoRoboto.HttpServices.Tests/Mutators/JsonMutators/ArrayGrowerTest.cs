using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
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
