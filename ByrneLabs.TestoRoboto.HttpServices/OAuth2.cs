using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class OAuth2 : AuthenticationMethod, IEntity<OAuth2>
    {
        public string AccessToken { get; set; }

        public OAuth2TokenLocation TokenLocation { get; set; }

        public new OAuth2 Clone(CloneDepth depth = CloneDepth.Deep) => (OAuth2) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
