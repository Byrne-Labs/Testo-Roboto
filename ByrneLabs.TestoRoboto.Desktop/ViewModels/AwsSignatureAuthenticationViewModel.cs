namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class AwsSignatureAuthenticationViewModel : AuthenticationViewModel
    {
        public string AccessKey { get; set; }

        public override string Name => "AWS Signature";

        public string Region { get; set; }

        public string SecretKey { get; set; }

        public string ServiceName { get; set; }

        public string SessionToken { get; set; }
    }
}
