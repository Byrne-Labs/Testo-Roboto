using System;
using System.Linq;
using System.Net.Http;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class CollectionTest
    {
        [Fact]
        public void TestCreateCollectionFuzzedMessages()
        {
            var collection = new Collection();
            var requestMessage = new RequestMessage();
            requestMessage.Body = new RawBody { Text = "{ \"asdf\": 123 }" };
            requestMessage.HttpMethod = HttpMethod.Post;
            requestMessage.Uri = new Uri("http://some.domain/path/resource?key=value");
            collection.Items.Add(requestMessage);
            collection.Items.Add(new Collection { Name = "Fuzzed Messages" });
            var subCollection = new Collection { Name = "Sub-collection" };
            collection.Items.Add(subCollection);
            subCollection.Items.Add(new RequestMessage { Body = new RawBody { Text = "{ \"xyz\": 456 }" } });

            var mutator = new Mock<Mutator>();
            mutator.Setup(m => m.MutateMessage(It.IsAny<string>())).Returns((string message) =>
            {
                var jObject = JObject.Parse(message);
                foreach (var value in jObject.Descendants().OfType<JValue>())
                {
                    value.Value = "asdf";
                }

                return new[] { jObject.ToString(Formatting.None) };
            });
            collection.AddFuzzedMessages(new[] { mutator.Object }, true);

            Assert.Equal(3, collection.Items.Count);
            Assert.Single(collection.Items.OfType<RequestMessage>());
            Assert.False(((RequestMessage) collection.Items[0]).FuzzedMessage);
            Assert.Equal(2, collection.Items.OfType<Collection>().Count());
            Assert.Single(collection.Items.OfType<Collection>().First().Items.OfType<RequestMessage>());
            Assert.True(collection.Items.OfType<Collection>().First().Items.OfType<RequestMessage>().Single().FuzzedMessage);
            Assert.Equal("{\"asdf\":\"asdf\"}", ((RawBody) collection.Items.OfType<Collection>().First().Items.OfType<RequestMessage>().Single().Body).Text);

            Assert.Equal(2, subCollection.Items.Count);
            Assert.Single(subCollection.Items.OfType<RequestMessage>());
            Assert.False(((RequestMessage) subCollection.Items[0]).FuzzedMessage);
            Assert.Single(subCollection.Items.OfType<Collection>());
            Assert.Single(subCollection.Items.OfType<Collection>().First().Items.OfType<RequestMessage>());
            Assert.True(subCollection.Items.OfType<Collection>().Single().Items.OfType<RequestMessage>().Single().FuzzedMessage);
            Assert.Equal("{\"xyz\":\"asdf\"}", ((RawBody) subCollection.Items.OfType<Collection>().Single().Items.OfType<RequestMessage>().Single().Body).Text);
        }

        [Fact]
        public void TestImportFromPostmanAwsSignature()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
		                ""type"": ""awsv4"",
		                ""awsv4"": [
			                {
				                ""key"": ""sessionToken"",
				                ""value"": ""mySessionToken"",
				                ""type"": ""string""
			                },
			                {
				                ""key"": ""service"",
				                ""value"": ""myService"",
				                ""type"": ""string""
			                },
			                {
				                ""key"": ""region"",
				                ""value"": ""myRegion"",
				                ""type"": ""string""
			                },
			                {
				                ""key"": ""secretKey"",
				                ""value"": ""mySecretKey"",
				                ""type"": ""string""
			                },
			                {
				                ""key"": ""accessKey"",
				                ""value"": ""myAccessKey"",
				                ""type"": ""string""
			                }
		                ]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<AwsSignature>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as AwsSignature;
            Assert.Equal("myAccessKey", authentication.AccessKey);
            Assert.Equal("myRegion", authentication.Region);
            Assert.Equal("mySecretKey", authentication.SecretKey);
            Assert.Equal("myService", authentication.ServiceName);
            Assert.Equal("mySessionToken", authentication.SessionToken);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanBasicAuthentication()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""basic"",
						""basic"": [
							{
								""key"": ""password"",
								""value"": ""myPassword"",
								""type"": ""string""
							},
							{
								""key"": ""username"",
								""value"": ""myUserName"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<BasicAuthentication>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as BasicAuthentication;
            Assert.Equal("myPassword", authentication.Password);
            Assert.Equal("myUserName", authentication.Username);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanBearerToken()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""bearer"",
						""bearer"": [
							{
								""key"": ""token"",
								""value"": ""myToken"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<BearerToken>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as BearerToken;
            Assert.Equal("myToken", authentication.Token);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanDigestAuthenticationWithInvalid()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""digest"",
						""digest"": [
							{
								""key"": ""algorithm"",
								""value"": ""MD500-sess"",
								""type"": ""string""
							}
						]
	                }
                }";

            Assert.Throws<ArgumentException>(() => Collection.ImportFromPostmanJson(json));
        }

        [Fact]
        public void TestImportFromPostmanDigestAuthenticationWithMd5()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""digest"",
						""digest"": [
							{
								""key"": ""opaque"",
								""value"": ""myOpaque"",
								""type"": ""string""
							},
							{
								""key"": ""clientNonce"",
								""value"": ""myClientNonce"",
								""type"": ""string""
							},
							{
								""key"": ""nonceCount"",
								""value"": ""myNonceCount"",
								""type"": ""string""
							},
							{
								""key"": ""qop"",
								""value"": ""myQop"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""realm"",
								""value"": ""myRealm"",
								""type"": ""string""
							},
							{
								""key"": ""password"",
								""value"": ""myPassword"",
								""type"": ""string""
							},
							{
								""key"": ""username"",
								""value"": ""myUsername"",
								""type"": ""string""
							},
							{
								""key"": ""algorithm"",
								""value"": ""MD5"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<DigestAuthentication>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as DigestAuthentication;
            Assert.Equal(DigestAuthenticationAlgorithm.Md5, authentication.Algorithm);
            Assert.Equal("myClientNonce", authentication.ClientNonce);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myNonceCount", authentication.NonceCount);
            Assert.Equal("myOpaque", authentication.Opaque);
            Assert.Equal("myPassword", authentication.Password);
            Assert.Equal("myQop", authentication.Qop);
            Assert.Equal("myRealm", authentication.Realm);
            Assert.Equal("myUsername", authentication.Username);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanDigestAuthenticationWithMd5Sess()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""digest"",
						""digest"": [
							{
								""key"": ""opaque"",
								""value"": ""myOpaque"",
								""type"": ""string""
							},
							{
								""key"": ""clientNonce"",
								""value"": ""myClientNonce"",
								""type"": ""string""
							},
							{
								""key"": ""nonceCount"",
								""value"": ""myNonceCount"",
								""type"": ""string""
							},
							{
								""key"": ""qop"",
								""value"": ""myQop"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""realm"",
								""value"": ""myRealm"",
								""type"": ""string""
							},
							{
								""key"": ""password"",
								""value"": ""myPassword"",
								""type"": ""string""
							},
							{
								""key"": ""username"",
								""value"": ""myUsername"",
								""type"": ""string""
							},
							{
								""key"": ""algorithm"",
								""value"": ""MD5-sess"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<DigestAuthentication>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as DigestAuthentication;
            Assert.Equal(DigestAuthenticationAlgorithm.Md5Sess, authentication.Algorithm);
            Assert.Equal("myClientNonce", authentication.ClientNonce);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myNonceCount", authentication.NonceCount);
            Assert.Equal("myOpaque", authentication.Opaque);
            Assert.Equal("myPassword", authentication.Password);
            Assert.Equal("myQop", authentication.Qop);
            Assert.Equal("myRealm", authentication.Realm);
            Assert.Equal("myUsername", authentication.Username);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanHawkWithInvalid()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""hawk"",
						""hawk"": [
							{
								""key"": ""algorithm"",
								""value"": ""sha4096"",
								""type"": ""string""
							}
						]
	                }
                }";

            Assert.Throws<ArgumentException>(() => Collection.ImportFromPostmanJson(json));
        }

        [Fact]
        public void TestImportFromPostmanHawkWithSha1()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""hawk"",
						""hawk"": [
							{
								""key"": ""timestamp"",
								""value"": ""myTimestamp"",
								""type"": ""string""
							},
							{
								""key"": ""delegation"",
								""value"": ""myDelegation"",
								""type"": ""string""
							},
							{
								""key"": ""appId"",
								""value"": ""myAppId"",
								""type"": ""string""
							},
							{
								""key"": ""extraData"",
								""value"": ""myExtraData"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""user"",
								""value"": ""myUser"",
								""type"": ""string""
							},
							{
								""key"": ""authKey"",
								""value"": ""myAuthKey"",
								""type"": ""string""
							},
							{
								""key"": ""authId"",
								""value"": ""myAuthId"",
								""type"": ""string""
							},
							{
								""key"": ""algorithm"",
								""value"": ""sha1"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<HawkAuthentication>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as HawkAuthentication;
            Assert.Equal(HawkAuthenticationAlgorithm.Sha1, authentication.Algorithm);
            Assert.Equal("myAppId", authentication.ApplicationId);
            Assert.Equal("myAuthId", authentication.AuthenticationId);
            Assert.Equal("myAuthKey", authentication.AuthenticationKey);
            Assert.Equal("myDelegation", authentication.Delegation);
            Assert.Equal("myExtraData", authentication.ExtraData);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myTimestamp", authentication.Timestamp);
            Assert.Equal("myUser", authentication.User);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanHawkWithSha256()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""hawk"",
						""hawk"": [
							{
								""key"": ""timestamp"",
								""value"": ""myTimestamp"",
								""type"": ""string""
							},
							{
								""key"": ""delegation"",
								""value"": ""myDelegation"",
								""type"": ""string""
							},
							{
								""key"": ""appId"",
								""value"": ""myAppId"",
								""type"": ""string""
							},
							{
								""key"": ""extraData"",
								""value"": ""myExtraData"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""user"",
								""value"": ""myUser"",
								""type"": ""string""
							},
							{
								""key"": ""authKey"",
								""value"": ""myAuthKey"",
								""type"": ""string""
							},
							{
								""key"": ""authId"",
								""value"": ""myAuthId"",
								""type"": ""string""
							},
							{
								""key"": ""algorithm"",
								""value"": ""sha256"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<HawkAuthentication>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as HawkAuthentication;
            Assert.Equal(HawkAuthenticationAlgorithm.Sha256, authentication.Algorithm);
            Assert.Equal("myAppId", authentication.ApplicationId);
            Assert.Equal("myAuthId", authentication.AuthenticationId);
            Assert.Equal("myAuthKey", authentication.AuthenticationKey);
            Assert.Equal("myDelegation", authentication.Delegation);
            Assert.Equal("myExtraData", authentication.ExtraData);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myTimestamp", authentication.Timestamp);
            Assert.Equal("myUser", authentication.User);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanMessageFormData()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""Some Messages"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [
		                {
			                ""name"": ""Some Message"",
			                ""request"": {
				                ""method"": ""POST"",
				                ""header"": [
					                {
						                ""key"": ""Key1"",
						                ""value"": ""Value1"",
						                ""description"": ""Description 1"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value2"",
						                ""description"": ""Description 2"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value3"",
						                ""description"": ""Description 3"",
						                ""type"": ""text""
					                }
				                ],
				                ""body"": {
					                ""mode"": ""formdata"",
					                ""formdata"": [
						                {
							                ""key"": ""Key1"",
							                ""value"": ""Value1"",
							                ""description"": ""Description 1"",
							                ""type"": ""text""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value2"",
							                ""description"": ""Description 2"",
							                ""type"": ""text""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value3"",
							                ""description"": ""Description 3"",
							                ""type"": ""text""
						                }
					                ]
				                },
				                ""url"": {
					                ""raw"": ""https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"",
					                ""protocol"": ""https"",
					                ""host"": [
						                ""some"",
						                ""domain""
					                ],
					                ""path"": [
						                ""path1"",
						                ""path2"",
						                ""resource""
					                ],
					                ""query"": [
						                {
							                ""key"": ""Key1"",
							                ""value"": ""Value1"",
							                ""description"": ""Description 1""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value2"",
							                ""description"": ""Description 2""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value3"",
							                ""description"": ""Description 3""
						                }
					                ]
				                }
			                }
		                }
	                ]
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.NotNull(collection);
            Assert.Equal("Some Messages", collection.Name);
            Assert.Equal(1, collection.Items.Count);
            Assert.IsType<RequestMessage>(collection.Items[0]);
            var requestMessage = collection.Items[0] as RequestMessage;
            Assert.Equal("Some Message", requestMessage.Name);
            Assert.Null(collection.Items[0].Description);
            Assert.IsType<NoAuthentication>(requestMessage.AuthenticationMethod);
            Assert.IsType<FormDataBody>(requestMessage.Body);
            var body = requestMessage.Body as FormDataBody;
            Assert.Equal(3, body.FormData.Count);
            Assert.Equal("Key1", body.FormData[0].Key);
            Assert.Equal("Value1", body.FormData[0].Value);
            Assert.Equal("Description 1", body.FormData[0].Description);
            Assert.Equal("Key2", body.FormData[1].Key);
            Assert.Equal("Value2", body.FormData[1].Value);
            Assert.Equal("Description 2", body.FormData[1].Description);
            Assert.Equal("Key2", body.FormData[2].Key);
            Assert.Equal("Value3", body.FormData[2].Value);
            Assert.Equal("Description 3", body.FormData[2].Description);
            Assert.Equal(3, requestMessage.Headers.Count);
            Assert.Equal("Key1", requestMessage.Headers[0].Key);
            Assert.Equal("Value1", requestMessage.Headers[0].Value);
            Assert.Equal("Description 1", requestMessage.Headers[0].Description);
            Assert.Equal("Key2", requestMessage.Headers[1].Key);
            Assert.Equal("Value2", requestMessage.Headers[1].Value);
            Assert.Equal("Description 2", requestMessage.Headers[1].Description);
            Assert.Equal("Key2", requestMessage.Headers[2].Key);
            Assert.Equal("Value3", requestMessage.Headers[2].Value);
            Assert.Equal("Description 3", requestMessage.Headers[2].Description);
            Assert.Equal(HttpMethod.Post, requestMessage.HttpMethod);
            Assert.Equal(3, requestMessage.QueryStringParameters.Count);
            Assert.Equal("Key1", requestMessage.QueryStringParameters[0].Key);
            Assert.Equal("Value1", requestMessage.QueryStringParameters[0].Value);
            Assert.Equal("Description 1", requestMessage.QueryStringParameters[0].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[1].Key);
            Assert.Equal("Value2", requestMessage.QueryStringParameters[1].Value);
            Assert.Equal("Description 2", requestMessage.QueryStringParameters[1].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[2].Key);
            Assert.Equal("Value3", requestMessage.QueryStringParameters[2].Value);
            Assert.Equal("Description 3", requestMessage.QueryStringParameters[2].Description);
            Assert.Equal(new Uri("https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"), requestMessage.Uri);
        }

        [Fact]
        public void TestImportFromPostmanMessageNoBody()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""Some Messages"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [
		                {
			                ""name"": ""Some Message"",
			                ""request"": {
				                ""method"": ""PUT"",
				                ""header"": [
					                {
						                ""key"": ""Key1"",
						                ""value"": ""Value1"",
						                ""description"": ""Description 1"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value2"",
						                ""description"": ""Description 2"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value3"",
						                ""description"": ""Description 3"",
						                ""type"": ""text""
					                }
				                ],
				                ""body"": {
					                ""mode"": ""raw"",
					                ""raw"": """"
				                },
				                ""url"": {
					                ""raw"": ""https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"",
					                ""protocol"": ""https"",
					                ""host"": [
						                ""some"",
						                ""domain""
					                ],
					                ""path"": [
						                ""path1"",
						                ""path2"",
						                ""resource""
					                ],
					                ""query"": [
						                {
							                ""key"": ""Key1"",
							                ""value"": ""Value1"",
							                ""description"": ""Description 1""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value2"",
							                ""description"": ""Description 2""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value3"",
							                ""description"": ""Description 3""
						                }
					                ]
				                }
			                }
		                }
	                ]
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.NotNull(collection);
            Assert.Equal("Some Messages", collection.Name);
            Assert.Equal(1, collection.Items.Count);
            Assert.IsType<RequestMessage>(collection.Items[0]);
            var requestMessage = collection.Items[0] as RequestMessage;
            Assert.Equal("Some Message", requestMessage.Name);
            Assert.Null(collection.Items[0].Description);
            Assert.IsType<NoAuthentication>(requestMessage.AuthenticationMethod);
            Assert.IsType<NoBody>(requestMessage.Body);
            Assert.Equal(3, requestMessage.Headers.Count);
            Assert.Equal("Key1", requestMessage.Headers[0].Key);
            Assert.Equal("Value1", requestMessage.Headers[0].Value);
            Assert.Equal("Description 1", requestMessage.Headers[0].Description);
            Assert.Equal("Key2", requestMessage.Headers[1].Key);
            Assert.Equal("Value2", requestMessage.Headers[1].Value);
            Assert.Equal("Description 2", requestMessage.Headers[1].Description);
            Assert.Equal("Key2", requestMessage.Headers[2].Key);
            Assert.Equal("Value3", requestMessage.Headers[2].Value);
            Assert.Equal("Description 3", requestMessage.Headers[2].Description);
            Assert.Equal(HttpMethod.Put, requestMessage.HttpMethod);
            Assert.Equal(3, requestMessage.QueryStringParameters.Count);
            Assert.Equal("Key1", requestMessage.QueryStringParameters[0].Key);
            Assert.Equal("Value1", requestMessage.QueryStringParameters[0].Value);
            Assert.Equal("Description 1", requestMessage.QueryStringParameters[0].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[1].Key);
            Assert.Equal("Value2", requestMessage.QueryStringParameters[1].Value);
            Assert.Equal("Description 2", requestMessage.QueryStringParameters[1].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[2].Key);
            Assert.Equal("Value3", requestMessage.QueryStringParameters[2].Value);
            Assert.Equal("Description 3", requestMessage.QueryStringParameters[2].Description);
            Assert.Equal(new Uri("https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"), requestMessage.Uri);
        }

        [Fact]
        public void TestImportFromPostmanMessageRaw()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""Some Messages"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [
		                {
			                ""name"": ""Some Message"",
			                ""request"": {
				                ""method"": ""DELETE"",
				                ""header"": [
					                {
						                ""key"": ""Key1"",
						                ""value"": ""Value1"",
						                ""description"": ""Description 1"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value2"",
						                ""description"": ""Description 2"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value3"",
						                ""description"": ""Description 3"",
						                ""type"": ""text""
					                }
				                ],
				                ""body"": {
					                ""mode"": ""raw"",
					                ""raw"": ""{ \""asdf\"": 123, \""fdsa\"": \""asdf\""}""
				                },
				                ""url"": {
					                ""raw"": ""https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"",
					                ""protocol"": ""https"",
					                ""host"": [
						                ""some"",
						                ""domain""
					                ],
					                ""path"": [
						                ""path1"",
						                ""path2"",
						                ""resource""
					                ],
					                ""query"": [
						                {
							                ""key"": ""Key1"",
							                ""value"": ""Value1"",
							                ""description"": ""Description 1""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value2"",
							                ""description"": ""Description 2""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value3"",
							                ""description"": ""Description 3""
						                }
					                ]
				                }
			                }
		                }
	                ]
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.NotNull(collection);
            Assert.Equal("Some Messages", collection.Name);
            Assert.Equal(1, collection.Items.Count);
            Assert.IsType<RequestMessage>(collection.Items[0]);
            var requestMessage = collection.Items[0] as RequestMessage;
            Assert.Equal("Some Message", requestMessage.Name);
            Assert.Null(collection.Items[0].Description);
            Assert.IsType<NoAuthentication>(requestMessage.AuthenticationMethod);
            Assert.IsType<RawBody>(requestMessage.Body);
            var body = requestMessage.Body as RawBody;
            Assert.Equal(@"{ ""asdf"": 123, ""fdsa"": ""asdf""}", body.Text);
            Assert.Equal(3, requestMessage.Headers.Count);
            Assert.Equal("Key1", requestMessage.Headers[0].Key);
            Assert.Equal("Value1", requestMessage.Headers[0].Value);
            Assert.Equal("Description 1", requestMessage.Headers[0].Description);
            Assert.Equal("Key2", requestMessage.Headers[1].Key);
            Assert.Equal("Value2", requestMessage.Headers[1].Value);
            Assert.Equal("Description 2", requestMessage.Headers[1].Description);
            Assert.Equal("Key2", requestMessage.Headers[2].Key);
            Assert.Equal("Value3", requestMessage.Headers[2].Value);
            Assert.Equal("Description 3", requestMessage.Headers[2].Description);
            Assert.Equal(HttpMethod.Delete, requestMessage.HttpMethod);
            Assert.Equal(3, requestMessage.QueryStringParameters.Count);
            Assert.Equal("Key1", requestMessage.QueryStringParameters[0].Key);
            Assert.Equal("Value1", requestMessage.QueryStringParameters[0].Value);
            Assert.Equal("Description 1", requestMessage.QueryStringParameters[0].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[1].Key);
            Assert.Equal("Value2", requestMessage.QueryStringParameters[1].Value);
            Assert.Equal("Description 2", requestMessage.QueryStringParameters[1].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[2].Key);
            Assert.Equal("Value3", requestMessage.QueryStringParameters[2].Value);
            Assert.Equal("Description 3", requestMessage.QueryStringParameters[2].Description);
            Assert.Equal(new Uri("https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"), requestMessage.Uri);
        }

        [Fact]
        public void TestImportFromPostmanMessageUrlEncoded()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""Some Messages"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [
		                {
			                ""name"": ""Some Message"",
			                ""request"": {
				                ""method"": ""HEAD"",
				                ""header"": [
					                {
						                ""key"": ""Key1"",
						                ""value"": ""Value1"",
						                ""description"": ""Description 1"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value2"",
						                ""description"": ""Description 2"",
						                ""type"": ""text""
					                },
					                {
						                ""key"": ""Key2"",
						                ""value"": ""Value3"",
						                ""description"": ""Description 3"",
						                ""type"": ""text""
					                }
				                ],
				                ""body"": {
					                ""mode"": ""urlencoded"",
					                ""urlencoded"": [
						                {
							                ""key"": ""Key1"",
							                ""value"": ""Value1"",
							                ""description"": ""Description 1"",
							                ""type"": ""text""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value2"",
							                ""description"": ""Description 2"",
							                ""type"": ""text""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value3"",
							                ""description"": ""Description 3"",
							                ""type"": ""text""
						                }
					                ]
				                },
				                ""url"": {
					                ""raw"": ""https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"",
					                ""protocol"": ""https"",
					                ""host"": [
						                ""some"",
						                ""domain""
					                ],
					                ""path"": [
						                ""path1"",
						                ""path2"",
						                ""resource""
					                ],
					                ""query"": [
						                {
							                ""key"": ""Key1"",
							                ""value"": ""Value1"",
							                ""description"": ""Description 1""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value2"",
							                ""description"": ""Description 2""
						                },
						                {
							                ""key"": ""Key2"",
							                ""value"": ""Value3"",
							                ""description"": ""Description 3""
						                }
					                ]
				                }
			                }
		                }
	                ]
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.NotNull(collection);
            Assert.Equal("Some Messages", collection.Name);
            Assert.Equal(1, collection.Items.Count);
            Assert.IsType<RequestMessage>(collection.Items[0]);
            var requestMessage = collection.Items[0] as RequestMessage;
            Assert.Equal("Some Message", requestMessage.Name);
            Assert.Null(collection.Items[0].Description);
            Assert.IsType<NoAuthentication>(requestMessage.AuthenticationMethod);
            Assert.IsType<UrlEncodedBody>(requestMessage.Body);
            var body = requestMessage.Body as UrlEncodedBody;
            Assert.Equal(3, body.FormData.Count);
            Assert.Equal("Key1", body.FormData[0].Key);
            Assert.Equal("Value1", body.FormData[0].Value);
            Assert.Equal("Description 1", body.FormData[0].Description);
            Assert.Equal("Key2", body.FormData[1].Key);
            Assert.Equal("Value2", body.FormData[1].Value);
            Assert.Equal("Description 2", body.FormData[1].Description);
            Assert.Equal("Key2", body.FormData[2].Key);
            Assert.Equal("Value3", body.FormData[2].Value);
            Assert.Equal("Description 3", body.FormData[2].Description);
            Assert.Equal(3, requestMessage.Headers.Count);
            Assert.Equal("Key1", requestMessage.Headers[0].Key);
            Assert.Equal("Value1", requestMessage.Headers[0].Value);
            Assert.Equal("Description 1", requestMessage.Headers[0].Description);
            Assert.Equal("Key2", requestMessage.Headers[1].Key);
            Assert.Equal("Value2", requestMessage.Headers[1].Value);
            Assert.Equal("Description 2", requestMessage.Headers[1].Description);
            Assert.Equal("Key2", requestMessage.Headers[2].Key);
            Assert.Equal("Value3", requestMessage.Headers[2].Value);
            Assert.Equal("Description 3", requestMessage.Headers[2].Description);
            Assert.Equal(HttpMethod.Head, requestMessage.HttpMethod);
            Assert.Equal(3, requestMessage.QueryStringParameters.Count);
            Assert.Equal("Key1", requestMessage.QueryStringParameters[0].Key);
            Assert.Equal("Value1", requestMessage.QueryStringParameters[0].Value);
            Assert.Equal("Description 1", requestMessage.QueryStringParameters[0].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[1].Key);
            Assert.Equal("Value2", requestMessage.QueryStringParameters[1].Value);
            Assert.Equal("Description 2", requestMessage.QueryStringParameters[1].Description);
            Assert.Equal("Key2", requestMessage.QueryStringParameters[2].Key);
            Assert.Equal("Value3", requestMessage.QueryStringParameters[2].Value);
            Assert.Equal("Description 3", requestMessage.QueryStringParameters[2].Description);
            Assert.Equal(new Uri("https://some.domain/path1/path2/resource?Key1=Value1&Key2=Value2&Key2=Value3"), requestMessage.Uri);
        }

        [Fact]
        public void TestImportFromPostmanNoAuthentication()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": []
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<NoAuthentication>(collection.AuthenticationMethod);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanNoAuthenticationMissing()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
					""auth"": {
						""type"": ""noauth""
					}
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<NoAuthentication>(collection.AuthenticationMethod);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanNtlm()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""ntlm"",
						""ntlm"": [
							{
								""key"": ""workstation"",
								""value"": ""myWorkstation"",
								""type"": ""string""
							},
							{
								""key"": ""domain"",
								""value"": ""myDomain"",
								""type"": ""string""
							},
							{
								""key"": ""password"",
								""value"": ""myPassword"",
								""type"": ""string""
							},
							{
								""key"": ""username"",
								""value"": ""myUsername"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<NtlmAuthentication>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as NtlmAuthentication;
            Assert.Equal("myDomain", authentication.Domain);
            Assert.Equal("myPassword", authentication.Password);
            Assert.Equal("myUsername", authentication.Username);
            Assert.Equal("myWorkstation", authentication.Workstation);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanOAuth1WithInvalid()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth1"",
						""oauth1"": [
							{
								""key"": ""signatureMethod"",
								""value"": ""HMAC-SHA4096"",
								""type"": ""mySignatureMethod""
							}
                        ]
	                }
                }";

            Assert.Throws<ArgumentException>(() => Collection.ImportFromPostmanJson(json));
        }

        [Fact]
        public void TestImportFromPostmanOAuth1WithPlainText()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth1"",
						""oauth1"": [
							{
								""key"": ""addParamsToHeader"",
								""value"": true,
								""type"": ""boolean""
							},
							{
								""key"": ""realm"",
								""value"": ""myRealm"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""timestamp"",
								""value"": ""myTimestamp"",
								""type"": ""string""
							},
							{
								""key"": ""signatureMethod"",
								""value"": ""PLAINTEXT"",
								""type"": ""mySignatureMethod""
							},
							{
								""key"": ""tokenSecret"",
								""value"": ""myTokenSecret"",
								""type"": ""string""
							},
							{
								""key"": ""token"",
								""value"": ""myToken"",
								""type"": ""string""
							},
							{
								""key"": ""consumerSecret"",
								""value"": ""myConsumerSecret"",
								""type"": ""string""
							},
							{
								""key"": ""consumerKey"",
								""value"": ""myConsumerKey"",
								""type"": ""string""
							},
							{
								""key"": ""addEmptyParamsToSign"",
								""value"": true,
								""type"": ""boolean""
							},
							{
								""key"": ""version"",
								""value"": ""myVersion"",
								""type"": ""string""
							}
                        ]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<OAuth1>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as OAuth1;
            Assert.Equal("myToken", authentication.AccessToken);
            Assert.True(authentication.AddEmptyParametersToSignature);
            Assert.Equal("myConsumerKey", authentication.ConsumerKey);
            Assert.Equal("myConsumerSecret", authentication.ConsumerSecret);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myRealm", authentication.Realm);
            Assert.Equal(OAuth1SignatureMethod.PlainText, authentication.SignatureMethod);
            Assert.Equal("myTimestamp", authentication.Timestamp);
            Assert.Equal(OAuth1TokenLocation.Headers, authentication.TokenLocation);
            Assert.Equal("myTokenSecret", authentication.TokenSecret);
            Assert.Equal("myVersion", authentication.Version);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanOAuth1WithSha1()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth1"",
						""oauth1"": [
							{
								""key"": ""addParamsToHeader"",
								""value"": false,
								""type"": ""boolean""
							},
							{
								""key"": ""realm"",
								""value"": ""myRealm"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""timestamp"",
								""value"": ""myTimestamp"",
								""type"": ""string""
							},
							{
								""key"": ""signatureMethod"",
								""value"": ""HMAC-SHA1"",
								""type"": ""mySignatureMethod""
							},
							{
								""key"": ""tokenSecret"",
								""value"": ""myTokenSecret"",
								""type"": ""string""
							},
							{
								""key"": ""token"",
								""value"": ""myToken"",
								""type"": ""string""
							},
							{
								""key"": ""consumerSecret"",
								""value"": ""myConsumerSecret"",
								""type"": ""string""
							},
							{
								""key"": ""consumerKey"",
								""value"": ""myConsumerKey"",
								""type"": ""string""
							},
							{
								""key"": ""addEmptyParamsToSign"",
								""value"": false,
								""type"": ""boolean""
							},
							{
								""key"": ""version"",
								""value"": ""myVersion"",
								""type"": ""string""
							}
                        ]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<OAuth1>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as OAuth1;
            Assert.Equal("myToken", authentication.AccessToken);
            Assert.False(authentication.AddEmptyParametersToSignature);
            Assert.Equal("myConsumerKey", authentication.ConsumerKey);
            Assert.Equal("myConsumerSecret", authentication.ConsumerSecret);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myRealm", authentication.Realm);
            Assert.Equal(OAuth1SignatureMethod.HmacSha1, authentication.SignatureMethod);
            Assert.Equal("myTimestamp", authentication.Timestamp);
            Assert.Equal(OAuth1TokenLocation.BodyAndUrl, authentication.TokenLocation);
            Assert.Equal("myTokenSecret", authentication.TokenSecret);
            Assert.Equal("myVersion", authentication.Version);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanOAuth1WithSha256()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth1"",
						""oauth1"": [
							{
								""key"": ""addParamsToHeader"",
								""value"": true,
								""type"": ""boolean""
							},
							{
								""key"": ""realm"",
								""value"": ""myRealm"",
								""type"": ""string""
							},
							{
								""key"": ""nonce"",
								""value"": ""myNonce"",
								""type"": ""string""
							},
							{
								""key"": ""timestamp"",
								""value"": ""myTimestamp"",
								""type"": ""string""
							},
							{
								""key"": ""signatureMethod"",
								""value"": ""HMAC-SHA256"",
								""type"": ""mySignatureMethod""
							},
							{
								""key"": ""tokenSecret"",
								""value"": ""myTokenSecret"",
								""type"": ""string""
							},
							{
								""key"": ""token"",
								""value"": ""myToken"",
								""type"": ""string""
							},
							{
								""key"": ""consumerSecret"",
								""value"": ""myConsumerSecret"",
								""type"": ""string""
							},
							{
								""key"": ""consumerKey"",
								""value"": ""myConsumerKey"",
								""type"": ""string""
							},
							{
								""key"": ""addEmptyParamsToSign"",
								""value"": true,
								""type"": ""boolean""
							},
							{
								""key"": ""version"",
								""value"": ""myVersion"",
								""type"": ""string""
							}
                        ]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<OAuth1>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as OAuth1;
            Assert.Equal("myToken", authentication.AccessToken);
            Assert.True(authentication.AddEmptyParametersToSignature);
            Assert.Equal("myConsumerKey", authentication.ConsumerKey);
            Assert.Equal("myConsumerSecret", authentication.ConsumerSecret);
            Assert.Equal("myNonce", authentication.Nonce);
            Assert.Equal("myRealm", authentication.Realm);
            Assert.Equal(OAuth1SignatureMethod.HmacSha256, authentication.SignatureMethod);
            Assert.Equal("myTimestamp", authentication.Timestamp);
            Assert.Equal(OAuth1TokenLocation.Headers, authentication.TokenLocation);
            Assert.Equal("myTokenSecret", authentication.TokenSecret);
            Assert.Equal("myVersion", authentication.Version);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanOAuth2WithHeaders()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth2"",
						""oauth2"": [
							{
								""key"": ""accessToken"",
								""value"": ""myAccessToken"",
								""type"": ""string""
							},
							{
								""key"": ""addTokenTo"",
								""value"": ""header"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<OAuth2>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as OAuth2;
            Assert.Equal("myAccessToken", authentication.AccessToken);
            Assert.Equal(OAuth2TokenLocation.Headers, authentication.TokenLocation);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanOAuth2WithInvalid()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth2"",
						""oauth2"": [
							{
								""key"": ""addTokenTo"",
								""value"": ""footer"",
								""type"": ""string""
							}
						]
	                }
                }";

            Assert.Throws<ArgumentException>(() => Collection.ImportFromPostmanJson(json));
        }

        [Fact]
        public void TestImportFromPostmanOAuth2WithQueryParameters()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""empty"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [],
	                ""auth"": {
						""type"": ""oauth2"",
						""oauth2"": [
							{
								""key"": ""accessToken"",
								""value"": ""myAccessToken"",
								""type"": ""string""
							},
							{
								""key"": ""addTokenTo"",
								""value"": ""queryParams"",
								""type"": ""string""
							}
						]
	                }
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.IsType<OAuth2>(collection.AuthenticationMethod);
            var authentication = collection.AuthenticationMethod as OAuth2;
            Assert.Equal("myAccessToken", authentication.AccessToken);
            Assert.Equal(OAuth2TokenLocation.QueryParameters, authentication.TokenLocation);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void TestImportFromPostmanSubCollection()
        {
            var json = @"
                {
	                ""info"": {
		                ""_postman_id"": ""6f02b4ed-5230-43d8-9a32-62a4500fc7c2"",
		                ""name"": ""Some Messages"",
		                ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
	                },
	                ""item"": [
		                {
			                ""name"": ""Folder"",
			                ""item"": [
				                {
					                ""name"": ""SomeMessage"",
					                ""request"": {
						                ""method"": ""GET"",
						                ""header"": [],
						                ""body"": {
							                ""mode"": ""raw"",
							                ""raw"": """"
						                },
						                ""url"": {
							                ""raw"": ""https://some.domain/path1/path2/resource"",
							                ""protocol"": ""https"",
							                ""host"": [
								                ""some"",
								                ""domain""
							                ],
							                ""path"": [
								                ""path1"",
								                ""path2"",
								                ""resource""
							                ]
						                },
						                ""description"": ""Description""
					                },
					                ""response"": []
				                }
			                ]
		                }
	                ]
                }";

            var collection = Collection.ImportFromPostmanJson(json);
            Assert.Single(collection.Items);
            Assert.IsType<Collection>(collection.Items[0]);
            Assert.Single(collection.Items.OfType<Collection>().Single().Items);
            Assert.IsType<RequestMessage>(collection.Items.OfType<Collection>().Single().Items.Single());
        }
    }
}
