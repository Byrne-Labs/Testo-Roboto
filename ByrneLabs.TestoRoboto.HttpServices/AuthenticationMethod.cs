using System.Net.Http.Headers;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class AuthenticationMethod : HandyObject<AuthenticationMethod>
    {
        public abstract AuthenticationHeaderValue CreateAuthenticationHeader();
    }
}
