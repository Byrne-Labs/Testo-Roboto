using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class ArrayShrinkerTest : MutatorTestBase
    {
        [Fact]
        public void TestArrayShrinker()
        {
            TestJsonMessageCountReturned<ArrayShrinker>(3);
        }
    }
}
