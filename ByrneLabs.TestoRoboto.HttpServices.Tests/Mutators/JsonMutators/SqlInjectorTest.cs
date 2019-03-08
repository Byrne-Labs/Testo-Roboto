using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
{
    public class XmlInjectorTest : MutatorTest
    {
        [Fact]
        public void TestXmlInjector()
        {
            TestMessageCountReturned<XmlInjector>(676);
        }
    }
}
