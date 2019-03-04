using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class Mutator
    {
        public abstract IEnumerable<string> MutateMessage(string message);
    }
}
