using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
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
