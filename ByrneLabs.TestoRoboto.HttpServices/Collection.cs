﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Collection : Item, IEntity<Collection>
    {
        private const string _fuzzedMessageCollectionName = "Fuzzed Messages";

        public IList<Item> Items { get; } = new List<Item>();

        public static Collection ImportFromPostmanFile(string filePath) => ImportFromPostmanJson(File.ReadAllText(filePath));

        public static Collection ImportFromPostmanJson(string collectionJson)
        {
            var json = JObject.Parse(collectionJson);
            var collection = new Collection();
            collection.Name = json["info"]["name"].ToString();
            collection.AuthenticationMethod = GetAuthenticationMethod(json);
            if (json.ContainsKey("item") && json["item"] is JArray)
            {
                foreach (var item in LoadItems(json["item"] as JArray))
                {
                    collection.Items.Add(item);
                }
            }

            return collection;
        }

        private static AuthenticationMethod GetAuthenticationMethod(JObject postmanItem)
        {
            AuthenticationMethod authenticationMethod;
            if (!postmanItem.ContainsKey("auth"))
            {
                authenticationMethod = new NoAuthentication();
            }
            else
            {
                var authenticationType = postmanItem["auth"]["type"].ToString();
                var properties = (JArray) postmanItem["auth"][authenticationType];
                switch (authenticationType)
                {
                    case "noauth":
                        authenticationMethod = new NoAuthentication();
                        break;
                    case "awsv4":
                        var awsSignature = new AwsSignature
                        {
                            AccessKey = GetPostmanKeyValue<string>(properties, "accessKey"),
                            Region = GetPostmanKeyValue<string>(properties, "region"),
                            SessionToken = GetPostmanKeyValue<string>(properties, "sessionToken"),
                            SecretKey = GetPostmanKeyValue<string>(properties, "secretKey"),
                            ServiceName = GetPostmanKeyValue<string>(properties, "service")
                        };
                        authenticationMethod = awsSignature;
                        break;
                    case "basic":
                        var basicAuthentication = new BasicAuthentication
                        {
                            Password = GetPostmanKeyValue<string>(properties, "password"),
                            Username = GetPostmanKeyValue<string>(properties, "username")
                        };
                        authenticationMethod = basicAuthentication;
                        break;
                    case "bearer":
                        var bearerToken = new BearerToken
                        {
                            Token = GetPostmanKeyValue<string>(properties, "token")
                        };
                        authenticationMethod = bearerToken;
                        break;
                    case "digest":
                        var digestAuthentication = new DigestAuthentication
                        {
                            ClientNonce = GetPostmanKeyValue<string>(properties, "clientNonce"),
                            Nonce = GetPostmanKeyValue<string>(properties, "nonce"),
                            NonceCount = GetPostmanKeyValue<string>(properties, "nonceCount"),
                            Opaque = GetPostmanKeyValue<string>(properties, "opaque"),
                            Password = GetPostmanKeyValue<string>(properties, "password"),
                            Qop = GetPostmanKeyValue<string>(properties, "qop"),
                            Realm = GetPostmanKeyValue<string>(properties, "realm"),
                            Username = GetPostmanKeyValue<string>(properties, "username")
                        };
                        var algorithm = GetPostmanKeyValue<string>(properties, "algorithm");
                        switch (algorithm)
                        {
                            case "MD5":
                                digestAuthentication.Algorithm = DigestAuthenticationAlgorithm.Md5;
                                break;
                            case "MD5-sess":
                                digestAuthentication.Algorithm = DigestAuthenticationAlgorithm.Md5Sess;
                                break;
                            default:
                                throw new ArgumentException($"Unexpected algorithm value {algorithm}");
                        }

                        authenticationMethod = digestAuthentication;
                        break;
                    case "hawk":
                        var hawk = new HawkAuthentication();
                        hawk.ApplicationId = GetPostmanKeyValue<string>(properties, "appId");
                        hawk.Delegation = GetPostmanKeyValue<string>(properties, "delegation");
                        hawk.ExtraData = GetPostmanKeyValue<string>(properties, "extraData");
                        hawk.AuthenticationId = GetPostmanKeyValue<string>(properties, "authId");
                        hawk.AuthenticationKey = GetPostmanKeyValue<string>(properties, "authKey");
                        hawk.Nonce = GetPostmanKeyValue<string>(properties, "nonce");
                        hawk.Timestamp = GetPostmanKeyValue<string>(properties, "timestamp");
                        hawk.User = GetPostmanKeyValue<string>(properties, "user");

                        var hawkAlgorithm = GetPostmanKeyValue<string>(properties, "algorithm");
                        switch (hawkAlgorithm)
                        {
                            case "sha1":
                                hawk.Algorithm = HawkAuthenticationAlgorithm.Sha1;
                                break;
                            case "sha256":
                                hawk.Algorithm = HawkAuthenticationAlgorithm.Sha256;
                                break;
                            default:
                                throw new ArgumentException($"Unexpected algorithm {hawkAlgorithm}");
                        }

                        authenticationMethod = hawk;
                        break;
                    case "ntlm":
                        var ntlmAuthentication = new NtlmAuthentication
                        {
                            Domain = GetPostmanKeyValue<string>(properties, "domain"),
                            Password = GetPostmanKeyValue<string>(properties, "password"),
                            Username = GetPostmanKeyValue<string>(properties, "username"),
                            Workstation = GetPostmanKeyValue<string>(properties, "workstation")
                        };
                        authenticationMethod = ntlmAuthentication;
                        break;
                    case "oauth1":
                        var oath1 = new OAuth1();
                        oath1.AccessToken = GetPostmanKeyValue<string>(properties, "token");
                        oath1.AddEmptyParametersToSignature = GetPostmanKeyValue<bool?>(properties, "addEmptyParamsToSign").GetValueOrDefault(false);
                        oath1.ConsumerKey = GetPostmanKeyValue<string>(properties, "consumerKey");
                        oath1.ConsumerSecret = GetPostmanKeyValue<string>(properties, "consumerSecret");
                        oath1.Nonce = GetPostmanKeyValue<string>(properties, "nonce");
                        oath1.Realm = GetPostmanKeyValue<string>(properties, "realm");
                        oath1.Timestamp = GetPostmanKeyValue<string>(properties, "timestamp");
                        oath1.TokenLocation = GetPostmanKeyValue<bool?>(properties, "addParamsToHeader").GetValueOrDefault(false) ? OAuth1TokenLocation.Headers : OAuth1TokenLocation.BodyAndUrl;
                        oath1.TokenSecret = GetPostmanKeyValue<string>(properties, "tokenSecret");
                        oath1.Version = GetPostmanKeyValue<string>(properties, "version");
                        var signatureMethod = GetPostmanKeyValue<string>(properties, "signatureMethod");
                        switch (signatureMethod)
                        {
                            case "HMAC-SHA1":
                                oath1.SignatureMethod = OAuth1SignatureMethod.HmacSha1;
                                break;
                            case "HMAC-SHA256":
                                oath1.SignatureMethod = OAuth1SignatureMethod.HmacSha256;
                                break;
                            case "PLAINTEXT":
                                oath1.SignatureMethod = OAuth1SignatureMethod.PlainText;
                                break;
                            default:
                                throw new ArgumentException($"Unexpected signature method {signatureMethod}");
                        }

                        authenticationMethod = oath1;
                        break;
                    case "oauth2":
                        var oath2 = new OAuth2();
                        oath2.AccessToken = GetPostmanKeyValue<string>(properties, "accessToken");
                        var tokenLocation = GetPostmanKeyValue<string>(properties, "addTokenTo");
                        switch (tokenLocation)
                        {
                            case "header":
                                oath2.TokenLocation = OAuth2TokenLocation.Headers;
                                break;
                            case "queryParams":
                                oath2.TokenLocation = OAuth2TokenLocation.QueryParameters;
                                break;
                            default:
                                throw new ArgumentException($"Unexpected token location {tokenLocation}");
                        }

                        authenticationMethod = oath2;
                        break;
                    default:
                        throw new ArgumentException($"Unexpected authentication type {authenticationType}");
                }
            }

            return authenticationMethod;
        }

        private static T GetPostmanKeyValue<T>(JArray properties, string key) => (T) ((JValue) properties.SingleOrDefault(property => property["key"].ToString() == key)?["value"])?.Value;

        private static Collection LoadCollection(JObject jsonCollection)
        {
            var collection = new Collection();
            collection.Name = jsonCollection["name"].ToString();
            collection.AuthenticationMethod = GetAuthenticationMethod(jsonCollection);
            foreach (var item in LoadItems(jsonCollection["item"] as JArray))
            {
                collection.Items.Add(item);
            }

            return collection;
        }

        private static IEnumerable<Item> LoadItems(JArray jsonItems)
        {
            var items = new List<Item>();
            foreach (var jsonItem in jsonItems.OfType<JObject>())
            {
                if (jsonItem.ContainsKey("item") && jsonItem["item"] is JArray)
                {
                    items.Add(LoadCollection(jsonItem));
                }

                if (jsonItem.ContainsKey("request") && jsonItem["request"] is JObject)
                {
                    items.Add(LoadRequestItem(jsonItem));
                }
            }

            return items;
        }

        private static RequestMessage LoadRequestItem(JObject jsonRequest)
        {
            var request = new RequestMessage();
            request.Name = jsonRequest["name"].ToString();
            request.AuthenticationMethod = GetAuthenticationMethod(jsonRequest);
            var httpMethod = jsonRequest["request"]["method"].ToString();
            switch (httpMethod)
            {
                case "GET":
                    request.HttpMethod = HttpMethod.Get;
                    break;
                case "POST":
                    request.HttpMethod = HttpMethod.Post;
                    break;
                case "PUT":
                    request.HttpMethod = HttpMethod.Put;
                    break;
                case "DELETE":
                    request.HttpMethod = HttpMethod.Delete;
                    break;
                case "HEAD":
                    request.HttpMethod = HttpMethod.Head;
                    break;
                case "OPTIONS":
                    request.HttpMethod = HttpMethod.Options;
                    break;
                case "TRACE":
                    request.HttpMethod = HttpMethod.Trace;
                    break;
                default:
                    request.HttpMethod = new HttpMethod(httpMethod);
                    break;
            }

            if (jsonRequest["request"]["header"] is JArray)
            {
                foreach (var jsonHeader in ((JArray) jsonRequest["request"]["header"]).OfType<JObject>())
                {
                    var header = new Header();
                    header.Key = jsonHeader["key"].ToString();
                    header.Value = jsonHeader["value"].ToString();
                    header.Description = jsonHeader["description"].ToString();
                    request.Headers.Add(header);
                }
            }

            var bodyType = jsonRequest["request"]["body"]["mode"].ToString();
            switch (bodyType)
            {
                case "formdata":
                    var formDataBody = new FormDataBody();
                    foreach (var jsonParameter in ((JArray) jsonRequest["request"]["body"]["formdata"]).OfType<JObject>())
                    {
                        var parameter = new KeyValue();
                        parameter.Key = jsonParameter["key"].ToString();
                        parameter.Value = jsonParameter["value"].ToString();
                        parameter.Description = jsonParameter["description"].ToString();
                        formDataBody.FormData.Add(parameter);
                    }

                    request.Body = formDataBody;
                    break;
                case "raw":
                    var bodyContent = jsonRequest["request"]["body"]["raw"].ToString();
                    if (bodyContent == string.Empty)
                    {
                        request.Body = new NoBody();
                    }
                    else
                    {
                        request.Body = new RawBody { Text = bodyContent };
                    }

                    break;
                case "urlencoded":
                    var urlEncodedBody = new UrlEncodedBody();
                    foreach (var jsonParameter in ((JArray) jsonRequest["request"]["body"]["urlencoded"]).OfType<JObject>())
                    {
                        var parameter = new KeyValue();
                        parameter.Key = jsonParameter["key"].ToString();
                        parameter.Value = jsonParameter["value"].ToString();
                        parameter.Description = jsonParameter["description"].ToString();
                        urlEncodedBody.FormData.Add(parameter);
                    }

                    request.Body = urlEncodedBody;
                    break;
                default:
                    throw new ArgumentException($"Unexpected body type {bodyType}");
            }

            request.Uri = new Uri(jsonRequest["request"]["url"]["raw"].ToString());
            request.QueryStringParameters.Clear();
            if (jsonRequest["request"]["url"]["query"] != null)
            {
                foreach (var jsonQueryParameter in (JArray) jsonRequest["request"]["url"]["query"])
                {
                    var queryStringParameter = new QueryStringParameter();
                    queryStringParameter.Key = jsonQueryParameter["key"].ToString();
                    queryStringParameter.Value = jsonQueryParameter["value"].ToString();
                    queryStringParameter.Description = jsonQueryParameter["description"].ToString();
                    request.QueryStringParameters.Add(queryStringParameter);
                }
            }

            return request;
        }

        public void AddFuzzedMessages(IEnumerable<Mutator> mutators, bool includeSubCollections)
        {
            if (Items.OfType<RequestMessage>().Any())
            {
                Collection fuzzedMessages;
                if (!Items.OfType<Collection>().Any(collection => collection.Name == _fuzzedMessageCollectionName))
                {
                    Items.Add(fuzzedMessages = new Collection { Description = "Fuzzed messages", Name = _fuzzedMessageCollectionName });
                }
                else
                {
                    fuzzedMessages = Items.OfType<Collection>().Single(collection => collection.Name == _fuzzedMessageCollectionName);
                }

                foreach (var nonFuzzedMessage in Items.OfType<RequestMessage>().Where(message => !message.FuzzedMessage))
                {
                    if (nonFuzzedMessage.Body is RawBody)
                    {
                        if (!string.IsNullOrWhiteSpace(((RawBody) nonFuzzedMessage.Body).Text))
                        {
                            foreach (var mutator in mutators)
                            {
                                foreach (var mutatedMessageContent in mutator.MutateMessage(((RawBody) nonFuzzedMessage.Body).Text))
                                {
                                    var fuzzedMessage = nonFuzzedMessage.Clone();
                                    fuzzedMessage.FuzzedMessage = true;
                                    fuzzedMessage.ExpectedStatusCode = null;
                                    fuzzedMessage.Body = new RawBody { Text = mutatedMessageContent };
                                    fuzzedMessages.Items.Add(fuzzedMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("Only raw body is currently supported for fuzzing");
                    }
                }

                if (includeSubCollections)
                {
                    foreach (var subcollection in Items.OfType<Collection>().Where(c => c.Name != _fuzzedMessageCollectionName))
                    {
                        subcollection.AddFuzzedMessages(mutators, true);
                    }
                }
            }
        }

        public new Collection Clone(CloneDepth depth = CloneDepth.Deep) => (Collection) base.Clone(depth);

        public IEnumerable<RequestMessage> DescendentRequestMessages() => Items.OfType<RequestMessage>().Union(Items.OfType<Collection>().SelectMany(collection => collection.DescendentRequestMessages()));

        public void ExportToPostman(FileInfo file) => throw new NotImplementedException();
    }
}
