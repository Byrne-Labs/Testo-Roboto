using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using ByrneLabs.TestoRoboto.HttpServices;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using ByrneLabs.TestoRoboto.HttpServices.Mutators.JsonMutators;

namespace ByrneLabs.TestoRoboto.Shopify
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var message = @"
                {
                    ""product"": {
                    ""title"": ""Burton Custom Freestyle 151"",
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
            requestMessage.Name = "Create New Product";
            requestMessage.Uri = new Uri("https://testoroboto.myshopify.com/admin/products.json");
            requestMessage.AuthenticationMethod = new BasicAuthentication { Username = "36e0780065f769830b0c2cb8dc18fd89", Password = "9e54820bab53b90fa12f1ddfd1528bb1" };
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            requestMessage.Body = new RawBody { Text = message };
            requestMessage.HttpMethod = HttpMethod.Post;

            var collection = new Collection();
            collection.Name = "Shopify";
            collection.Items.Add(requestMessage);
            collection.AddFuzzedMessages(new Mutator[]
            {
                new ArrayGrower(),
                new ArrayShrinker(),
                new PropertyAdder(),
                new PropertyRemover(),
                new JavaScriptInjector(),
                new RandomValueChanger(),
                new SqlInjector(),
                new XmlInjector()
            }, true);

            PostmanImporterExporter.ExportToPostman(collection, new FileInfo("Shopify Fuzzed.postman_collection.json"));

            var testRequest = new TestRequest();
            testRequest.TimeBetweenRequests = 500;

            var dispatcher = new Dispatcher();
            dispatcher.Dispatch(testRequest);

            var failures = collection.DescendentRequestMessages().Where(request => (int)request.ResponseMessages.First().StatusCode >= 500 && (int)request.ResponseMessages.First().StatusCode < 600).ToList();
        }
    }
}
