using System;

namespace ByrneLabs.TestoRoboto.Shopify
{
    class Program
    {
        static void Main(string[] args)
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

            var hammer = new Hammer();
            hammer.Pound(message, "https://testoroboto.myshopify.com/admin/products.json", "36e0780065f769830b0c2cb8dc18fd89", "9e54820bab53b90fa12f1ddfd1528bb1", @"c:\dev\code");
        }
    }
}
