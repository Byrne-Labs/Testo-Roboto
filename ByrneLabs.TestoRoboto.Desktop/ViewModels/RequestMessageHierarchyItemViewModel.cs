using System.Collections.Generic;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageHierarchyItemViewModel : ViewModelBase
    {
        protected RequestMessageHierarchyItemViewModel()
        {
            CloseCommand = new RelayCommand(param => OnClose());
        }

        public IEnumerable<AuthenticationViewModel> AuthenticationTypes { get; } = new AuthenticationViewModel[] { new InheritedAuthenticationViewModel(), new NoAuthenticationViewModel(), new AwsSignatureAuthenticationViewModel(), new BasicAuthenticationViewModel(), new BearerTokenAuthenticationViewModel(), new DigestAuthenticationViewModel(), new HawkAuthenticationViewModel(), new NtlmAuthenticationViewModel(), new Oauth1AuthenticationViewModel(), new Oauth2AuthenticationViewModel() };

        public AuthenticationViewModel AuthenticationViewModel { get; set; }

        public RelayCommand CloseCommand { get; }

        public string Description { get; set; }

        public bool IsClosed { get; set; }

        public bool IsSelected { get; set; }

        public string Name { get; set; }

        public RequestMessageCollectionViewModel ParentCollection { get; set; }

        public void OnClose()
        {
            IsClosed = true;
        }
    }
}
