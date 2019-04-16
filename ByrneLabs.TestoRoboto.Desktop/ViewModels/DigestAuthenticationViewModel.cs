using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class DigestAuthenticationViewModel: AuthenticationViewModel
    {
        public string Algorithm { get; set; }

        public IEnumerable<string> Algorithms { get; } = new[] { "MD5", "MD5-sess" };

        public string ClientNonce { get; set; }

        public string Nonce { get; set; }

        public string NonceCount { get; set; }

        public string Opaque { get; set; }

        public string Password { get; set; }

        public string QualityOfProtection { get; set; }

        public string Realm { get; set; }

        public string Username { get; set; }

        public override string Name => "Digest";
    }
}
