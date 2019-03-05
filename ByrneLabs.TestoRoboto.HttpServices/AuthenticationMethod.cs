using System.Net.Http.Headers;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class AuthenticationMethod : Entity
    {
        public abstract AuthenticationHeaderValue CreateAuthenticationHeader();
    }
}
