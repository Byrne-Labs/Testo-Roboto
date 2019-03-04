using ByrneLabs.TestoRoboto.Json.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
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
