using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class Oauth1AuthenticationViewModel : AuthenticationViewModel
    {
        public string AccessToken { get; set; }

        public bool AddEmptyParametersToSignature { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public override string Name => "OAuth V1";

        public string Nonce { get; set; }

        public string Realm { get; set; }

        public string SignatureMethod { get; set; }

        public IEnumerable<string> SignatureMethods { get; } = new[] { "HMAC-SHA1", "HMAC-SHA256", "Plain Text" };

        public string Timestamp { get; set; }

        public string TokenLocation { get; set; } = "Headers";

        public IEnumerable<string> TokenLocations { get; } = new[] { "Headers", "Body/URL" };

        public string TokenSecret { get; set; }

        public string Version { get; set; }
    }
}
