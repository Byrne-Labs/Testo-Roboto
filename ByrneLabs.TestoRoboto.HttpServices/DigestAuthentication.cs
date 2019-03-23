using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class DigestAuthentication : AuthenticationMethod, ICloneable<DigestAuthentication>
    {
        public DigestAuthenticationAlgorithm Algorithm { get; set; }

        public string ClientNonce { get; set; }

        public string Nonce { get; set; }

        public string NonceCount { get; set; }

        public string Opaque { get; set; }

        public string Password { get; set; }

        public string Qop { get; set; }

        public string Realm { get; set; }

        public string Username { get; set; }

        public new DigestAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (DigestAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
