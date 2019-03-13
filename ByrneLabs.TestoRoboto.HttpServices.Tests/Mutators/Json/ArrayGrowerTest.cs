using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
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
