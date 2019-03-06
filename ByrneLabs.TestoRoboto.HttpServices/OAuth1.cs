using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class OAuth1 : AuthenticationMethod, IEntity<OAuth1>
    {
        public string AccessToken { get; set; }

        public bool AddEmptyParametersToSignature { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string Nonce { get; set; }

        public string Realm { get; set; }

        public OAuth1SignatureMethod SignatureMethod { get; set; }

        public string Timestamp { get; set; }

        public OAuth1TokenLocation TokenLocation { get; set; }

        public string TokenSecret { get; set; }

        public string Version { get; set; }

        public new OAuth1 Clone(CloneDepth depth = CloneDepth.Deep) => (OAuth1) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
