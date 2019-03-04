using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
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
