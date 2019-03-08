using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
{
    public class RandomValueChangerTest : MutatorTest
    {
        [Fact]
        public void TestRandomValueChanger()
        {
            TestMessageCountReturned<RandomValueChanger>(962);
        }
    }
}
