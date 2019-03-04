using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class Fuzzer
    {

        public abstract IEnumerable<string> Fuzz(string message);
    }
}
