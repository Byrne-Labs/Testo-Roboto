using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class Oath1AuthenticationViewModel
    {
        public string AccessToken { get; set; }

        public bool AddEmptyParametersToSignature { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string Nonce { get; set; }

        public string Realm { get; set; }

        public string SignatureMethod { get; set; }

        public IEnumerable<string> SignatureMethods { get; set; }

        public string Timestamp { get; set; }

        public string TokenLocation { get; set; }

        public IEnumerable<string> TokenLocations { get; set; }

        public string TokenSecret { get; set; }

        public string Version { get; set; }
    }
}
