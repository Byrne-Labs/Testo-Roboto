using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class HawkAuthenticationViewModel : AuthenticationViewModel
    {
        public string Algorithm { get; set; }

        public IEnumerable<string> Algorithms { get; } = new[] { "SHA256", "SHA1" };

        public string ApplicationId { get; set; }

        public string AuthenticationId { get; set; }

        public string AuthenticationKey { get; set; }

        public string Delegation { get; set; }

        public string ExtraData { get; set; }

        public override string Name => "Hawk";

        public string Nonce { get; set; }

        public string Timestamp { get; set; }

        public string User { get; set; }
    }
}
