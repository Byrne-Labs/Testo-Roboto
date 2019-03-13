using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class JavaScriptInjectorTest : MutatorTest
    {
        [Fact]
        public void TestJavaScriptInjector()
        {
            TestMessageCountReturned<JavaScriptInjector>(4770);
        }
    }
}
