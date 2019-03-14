using ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Headers
{
    public class RandomValueChangerTest : MutatorTestBase
    {
        [Fact]
        public void TestRandomValueChanger()
        {
            TestJsonMessageCountReturned<RandomValueChanger>(124);
        }
    }
}
