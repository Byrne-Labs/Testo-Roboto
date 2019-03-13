using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
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
