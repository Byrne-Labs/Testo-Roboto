using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
//using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class OAuth2 : AuthenticationMethod, ICloneable<OAuth2>
    {
        [Key(0)]
        public string AccessToken { get; set; }

        [Key(1)]
        public OAuth2TokenLocation TokenLocation { get; set; }

        public new OAuth2 Clone(CloneDepth depth = CloneDepth.Deep) => (OAuth2) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
