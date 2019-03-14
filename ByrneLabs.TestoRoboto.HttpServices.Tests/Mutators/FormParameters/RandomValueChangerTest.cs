using ByrneLabs.TestoRoboto.HttpServices.Mutators.FormParameters;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.FormParameters
{
    public class RandomValueChangerTest : MutatorTestBase
    {
        [Fact]
        public void TestRandomValueChanger()
        {
            TestFormMessageCountReturned<RandomValueChanger>(124);
        }
    }
}
