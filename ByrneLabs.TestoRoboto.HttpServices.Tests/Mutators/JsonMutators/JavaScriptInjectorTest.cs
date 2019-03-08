using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
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
