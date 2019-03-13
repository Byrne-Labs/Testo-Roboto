using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class HawkAuthenticationViewModel
    {
        public string Algorithm { get; set; }

        public IEnumerable<string> Algorithms { get; set; }

        public string ApplicationId { get; set; }

        public string AuthenticationId { get; set; }

        public string AuthenticationKey { get; set; }

        public string Delegation { get; set; }

        public string ExtraData { get; set; }

        public string Nonce { get; set; }

        public string Timestamp { get; set; }

        public string User { get; set; }
    }
}
