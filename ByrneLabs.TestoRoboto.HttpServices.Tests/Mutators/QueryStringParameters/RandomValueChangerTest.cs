using ByrneLabs.TestoRoboto.HttpServices.Mutators.QueryStringParameters;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.QueryStringParameters
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
