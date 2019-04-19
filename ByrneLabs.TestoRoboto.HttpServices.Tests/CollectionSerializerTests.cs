using System;
using System.IO;
using System.Net.Http;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class CollectionSerializerTests
    {
        [Fact]
        public void TestBinaryCollectionSerializer()
        {
            var collection = GetTestCollection();
            var serializer = new BinaryCollectionSerializer();
            var tempFile = Path.GetTempFileName();
            serializer.WriteToFile(collection, tempFile);

            var deserializedCollection = serializer.ReadFromFile(tempFile);
            Assert.Equal(collection, deserializedCollection);

            File.Delete(tempFile);
        }

        private RequestMessageCollection GetTestCollection()
        {
            var collection = new RequestMessageCollection();
            collection.Name = "Some Messages";
            collection.AuthenticationMethod = new BasicAuthentication { Username = "username1", Password = "password1" };
            var requestMessage = new RequestMessage();
            requestMessage.Name = "Some Message";
            requestMessage.AuthenticationMethod = new BasicAuthentication { Username = "username2", Password = "password2" };
            requestMessage.Body = new RawBody { Text = "{ \"asdf\": 123 }" };
            requestMessage.HttpMethod = HttpMethod.Post.ToString();
            requestMessage.Uri = new Uri("http://some.domain/path/resource?key=value");
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            collection.Items.Add(requestMessage);
            var subCollection = new RequestMessageCollection { Name = "Sub-collection" };
            subCollection.AuthenticationMethod = new BasicAuthentication { Username = "username3", Password = "password3" };
            collection.Items.Add(subCollection);
            subCollection.Items.Add(new RequestMessage { HttpMethod = HttpMethod.Post.ToString(), Name = "Some Message", Body = new RawBody { Text = "{ \"xyz\": 456 }" } });
            collection.Items.Add(new RequestMessageCollection { Name = "Fuzzed Messages" });

            return collection;
        }
    }
}
