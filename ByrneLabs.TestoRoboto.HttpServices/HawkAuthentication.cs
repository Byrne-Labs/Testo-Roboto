using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class HawkAuthentication : AuthenticationMethod, IEntity<HawkAuthentication>
    {
        public HawkAuthenticationAlgorithm Algorithm { get; set; }

        public string ApplicationId { get; set; }

        public string AuthenticationId { get; set; }

        public string AuthenticationKey { get; set; }

        public string Delegation { get; set; }

        public string ExtraData { get; set; }

        public string Nonce { get; set; }

        public string Timestamp { get; set; }

        public string User { get; set; }

        public new HawkAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (HawkAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
