using ByrneLabs.TestoRoboto.Json.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
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
