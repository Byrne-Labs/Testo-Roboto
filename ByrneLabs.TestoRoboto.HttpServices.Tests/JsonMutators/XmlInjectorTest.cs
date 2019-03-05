using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.JsonMutators
{
    public class SqlInjectorTest : MutatorTest
    {
        [Fact]
        public void TestSqlInjector()
        {
            TestMessageCountReturned<SqlInjector>(377);
        }
    }
}
