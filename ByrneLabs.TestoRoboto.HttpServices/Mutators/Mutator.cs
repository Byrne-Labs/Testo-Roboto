using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    public abstract class Mutator
    {
        public abstract IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage);
    }
}
