using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
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
