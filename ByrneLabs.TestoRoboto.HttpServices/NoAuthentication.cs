using System.Net.Http.Headers;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class NoAuthentication : AuthenticationMethod, IEntity<NoAuthentication>
    {
        public new NoAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (NoAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => null;
    }
}
