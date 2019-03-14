using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class ArrayGrowerTest : MutatorTestBase
    {
        [Fact]
        public void TestArrayGrower()
        {
            TestJsonMessageCountReturned<ArrayGrower>(4);
        }
    }
}
