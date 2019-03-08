using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class RequestMessage : Item, IEntity<RequestMessage>
    {
        private ObservableCollection<QueryStringParameter> _queryStringParameters;
        private Uri _uri;

        public RequestMessage()
        {
            InitializeQueryStringParameters(Enumerable.Empty<QueryStringParameter>());
        }

        public Body Body { get; set; }

        public IList<Cookie> Cookies { get; set; } = new List<Cookie>();

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public HttpStatusCode? ExpectedStatusCode { get; set; } = HttpStatusCode.OK;

        public bool FuzzedMessage { get; set; }

        public IList<Header> Headers { get; set; } = new List<Header>();

        public HttpMethod HttpMethod { get; set; }

        public IList<QueryStringParameter> QueryStringParameters => _queryStringParameters;

        public IList<HttpResponseMessage> ResponseMessages { get; set; } = new List<HttpResponseMessage>();

        public Uri Uri
        {
            get => _uri;
            set
            {
                var queryStringParameters = new List<QueryStringParameter>();
                _uri = value;
                if (_uri.Query != string.Empty)
                {
                    foreach (var parameter in _uri.Query.SubstringAfterLast("?").Split('&').Where(p => p != string.Empty))
                    {
                        var queryStringParameter = new QueryStringParameter();
                        queryStringParameter.Key = parameter.SubstringBeforeLast("=");
                        queryStringParameter.Value = parameter.SubstringAfterLast("=");
                        queryStringParameters.Add(queryStringParameter);
                    }
                }

                InitializeQueryStringParameters(queryStringParameters);
            }
        }

        public new RequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (RequestMessage) base.Clone(depth);

        public override bool Validate() => base.Validate() && HttpMethod != null;

        private void InitializeQueryStringParameters(IEnumerable<QueryStringParameter> queryStringParameters)
        {
            _queryStringParameters = new ObservableCollection<QueryStringParameter>(queryStringParameters);
            _queryStringParameters.CollectionChanged += (sender, args) =>
            {
                if (_uri != null)
                {
                    var uriBuilder = new StringBuilder(Uri.ToString().Contains("?") ? Uri.ToString().SubstringBeforeLast("?") : Uri.ToString()).Append('?');
                    foreach (var queryStringParameter in _queryStringParameters)
                    {
                        uriBuilder.Append(queryStringParameter.Key).Append('=').Append(queryStringParameter.UriEncodedValue).Append('&');
                    }

                    uriBuilder.Remove(uriBuilder.Length - 1, 1);

                    _uri = new Uri(uriBuilder.ToString());
                }
            };
        }
    }
}
