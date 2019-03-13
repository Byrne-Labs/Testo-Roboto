using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class DigestAuthenticationViewModel
    {
        public string Algorithm { get; set; }

        public IEnumerable<string> Algorithms { get; set; }

        public string ClientNonce { get; set; }

        public string Nonce { get; set; }

        public string NonceCount { get; set; }

        public string Opaque { get; set; }

        public string Password { get; set; }

        public string Qop { get; set; }

        public string Realm { get; set; }

        public string Username { get; set; }
    }
}
