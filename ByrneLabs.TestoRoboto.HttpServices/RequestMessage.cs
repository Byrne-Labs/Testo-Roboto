﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using ByrneLabs.Commons;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class RequestMessage : RequestMessageHierarchyItem, ICloneable<RequestMessage>
    {
        [Key(3)]
        private ObservableCollection<QueryStringParameter> _queryStringParameters;
        [Key(4)]
        private Uri _uri;

        public RequestMessage()
        {
            InitializeQueryStringParameters(Enumerable.Empty<QueryStringParameter>());
        }

        [Key(5)]
        public Body Body { get; set; }

        [Key(6)]
        public IList<Cookie> Cookies { get; } = new List<Cookie>();

        [Key(7)]
        public string Encoding { get; set; } = System.Text.Encoding.UTF8.WebName;

        [Key(8)]
        public HttpStatusCode? ExpectedStatusCode { get; set; }

        [IgnoreMember]
        public string Fingerprint
        {
            get
            {
                var fingerprint = new StringBuilder();
                if (HttpMethod != null)
                {
                    fingerprint.Append("method: ").AppendLine(HttpMethod);
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

        [Key(9)]
        public IList<Header> Headers { get; } = new List<Header>();

        [Key(10)]
        public string HttpMethod { get; set; }

        [IgnoreMember]
        public bool IsBodyContentJson
        {
            get
            {
                return Body is RawBody && Headers.Any(header => header.Key == "Content-Type" && header.Value.StartsWith("application/json", StringComparison.Ordinal));
            }
        }

        [IgnoreMember]
        public bool IsBodyContentXml
        {
            get
            {
                return Body is RawBody && Headers.Any(header => header.Key == "Content-Type" && (header.Value.StartsWith("application/xml", StringComparison.Ordinal) || header.Value.StartsWith("text/xml", StringComparison.Ordinal)));
            }
        }

        [IgnoreMember]
        public IList<QueryStringParameter> QueryStringParameters => _queryStringParameters;

        [Key(11)]
        public List<ResponseMessage> ResponseMessages { get; } = new List<ResponseMessage>();

        [IgnoreMember]
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
        private static void ClearJsonValues(JObject json)
        {
            foreach (var child in json.Children().OfType<JProperty>())
            {
                switch (child.Value)
                {
                    case JObject jObject:
                        ClearJsonValues(jObject);
                        break;
                    case JArray jArray:
                        foreach (var item in jArray.OfType<JObject>())
                        {
                            ClearJsonValues(item);
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

            ClearJsonValues(json);

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
