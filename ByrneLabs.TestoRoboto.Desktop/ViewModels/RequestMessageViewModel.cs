using System.Collections.Generic;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageViewModel : ViewModelBase
    {
        public string AuthenticationType { get; set; }

        public IEnumerable<string> AuthenticationTypes { get; set; }

        public object AuthenticationViewModel { get; set; }

        public string BodyType { get; set; }

        public IEnumerable<string> BodyTypes { get; set; }

        public object BodyViewModel { get; set; }

        public string ContentType { get; set; }

        public IEnumerable<string> ContentTypes { get; set; }

        public IList<KeyValueViewModel> Headers { get; set; }

        public string HttpMethod { get; set; }

        public IEnumerable<string> HttpMethods { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
