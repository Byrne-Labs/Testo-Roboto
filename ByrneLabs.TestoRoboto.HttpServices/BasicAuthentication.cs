using System;
using System.Net.Http.Headers;
using System.Text;
using ByrneLabs.Commons;
//using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    public class BasicAuthentication : AuthenticationMethod, ICloneable<BasicAuthentication>
    {
        [Key(0)]
        public string Password { get; set; }

        [Key(1)]
        public string Username { get; set; }

        public new BasicAuthentication Clone(CloneDepth depth = CloneDepth.Deep) => (BasicAuthentication) base.Clone(depth);

        public override AuthenticationHeaderValue CreateAuthenticationHeader()
        {
            var byteArray = Encoding.ASCII.GetBytes(Username + ":" + Password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
    }
}
