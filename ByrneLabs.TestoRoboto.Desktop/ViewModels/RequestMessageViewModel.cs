using System.Collections.Generic;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageViewModel : ViewModelBase
    {
        public string AuthenticationType { get; set; }

        public IEnumerable<string> AuthenticationTypesToChooseFrom { get; set; }

        public object AuthenticationViewModel { get; set; }

        public string BodyType { get; set; }

        public IEnumerable<string> BodyTypes { get; set; } = new List<string>();

        public object BodyViewModel { get; set; }

        public string ContentType { get; set; }

        public IEnumerable<string> ContentTypesToChooseFrom { get; set; } = new List<string>();

        public IList<KeyValueViewModel> Headers { get; set; } = new List<KeyValueViewModel>();

        public string HttpMethod { get; set; }

        public IEnumerable<string> HttpMethodsToChooseFrom { get; set; } = new List<string>();

        public string Name { get; set; }

        public IList<KeyValueViewModel> QueryParameters { get; set; } = new List<KeyValueViewModel>();

        public string Url { get; set; }
    }
}
