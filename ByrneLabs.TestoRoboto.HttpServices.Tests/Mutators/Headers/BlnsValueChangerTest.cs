using ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Headers
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
