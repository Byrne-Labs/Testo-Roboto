using ByrneLabs.TestoRoboto.HttpServices.Mutators.QueryStringParameters;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.QueryStringParameters
{
    public class BlnsValueChangerTest : MutatorTestBase
    {
        [Fact]
        public void TestBlnsValueChanger()
        {
            TestJsonMessageCountReturned<BlnsValueChanger>(1008);
        }
    }
}
