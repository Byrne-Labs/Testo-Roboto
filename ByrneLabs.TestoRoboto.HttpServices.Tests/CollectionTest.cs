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
    }
}
