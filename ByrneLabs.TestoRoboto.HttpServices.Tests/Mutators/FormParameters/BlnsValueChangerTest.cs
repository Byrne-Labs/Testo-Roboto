using ByrneLabs.TestoRoboto.HttpServices.Mutators.FormParameters;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.FormParameters
{
    public class BlnsValueChangerTest : MutatorTestBase
    {
        [Fact]
        public void TestBlnsValueChanger()
        {
            TestFormMessageCountReturned<BlnsValueChanger>(1008);
        }
    }
}
