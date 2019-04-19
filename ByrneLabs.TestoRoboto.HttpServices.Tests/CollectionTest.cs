using System;
using System.Linq;
using System.Net.Http;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
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
            var collection = new RequestMessageCollection();
            var requestMessage = new RequestMessage();
            requestMessage.Body = new RawBody { Text = "{ \"asdf\": 123 }" };
            requestMessage.HttpMethod = HttpMethod.Post.ToString();
            requestMessage.Uri = new Uri("http://some.domain/path/resource?key=value");
            collection.Items.Add(requestMessage);
            var subCollection = new RequestMessageCollection { Name = "Sub-collection" };
            collection.Items.Add(subCollection);
            subCollection.Items.Add(new RequestMessage { Body = new RawBody { Text = "{ \"xyz\": 456 }" } });

            var mutator = new Mock<Mutator>();
            mutator.Setup(m => m.MutateMessage(It.IsAny<RequestMessage>())).Returns((RequestMessage requestMessageToFuzz) =>
            {
                var fuzzedRequestMessage = requestMessageToFuzz.CloneIntoFuzzedRequestMessage();
                var jObject = JObject.Parse(((RawBody) fuzzedRequestMessage.Body).Text);
                foreach (var value in jObject.Descendants().OfType<JValue>())
                {
                    value.Value = "asdf";
                }

                ((RawBody) fuzzedRequestMessage.Body).Text = jObject.ToString(Formatting.None);

                return new[] { fuzzedRequestMessage };
            });
            collection.AddFuzzedMessages(new[] { mutator.Object }, true);

            Assert.Equal(3, collection.Items.Count);
            Assert.Single(collection.Items.OfType<RequestMessage>());
            Assert.Equal(2, collection.Items.OfType<RequestMessageCollection>().Count());
            Assert.Single(collection.Items.OfType<RequestMessageCollection>().First().Items.OfType<RequestMessage>());
            Assert.IsType<FuzzedRequestMessage>(collection.Items.OfType<RequestMessageCollection>().Single(sc => sc.FuzzedMessageCollection).Items.OfType<RequestMessage>().Single());
            Assert.Equal("{\"asdf\":\"asdf\"}", ((RawBody) collection.Items.OfType<RequestMessageCollection>().Single(sc => sc.FuzzedMessageCollection).Items.OfType<RequestMessage>().Single().Body).Text);

            Assert.Equal(2, subCollection.Items.Count);
            Assert.Single(subCollection.Items.OfType<RequestMessage>());
            Assert.Single(subCollection.Items.OfType<RequestMessageCollection>());
            Assert.Single(subCollection.Items.OfType<RequestMessageCollection>().First().Items.OfType<RequestMessage>());
            Assert.IsType<FuzzedRequestMessage>(subCollection.Items.OfType<RequestMessageCollection>().Single().Items.OfType<RequestMessage>().Single());
            Assert.Equal("{\"xyz\":\"asdf\"}", ((RawBody) subCollection.Items.OfType<RequestMessageCollection>().Single().Items.OfType<RequestMessage>().Single().Body).Text);
        }
    }
}
