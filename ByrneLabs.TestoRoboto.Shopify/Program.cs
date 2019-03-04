using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ByrneLabs.TestoRoboto.HttpServices;
using ByrneLabs.TestoRoboto.HttpServices.JsonMutators;

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
                        ""option1"": ""Blue"",
                        ""option2"": ""155""
                      },
                      {
                        ""option1"": ""Black"",
                        ""option2"": ""159""
                      }
                    ],
                    ""options"": [
                      {
                        ""name"": ""Color"",
                        ""values"": [
                          ""Blue"",
                          ""Black""
                        ]
                      },
                      {
                        ""name"": ""Size"",
                        ""values"": [
                          ""155"",
                          ""159""
                        ]
                      }
                    ]
                  }
                }";

            var testRequest = new TestRequest();

            testRequest.RequestMessage.RequestUri=new Uri("https://testoroboto.myshopify.com/admin/products.json");
            var byteArray = Encoding.ASCII.GetBytes("36e0780065f769830b0c2cb8dc18fd89:9e54820bab53b90fa12f1ddfd1528bb1");
            testRequest.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            testRequest.RequestMessage.Content = new StringContent(message, Encoding.UTF8, "application/json");
            testRequest.RequestMessage.Method = HttpMethod.Post;
            testRequest.TimeBetweenRequests = 500;
            testRequest.Mutators.Add(new ArrayGrower());
            testRequest.Mutators.Add(new ArrayShrinker());
            testRequest.Mutators.Add(new PropertyAdder());
            testRequest.Mutators.Add(new PropertyRemover());
            testRequest.Mutators.Add(new RandomValueChanger());
            testRequest.Mutators.Add(new SqlInjector());
            testRequest.Mutators.Add(new XmlInjector());

            var dispatcher = new Dispatcher();
            var responses = dispatcher.Dispatch(testRequest);
        }
    }
}
