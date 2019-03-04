using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ByrneLabs.TestoRoboto.Json;

namespace ByrneLabs.TestoRoboto
{
    public class Hammer
    {
        public void Pound(string message, string url, string userName, string password, string failureDirectory)
        {
            using (var httpClient = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(userName + ":" + password);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = httpClient.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json")).Result;

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Unfuzzed message was rejected.");
                }
            }


            var fuzzer = new JsonFuzzer();

            var fuzzedMessages = fuzzer.Fuzz(message);

            Parallel.ForEach(fuzzedMessages, fuzzedMessage =>
            {
                using (var httpClient = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes(userName + ":" + password);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    var response = httpClient.PostAsync(url, new StringContent(fuzzedMessage, Encoding.UTF8, "application/json")).Result;

                    if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600)
                    {
                        File.WriteAllText(Path.Combine(failureDirectory, DateTime.Now.Ticks.ToString()) + ".json", fuzzedMessage);
                    }
                }
            });
        }
    }
}
