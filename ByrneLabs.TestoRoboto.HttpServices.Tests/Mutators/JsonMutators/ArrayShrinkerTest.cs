using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
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
