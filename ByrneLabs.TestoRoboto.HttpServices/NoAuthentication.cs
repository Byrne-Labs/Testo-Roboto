using System.Net.Http.Headers;
using ByrneLabs.Commons;
//using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class NoAuthentication : AuthenticationMethod, ICloneable<NoAuthentication>
    {
        public new NoAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (NoAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => null;
    }
}
