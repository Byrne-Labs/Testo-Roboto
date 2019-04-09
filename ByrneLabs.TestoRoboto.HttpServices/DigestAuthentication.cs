using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    public class DigestAuthentication : AuthenticationMethod, ICloneable<DigestAuthentication>
    {
        [Key(0)]
        public DigestAuthenticationAlgorithm Algorithm { get; set; }

        [Key(1)]
        public string ClientNonce { get; set; }

        [Key(2)]
        public string Nonce { get; set; }

        [Key(3)]
        public string NonceCount { get; set; }

        [Key(4)]
        public string Opaque { get; set; }

        [Key(5)]
        public string Password { get; set; }

        [Key(6)]
        public string Qop { get; set; }

        [Key(7)]
        public string Realm { get; set; }

        [Key(8)]
        public string Username { get; set; }

        public new DigestAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (DigestAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
