using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class NtlmAuthentication : AuthenticationMethod, ICloneable<NtlmAuthentication>
    {
        public string Domain { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        public string Workstation { get; set; }

        public new NtlmAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (NtlmAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
