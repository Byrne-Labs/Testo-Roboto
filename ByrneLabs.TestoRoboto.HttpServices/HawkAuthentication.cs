using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class HawkAuthentication : AuthenticationMethod, IEntity<HawkAuthentication>
    {
        public HawkAuthenticationAlgorithm Algorithm { get; set; }

        public string App { get; set; }

        public string Dlg { get; set; }

        public string Ext { get; set; }

        public string HawkAuthenticationId { get; set; }

        public string HawkAuthenticationKey { get; set; }

        public string Nonce { get; set; }

        public string Timestamp { get; set; }

        public string User { get; set; }

        public new HawkAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (HawkAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
