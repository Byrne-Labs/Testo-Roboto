using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageViewModel : ViewModelBase
    {
        private Uri _url;

        public RequestMessageViewModel()
        {
            AddQueryStringParameterCommand = new RelayCommand(param => AddQueryStringParameter());
            DeleteSelectedQueryStringParameterCommand = new RelayCommand(param => DeleteSelectedQueryStringParameter(), param => CanDeleteSelectedQueryStringParameter());
            AddHeaderCommand = new RelayCommand(param => AddHeader());
            DeleteSelectedHeaderCommand = new RelayCommand(param => DeleteSelectedHeader(), param => CanDeleteSelectedHeader());
            AddCookieCommand = new RelayCommand(param => AddCookie());
            DeleteSelectedCookieCommand = new RelayCommand(param => DeleteSelectedCookie(), param => CanDeleteSelectedCookie());
            QueryStringParameters.CollectionChanged += (sender, args) =>
            {
                if (_url != null)
                {
                    var uriBuilder = new StringBuilder(Url.ToString().Contains("?") ? Url.ToString().SubstringBeforeLast("?") : Url.ToString()).Append('?');
                    foreach (var queryStringParameter in QueryStringParameters)
                    {
                        uriBuilder.Append(queryStringParameter.Key).Append('=').Append(queryStringParameter.UriEncodedValue).Append('&');
                    }

                    uriBuilder.Remove(uriBuilder.Length - 1, 1);

                    _url = new Uri(uriBuilder.Length <= 2083 ? uriBuilder.ToString() : uriBuilder.ToString().Substring(0, 2083));
                }
            };
        }

        public RelayCommand AddCookieCommand { get; }

        public RelayCommand AddHeaderCommand { get; }

        public RelayCommand AddQueryStringParameterCommand { get; }

        public IEnumerable<string> AuthenticationTypesToChooseFrom { get; } = new[] { "AWS Signature", "Basic", "Bearer Token", "Digest", "Hawk", "NTLM", "OAUTH v1", "OAUTH v2" };

        public object AuthenticationViewModel { get; set; }

        public string BodyType { get; set; }

        public IEnumerable<string> BodyTypes { get; } = new List<string>();

        public object BodyViewModel { get; set; }

        public string ContentType { get; set; }

        public IEnumerable<string> ContentTypesToChooseFrom { get; } = new List<string>();

        public ObservableCollection<CookieViewModel> Cookies { get; } = new ObservableCollection<CookieViewModel>();

        public RelayCommand DeleteSelectedCookieCommand { get; }

        public RelayCommand DeleteSelectedHeaderCommand { get; }

        public RelayCommand DeleteSelectedQueryStringParameterCommand { get; }

        public ObservableCollection<HeaderViewModel> Headers { get; } = new ObservableCollection<HeaderViewModel>();

        public string HttpMethod { get; set; }

        public IEnumerable<string> HttpMethodsToChooseFrom { get; } = new List<string>();

        public string Name { get; set; }

        public ObservableCollection<QueryStringParameterViewModel> QueryStringParameters { get; } = new ObservableCollection<QueryStringParameterViewModel>();

        public string SelectedAuthenticationType { get; set; }

        public CookieViewModel SelectedCookie { get; set; }

        public HeaderViewModel SelectedHeader { get; set; }

        public QueryStringParameterViewModel SelectedQueryStringParameter { get; set; }

        public Uri Url
        {
            get => _url;
            set
            {
                var queryStringParameters = new List<QueryStringParameterViewModel>();
                _url = value;
                if (_url.Query != string.Empty)
                {
                    foreach (var parameter in _url.Query.SubstringAfterLast("?").Split('&').Where(p => p != string.Empty))
                    {
                        var queryStringParameter = new QueryStringParameterViewModel();
                        queryStringParameter.Key = parameter.SubstringBeforeLast("=");
                        queryStringParameter.Value = parameter.SubstringAfterLast("=");
                        queryStringParameters.Add(queryStringParameter);
                    }
                }

                foreach (var newQueryStringParameter in queryStringParameters.Where(qsp => !QueryStringParameters.Any(qsp2 => qsp.Key == qsp2.Key && qsp.Value == qsp2.Value)))
                {
                    if (QueryStringParameters.Count(qsp => qsp.Key == newQueryStringParameter.Key) == 1)
                    {
                        QueryStringParameters.Single(qsp => qsp.Key == newQueryStringParameter.Key).Value = newQueryStringParameter.Value;
                    }
                    else
                    {
                        QueryStringParameters.Add(newQueryStringParameter);
                    }
                }

                var queryStringParametersToRemove = new List<QueryStringParameterViewModel>();
                foreach (var existingQueryStringParameter in QueryStringParameters)
                {
                    if (!queryStringParameters.Exists(qsp => qsp.Key == existingQueryStringParameter.Key && qsp.Value == existingQueryStringParameter.Value))
                    {
                        queryStringParametersToRemove.Add(existingQueryStringParameter);
                    }
                }

                foreach (var queryStringParameterToRemove in queryStringParametersToRemove)
                {
                    QueryStringParameters.Remove(queryStringParameterToRemove);
                }
            }
        }

        public void AddCookie()
        {
            Cookies.Add(new CookieViewModel());
        }

        public void AddHeader()
        {
            Headers.Add(new HeaderViewModel());
        }

        public void AddQueryStringParameter()
        {
            QueryStringParameters.Add(new QueryStringParameterViewModel());
        }

        public bool CanDeleteSelectedCookie() => SelectedCookie != null;

        public bool CanDeleteSelectedHeader() => SelectedHeader != null;

        public bool CanDeleteSelectedQueryStringParameter() => SelectedQueryStringParameter != null;

        public void DeleteSelectedCookie()
        {
            Cookies.Remove(SelectedCookie);
        }

        public void DeleteSelectedHeader()
        {
            Headers.Remove(SelectedHeader);
        }

        public void DeleteSelectedQueryStringParameter()
        {
            QueryStringParameters.Remove(SelectedQueryStringParameter);
        }
    }
}
