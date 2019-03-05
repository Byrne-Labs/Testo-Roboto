using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
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
