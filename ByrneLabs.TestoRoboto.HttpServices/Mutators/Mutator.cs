using System.Collections.Generic;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators
{
    [MessagePackObject]
    public abstract class Mutator
    {
        public abstract IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage);
    }
}
