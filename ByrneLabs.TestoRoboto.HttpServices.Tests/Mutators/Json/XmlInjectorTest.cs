using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
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
