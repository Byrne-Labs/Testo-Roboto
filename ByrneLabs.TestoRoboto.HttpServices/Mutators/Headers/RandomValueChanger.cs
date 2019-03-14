using System.Collections.Generic;
using ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Headers
{
    public class RandomValueChanger : ValueChanger
    {
        protected override IEnumerable<object> TestValues => RandomValues.Values;
    }
}
