using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
//using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class HawkAuthentication : AuthenticationMethod, ICloneable<HawkAuthentication>
    {
        [Key(0)]
        public HawkAuthenticationAlgorithm Algorithm { get; set; }

        [Key(1)]
        public string ApplicationId { get; set; }

        [Key(2)]
        public string AuthenticationId { get; set; }

        [Key(3)]
        public string AuthenticationKey { get; set; }

        [Key(4)]
        public string Delegation { get; set; }

        [Key(5)]
        public string ExtraData { get; set; }

        [Key(6)]
        public string Nonce { get; set; }

        [Key(7)]
        public string Timestamp { get; set; }

        [Key(8)]
        public string User { get; set; }

        public new HawkAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (HawkAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
