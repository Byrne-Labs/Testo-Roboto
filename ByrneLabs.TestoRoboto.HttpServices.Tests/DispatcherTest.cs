using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class DispatcherTest
    {
        [Fact]
        public void IntegrationTestDispatch()
        {
            var message = @"
                {
                  ""product"": {
                    ""title"": ""something"",
                    ""vendor"": ""Burton"",
                    ""product_type"": ""Snowboard"",
                    ""variants"": [
                      {
                        ""option1"": ""Blue""
                      }
                    ],
                    ""options"": [
                      {
                        ""name"": ""Color"",
                        ""values"": [
                          ""Blue""
                        ]
                      }
                    ]
                  }
                }";

            var requestMessage = new RequestMessage();
            requestMessage.Uri = new Uri("https://testoroboto.myshopify.com/admin/products.json");
            requestMessage.AuthenticationMethod = new BasicAuthentication { Username = "36e0780065f769830b0c2cb8dc18fd89", Password = "9e54820bab53b90fa12f1ddfd1528bb1" };
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            requestMessage.Body = new RawBody { Text = message };
            requestMessage.HttpMethod = HttpMethod.Post;
            requestMessage.ExpectedStatusCode = HttpStatusCode.Created;

            var collection = new Collection();
            collection.Items.Add(requestMessage);

            var mutator = new Mock<Mutator>();
            mutator.Setup(m => m.MutateMessage(It.IsAny<RequestMessage>())).Returns((RequestMessage requestMessageToFuzz) =>
            {
                var fuzzedRequestMessage = requestMessageToFuzz.Clone();
                fuzzedRequestMessage.FuzzedMessage = true;

                var jObject = JObject.Parse(((RawBody) fuzzedRequestMessage.Body).Text);
                jObject["product"] = null;

                ((RawBody) fuzzedRequestMessage.Body).Text = jObject.ToString(Formatting.None);

                return new[] { fuzzedRequestMessage };
            });
            collection.AddFuzzedMessages(new[] { mutator.Object }, true);

            var testRequest = new TestRequest();
            testRequest.TimeBetweenRequests = 500;
            testRequest.Items.Add(collection);

            var dispatcher = new Dispatcher();
            dispatcher.Dispatch(testRequest);

            Assert.Single(requestMessage.ResponseMessages);
            Assert.Equal(HttpStatusCode.Created, requestMessage.ResponseMessages.Single().StatusCode);

            var fuzzedRequest = collection.Items.OfType<Collection>().Single().Items.OfType<RequestMessage>().Single();

            Assert.Single(fuzzedRequest.ResponseMessages);
            Assert.Equal(HttpStatusCode.BadRequest, fuzzedRequest.ResponseMessages.Single().StatusCode);
        }
    }
}
