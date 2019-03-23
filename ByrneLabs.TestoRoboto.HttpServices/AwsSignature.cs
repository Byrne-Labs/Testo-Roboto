using System;
using System.Net.Http.Headers;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class AwsSignature : AuthenticationMethod, ICloneable<AwsSignature>
    {
        public string AccessKey { get; set; }

        public string Region { get; set; }

        public string SecretKey { get; set; }

        public string ServiceName { get; set; }

        public string SessionToken { get; set; }

        public new AwsSignature Clone(CloneDepth depth = CloneDepth.Deep) => (AwsSignature) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader() => throw new NotImplementedException();
    }
}
