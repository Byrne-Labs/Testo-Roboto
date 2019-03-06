﻿using System;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class CollectionTest
    {
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
    }
}
