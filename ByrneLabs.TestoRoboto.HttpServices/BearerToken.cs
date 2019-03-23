using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class BearerToken : AuthenticationMethod, ICloneable<BearerToken>
    {
        public string Token { get; set; }

        public new BearerToken Clone(CloneDepth depth = CloneDepth.Deep) => (BearerToken) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
