namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class BasicAuthenticationViewModel : AuthenticationViewModel
    {
        public override string Name => "Basic";

        public string Password { get; set; }

        public string Username { get; set; }
    }
}
