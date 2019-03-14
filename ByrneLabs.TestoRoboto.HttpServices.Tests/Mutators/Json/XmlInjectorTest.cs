﻿using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.Json
{
    public class SqlInjectorTest : MutatorTestBase
    {
        [Fact]
        public void TestSqlInjector()
        {
            TestJsonMessageCountReturned<SqlInjector>(377);
        }
    }
}