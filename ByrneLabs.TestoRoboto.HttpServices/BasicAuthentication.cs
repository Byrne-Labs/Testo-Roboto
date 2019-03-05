using System;
using System.Net.Http.Headers;
using System.Text;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class BasicAuthentication : AuthenticationMethod, IEntity<BasicAuthentication>
    {
        public string Password { get; set; }

        public string Username { get; set; }

        public new BasicAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (BasicAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader()
        {
            var byteArray = Encoding.ASCII.GetBytes(Username + ":" + Password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
    }
}
