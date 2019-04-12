using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
//using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class NtlmAuthentication : AuthenticationMethod, ICloneable<NtlmAuthentication>
    {
        [Key(0)]
        public string Domain { get; set; }

        [Key(1)]
        public string Password { get; set; }

        [Key(2)]
        public string Username { get; set; }

        [Key(3)]
        public string Workstation { get; set; }

        public new NtlmAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (NtlmAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
