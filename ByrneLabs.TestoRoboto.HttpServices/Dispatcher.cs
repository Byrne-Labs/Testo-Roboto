using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Dispatcher
    {
        public IEnumerable<HttpResponseMessage> Dispatch(TestRequest testRequest)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.SendAsync(testRequest.RequestMessage).Result;

                response.EnsureSuccessStatusCode();
            }

            var fuzzedMessages = testRequest.Mutators.SelectMany(mutator => mutator.MutateMessage(testRequest.RequestMessage.Content.ReadAsStringAsync().Result)).ToArray();
            var responses = new List<HttpResponseMessage>();
            foreach (var fuzzedMessage in fuzzedMessages)
            {
                using (var httpClient = new HttpClient())
                {
                    var fuzzedRequest = new HttpRequestMessage
                    {
                        Content = new StringContent(fuzzedMessage, testRequest.Encoding),
                        Method = testRequest.RequestMessage.Method,
                        RequestUri = testRequest.RequestMessage.RequestUri,
                        Version = testRequest.RequestMessage.Version
                    };
                    foreach (var header in testRequest.RequestMessage.Headers)
                    {
                        fuzzedRequest.Headers.Add(header.Key, header.Value);
                    }

                    foreach (var property in testRequest.RequestMessage.Properties)
                    {
                        fuzzedRequest.Properties.Add(property);
                    }

                    responses.Add(httpClient.SendAsync(fuzzedRequest).Result);
                }

                Thread.Sleep(testRequest.TimeBetweenRequests);
            }

            return responses;
        }
    }
}
