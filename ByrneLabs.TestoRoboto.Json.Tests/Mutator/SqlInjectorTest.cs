using ByrneLabs.TestoRoboto.Json.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
{
    public class SqlInjectorTest : MutatorTest
    {
        [Fact]
        public void TestSqlInjector()
        {
            TestMessageCountReturned<SqlInjector>(4290);
        }
    }
}
