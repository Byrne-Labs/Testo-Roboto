namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class BearerTokenAuthenticationViewModel : AuthenticationViewModel
    {
        public override string Name => "Bearer Token";

        public string Token { get; set; }
    }
}
