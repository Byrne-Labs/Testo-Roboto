using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    public class OAuth1 : AuthenticationMethod, ICloneable<OAuth1>
    {
        [Key(0)]
        public string AccessToken { get; set; }

        [Key(1)]
        public bool AddEmptyParametersToSignature { get; set; }

        [Key(2)]
        public string ConsumerKey { get; set; }

        [Key(3)]
        public string ConsumerSecret { get; set; }

        [Key(4)]
        public string Nonce { get; set; }

        [Key(5)]
        public string Realm { get; set; }

        [Key(6)]
        public OAuth1SignatureMethod SignatureMethod { get; set; }

        [Key(7)]
        public string Timestamp { get; set; }

        [Key(8)]
        public OAuth1TokenLocation TokenLocation { get; set; }

        [Key(9)]
        public string TokenSecret { get; set; }

        [Key(10)]
        public string Version { get; set; }

        public new OAuth1 Clone(CloneDepth depth = CloneDepth.Deep) => (OAuth1) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
