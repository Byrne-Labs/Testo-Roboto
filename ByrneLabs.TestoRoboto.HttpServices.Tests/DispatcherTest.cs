﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using Moq;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class DispatcherTest
    {
        [Fact]
        public void IntegrationTestDispatch()
        {
            var message = @"{""name"":""test"",""salary"":""123"",""age"":""23""}";

            var requestMessage = new RequestMessage();
            requestMessage.Uri = new Uri("https://postman-echo.com/post");
            requestMessage.AuthenticationMethod = new NoAuthentication();
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            requestMessage.Body = new RawBody { Text = message };
            requestMessage.HttpMethod = HttpMethod.Post.ToString();
            requestMessage.ExpectedStatusCode = HttpStatusCode.OK;

            var collection = new RequestMessageCollection();
            collection.Items.Add(requestMessage);

            var mutator = new Mock<Mutator>();
            mutator.Setup(m => m.MutateMessage(It.IsAny<RequestMessage>())).Returns((RequestMessage requestMessageToFuzz) =>
            {
                var fuzzedRequestMessage = requestMessageToFuzz.CloneIntoFuzzedRequestMessage();
                ((RawBody) fuzzedRequestMessage.Body).Text = "asdf 123";

                return new[] { fuzzedRequestMessage };
            });
            collection.AddFuzzedMessages(new[] { mutator.Object }, true);

            var testRequest = new TestRequest();
            testRequest.TimeBetweenRequests = 500;
            testRequest.Items.Add(collection);

            Dispatcher.Dispatch(testRequest);

            Assert.Single(requestMessage.ResponseMessages);
            Assert.Equal(HttpStatusCode.OK, requestMessage.ResponseMessages.Single().StatusCode);

            var fuzzedRequest = collection.Items.OfType<RequestMessageCollection>().Single().Items.OfType<RequestMessage>().Single();

            Assert.Single(fuzzedRequest.ResponseMessages);
        }
    }
}
