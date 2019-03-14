using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class JavaScriptInjectorTest : MutatorTestBase
    {
        [Fact]
        public void TestJavaScriptInjector()
        {
            TestJsonMessageCountReturned<JavaScriptInjector>(4770);
        }
    }
}
