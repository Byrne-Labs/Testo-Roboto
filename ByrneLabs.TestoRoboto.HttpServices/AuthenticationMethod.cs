using System.Net.Http.Headers;
using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    [Union(0, typeof(AwsSignature))]
    [Union(1, typeof(BasicAuthentication))]
    [Union(2, typeof(BearerToken))]
    [Union(3, typeof(DigestAuthentication))]
    [Union(4, typeof(HawkAuthentication))]
    [Union(5, typeof(NoAuthentication))]
    [Union(6, typeof(NtlmAuthentication))]
    [Union(7, typeof(OAuth1))]
    [Union(8, typeof(OAuth2))]
    public abstract class AuthenticationMethod : HandyObject<AuthenticationMethod>
    {
        public abstract AuthenticationHeaderValue CreateAuthenticationHeader();
    }
}
