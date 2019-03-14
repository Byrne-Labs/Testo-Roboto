using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class BlnsValueChangerTest : MutatorTestBase
    {
        [Fact]
        public void TestBlnsValueChanger()
        {
            TestJsonMessageCountReturned<BlnsValueChanger>(6552);
        }
    }
}
