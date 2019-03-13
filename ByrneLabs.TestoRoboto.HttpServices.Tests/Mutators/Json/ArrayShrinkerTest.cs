using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class ArrayShrinkerTest : MutatorTest
    {
        [Fact]
        public void TestArrayShrinker()
        {
            TestMessageCountReturned<ArrayShrinker>(3);
        }
    }
}
