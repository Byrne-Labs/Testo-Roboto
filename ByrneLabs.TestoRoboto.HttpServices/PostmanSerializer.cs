using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class PostmanSerializer : ICollectionSerializer
    {
        public bool BinaryOnly => false;

        private static JObject CreateAuthenticationParameter(string key, string value)
        {
            var jsonParameter = new JObject();
            jsonParameter.Add("key", key);
            jsonParameter.Add("value", value);
            jsonParameter.Add("type", "string");

            return jsonParameter;
        }

        private static JObject ExportToPostman(Item item)
        {
            var jsonItem = new JObject();
            jsonItem.Add("name", item.Name);
            if (item is Collection collection)
            {
                var jsonItems = new JArray();
                foreach (var childItem in collection.Items.OfType<Collection>())
                {
                    jsonItems.Add(ExportToPostman(childItem));
                }

                foreach (var childItem in collection.Items.OfType<RequestMessage>())
                {
                    jsonItems.Add(ExportToPostman((Item) childItem));
                }

                jsonItem.Add("item", jsonItems);

                if (item.AuthenticationMethod != null && !(item.AuthenticationMethod is NoAuthentication))
                {
                    jsonItem.Add("auth", ExportToPostman(item.AuthenticationMethod));
                }
            }
            else if (item is RequestMessage message)
            {
                jsonItem.Add("request", ExportToPostman(message));
            }

            return jsonItem;
        }

        private static JObject ExportToPostman(AuthenticationMethod authentication)
        {
            var jsonAuthentication = new JObject();
            var jsonAuthenticationParameters = new JArray();
            string authenticationTypeName;

            if (authentication is BasicAuthentication basicAuthentication)
            {
                authenticationTypeName = "basic";
                jsonAuthenticationParameters.Add(CreateAuthenticationParameter("password", basicAuthentication.Password));
                jsonAuthenticationParameters.Add(CreateAuthenticationParameter("username", basicAuthentication.Username));
            }
            else
            {
                throw new NotSupportedException("Only basic authentication is currently supported");
            }

            jsonAuthentication.Add("type", authenticationTypeName);
            jsonAuthentication.Add(authenticationTypeName, jsonAuthenticationParameters);

            return jsonAuthentication;
        }

        private static JObject ExportToPostman(RequestMessage requestMessage)
        {
            var jsonRequestMessage = new JObject();

            if (requestMessage.AuthenticationMethod != null && !(requestMessage.AuthenticationMethod is NoAuthentication))
            {
                jsonRequestMessage.Add("auth", ExportToPostman(requestMessage.AuthenticationMethod));
            }

            if (requestMessage.HttpMethod != null)
            {
                jsonRequestMessage.Add("method", requestMessage.HttpMethod.ToString());
            }

            var jsonHeaders = new JArray();
            foreach (var header in requestMessage.Headers)
            {
                var jsonHeader = new JObject
                {
                    { "key", header.Key },
                    { "name", header.Key },
                    { "value", header.Value },
                    { "description", header.Description },
                    { "type", "text" }
                };
                jsonHeaders.Add(jsonHeader);
            }

            jsonRequestMessage.Add("header", jsonHeaders);

            if (requestMessage.Body is RawBody body)
            {
                var jsonBody = new JObject
                {
                    { "mode", "raw" },
                    { "raw", body.Text }
                };
                jsonRequestMessage.Add("body", jsonBody);
            }
            else
            {
                throw new NotSupportedException("Only raw body is currently supported");
            }

            if (requestMessage.Uri != null)
            {
                var jsonUriHost = new JArray();
                foreach (var host in requestMessage.Uri.Host.Split('.'))
                {
                    jsonUriHost.Add(host);
                }

                var jsonUriPath = new JArray();
                foreach (var host in requestMessage.Uri.LocalPath.Split('/'))
                {
                    jsonUriPath.Add(host);
                }

                var jsonUriQuery = new JArray();
                foreach (var queryParameter in requestMessage.QueryStringParameters)
                {
                    var jsonQueryParameter = new JObject();
                    jsonQueryParameter.Add("key", queryParameter.Key);
                    jsonQueryParameter.Add("value", queryParameter.UriEncodedValue);
                    jsonQueryParameter.Add("description", queryParameter.Description);
                    jsonUriQuery.Add(jsonQueryParameter);
                }

                var jsonUri = new JObject
                {
                    { "raw", requestMessage.Uri.ToString() },
                    { "protocol", requestMessage.Uri.Scheme },
                    { "host", jsonUriHost },
                    { "path", jsonUriPath },
                    { "query", jsonUriQuery }
                };

                jsonRequestMessage.Add("url", jsonUri);
            }
            else
            {
                var jsonUri = new JObject
                {
                    { "raw", string.Empty }
                };

                jsonRequestMessage.Add("url", jsonUri);
            }

            return jsonRequestMessage;
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

        public Collection Read(byte[] bytes) => ReadFromString(Encoding.Unicode.GetString(bytes));

        public Collection ReadFromFile(string fileName) => ReadFromString(File.ReadAllText(fileName));

        public Collection ReadFromString(string collectionText)
        {
            var json = JObject.Parse(collectionText);
            var collection = new Collection();
            collection.EntityId = json["_postman_id"] != null ? Guid.Parse(json["_postman_id"].ToString()) : Guid.NewGuid();
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

        public byte[] Write(Collection collection) => Encoding.Unicode.GetBytes(WriteToString(collection));

        public void WriteToFile(Collection collection, string fileName) => File.WriteAllText(fileName, WriteToString(collection), Encoding.Unicode);

        public string WriteToString(Collection collection)
        {
            collection.AssertValid();

            var jsonCollection = new JObject();
            var jsonCollectionInfo = new JObject
            {
                { "_postman_id", collection.EntityId },
                { "name", collection.Name },
                { "schema", "https://schema.getpostman.com/json/collection/v2.1.0/collection.json" }
            };
            jsonCollection.Add("info", jsonCollectionInfo);

            var jsonCollectionItems = new JArray();
            foreach (var childItem in collection.Items.OfType<Collection>())
            {
                jsonCollectionItems.Add(ExportToPostman(childItem));
            }

            foreach (var childItem in collection.Items.OfType<RequestMessage>())
            {
                jsonCollectionItems.Add(ExportToPostman((Item) childItem));
            }

            jsonCollection.Add("item", jsonCollectionItems);

            if (collection.AuthenticationMethod != null && !(collection.AuthenticationMethod is NoAuthentication))
            {
                jsonCollection.Add("auth", ExportToPostman(collection.AuthenticationMethod));
            }

            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            using (var jsonWriter = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4
            })
            {
                jsonCollection.WriteTo(jsonWriter);
            }

            return stringBuilder.ToString();
        }
    }
}
