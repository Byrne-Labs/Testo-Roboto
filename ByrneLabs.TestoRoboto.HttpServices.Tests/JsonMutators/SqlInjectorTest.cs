using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
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
