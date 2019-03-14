using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class XmlInjectorTest : MutatorTestBase
    {
        [Fact]
        public void TestXmlInjector()
        {
            TestJsonMessageCountReturned<XmlInjector>(676);
        }
    }
}
