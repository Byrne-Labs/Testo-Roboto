using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Common
{
    public abstract class Fuzzer
    {
        public abstract IEnumerable<string> Fuzz(string message);
    }
}
