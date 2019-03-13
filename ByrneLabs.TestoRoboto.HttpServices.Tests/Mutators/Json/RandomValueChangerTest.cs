using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
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
