using ByrneLabs.TestoRoboto.Json.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
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
