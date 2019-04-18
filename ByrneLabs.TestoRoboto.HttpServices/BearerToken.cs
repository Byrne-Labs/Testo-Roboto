using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class BearerToken : AuthenticationMethod, ICloneable<BearerToken>
    {
        [Key(0)]
        public string Token { get; set; }

        public new BearerToken Clone(CloneDepth depth = CloneDepth.Deep) => (BearerToken) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
