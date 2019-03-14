using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class RandomValueChangerTest : MutatorTestBase
    {
        [Fact]
        public void TestRandomValueChanger()
        {
            TestJsonMessageCountReturned<RandomValueChanger>(806);
        }
    }
}
