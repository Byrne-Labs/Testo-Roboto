using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class AwsSignature : AuthenticationMethod, ICloneable<AwsSignature>
    {
        [Key(0)]
        public string AccessKey { get; set; }

        [Key(1)]
        public string Region { get; set; }

        [Key(2)]
        public string SecretKey { get; set; }

        [Key(3)]
        public string ServiceName { get; set; }

        [Key(4)]
        public string SessionToken { get; set; }

        public new AwsSignature Clone(CloneDepth depth = CloneDepth.Deep) => (AwsSignature) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
