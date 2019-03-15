using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [PublicAPI]
    public class RequestMessage : Item, IEntity<RequestMessage>
    {
        private ObservableCollection<QueryStringParameter> _queryStringParameters;
        private Uri _uri;

        public RequestMessage()
        {
            InitializeQueryStringParameters(Enumerable.Empty<QueryStringParameter>());
        }

        public Body Body { get; set; }

        public IList<Cookie> Cookies { get; } = new List<Cookie>();

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public HttpStatusCode? ExpectedStatusCode { get; set; } = HttpStatusCode.OK;

        public string Fingerprint
        {
            get
            {
                var fingerprint = new StringBuilder();
                if (HttpMethod != null)
                {
                    fingerprint.Append("method: ").AppendLine(HttpMethod.ToString());
                }

                if (Uri != null)
                {
                    fingerprint.Append("URL: ").Append(Uri.Scheme).Append(Uri.SchemeDelimiter).Append(Uri.Host).AppendLine(Uri.AbsolutePath);
                }

                if (QueryStringParameters.Any())
                {
                    fingerprint.Append("query string parameters: ").AppendLine(string.Join(", ", QueryStringParameters.Select(queryStringParameter => queryStringParameter.Key)));
                }

                if (Headers.Any())
                {
                    fingerprint.Append("headers: ").AppendLine(string.Join(", ", Headers.Select(header => header.Key)));
                }

                if (Cookies.Any())
                {
                    fingerprint.Append("cookies: ").AppendLine(string.Join(", ", Cookies.Select(cookie => cookie.Name)));
                }

                if (IsBodyContentJson)
                {
                    fingerprint.Append("body: ").AppendLine(GetJsonFingerprint(((RawBody) Body).Text));
                }
                else if (!(Body is NoBody))
                {
                    fingerprint.Append("body: ").AppendLine(Body.Fingerprint);
                }

                return fingerprint.ToString();
            }
        }

        public IList<Header> Headers { get; } = new List<Header>();

        public HttpMethod HttpMethod { get; set; }

        public bool IsBodyContentJson
        {
            get
            {
                return Body is RawBody && Headers.Any(header => header.Key == "Content-Type" && header.Value.StartsWith("application/json", StringComparison.Ordinal));
            }
        }

        public bool IsBodyContentXml
        {
            get
            {
                return Body is RawBody && Headers.Any(header => header.Key == "Content-Type" && (header.Value.StartsWith("application/xml", StringComparison.Ordinal) || header.Value.StartsWith("text/xml", StringComparison.Ordinal)));
            }
        }

        public IList<QueryStringParameter> QueryStringParameters => _queryStringParameters;

        public IList<ResponseMessage> ResponseMessages { get; } = new List<ResponseMessage>();

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

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void ClearValues(JObject json)
        {
            foreach (var child in json.Children().OfType<JProperty>())
            {
                switch (child.Value)
                {
                    case JObject jObject:
                        ClearValues(jObject);
                        break;
                    case JArray jArray:
                        foreach (var item in jArray.OfType<JObject>())
                        {
                            ClearValues(item);
                        }

                        var itemStrings = new List<string>();
                        var itemsToRemove = new List<JObject>();

                        foreach (var item in jArray.OfType<JObject>())
                        {
                            var itemString = item.ToString(Formatting.None);
                            if (itemStrings.Contains(itemString))
                            {
                                itemsToRemove.Add(item);
                            }
                            else
                            {
                                itemStrings.Add(itemString);
                            }
                        }

                        foreach (var itemToRemove in itemsToRemove)
                        {
                            jArray.Remove(itemToRemove);
                        }

                        break;
                    case JValue jValue:
                        jValue.Value = null;
                        break;
                    default:
                        throw new ArgumentException($"Unexpected type {child.GetType()}");
                }
            }
        }

        private static string GetJsonFingerprint(string jsonText)
        {
            JObject json;
            try
            {
                json = JObject.Parse(jsonText);
            }
            catch
            {
                return jsonText;
            }

            ClearValues(json);

            return json.ToString(Formatting.None);
        }

        public override void AssertValid()
        {
            base.AssertValid();
            if (HttpMethod == null)
            {
                throw new InvalidOperationException($"The {nameof(HttpMethod)} cannot be null");
            }
        }

        public new RequestMessage Clone(CloneDepth depth = CloneDepth.Deep) => (RequestMessage) base.Clone(depth);

        public FuzzedRequestMessage CloneIntoFuzzedRequestMessage()
        {
            var fuzzedRequestMessage = DeepCloner.CloneInto<FuzzedRequestMessage, RequestMessage>(this);
            fuzzedRequestMessage.SourceRequestMessage = this;

            return fuzzedRequestMessage;
        }

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

                    _uri = new Uri(uriBuilder.Length <= 2083 ? uriBuilder.ToString() : uriBuilder.ToString().Substring(0, 2083));
                }
            };
        }
    }
}
