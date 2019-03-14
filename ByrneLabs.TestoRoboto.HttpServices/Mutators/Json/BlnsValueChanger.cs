using System.Collections.Generic;
using ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Json
{
    public class BlnsValueChanger : ValueChanger
    {
        protected override IEnumerable<object> TestValues => Blns.Values;
    }
}
