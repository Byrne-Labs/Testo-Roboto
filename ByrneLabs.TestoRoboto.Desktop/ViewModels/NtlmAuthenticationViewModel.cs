namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class NtlmAuthenticationViewModel : AuthenticationViewModel
    {
        public string Domain { get; set; }

        public override string Name => "NTLM";

        public string Password { get; set; }

        public string Username { get; set; }

        public string Workstation { get; set; }
    }
}
