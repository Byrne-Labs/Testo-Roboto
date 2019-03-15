using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public static class Dispatcher
    {
        public static void Dispatch(TestRequest testRequest)
        {
            var requestMessages = testRequest.Items.OfType<RequestMessage>().Union(testRequest.Items.OfType<Collection>().SelectMany(collection => collection.DescendentRequestMessages())).ToList();

            if (testRequest.ExcludeUnfuzzableRequests)
            {
                RemoveUnfuzzableRequestMessages(requestMessages);
            }

            if (testRequest.TimeBetweenRequests > 0)
            {
                foreach (var requestMessage in requestMessages.Where(r => !(r is FuzzedRequestMessage)))
                {
                    DispatchRequest(requestMessage);
                    if (requestMessage.ExpectedStatusCode != null && requestMessage.ResponseMessages.Last().StatusCode != requestMessage.ExpectedStatusCode)
                    {
                        throw new HttpRequestException($"The unfuzzed message request returned a status code of {requestMessage.ResponseMessages.Last().StatusCode} instead of the expected {requestMessage.ExpectedStatusCode}");
                    }

                    Thread.Sleep(testRequest.TimeBetweenRequests);
                }
            }
            else
            {
                Parallel.ForEach(requestMessages.OfType<FuzzedRequestMessage>(), requestMessage =>
                {
                    DispatchRequest(requestMessage);
                    if (requestMessage.ExpectedStatusCode != null && requestMessage.ResponseMessages.Last().StatusCode != requestMessage.ExpectedStatusCode)
                    {
                        throw new HttpRequestException($"The unfuzzed message request returned a status code of {requestMessage.ResponseMessages.Last().StatusCode} instead of the expected {requestMessage.ExpectedStatusCode}");
                    }
                });
            }

            if (testRequest.TimeBetweenRequests > 0)
            {
                foreach (var requestMessage in requestMessages.OfType<FuzzedRequestMessage>())
                {
                    DispatchRequest(requestMessage);
                    Thread.Sleep(testRequest.TimeBetweenRequests);
                }
            }
            else
            {
                Parallel.ForEach(requestMessages.OfType<FuzzedRequestMessage>(), DispatchRequest);
            }
        }

        private static void DispatchRequest(RequestMessage requestMessage)
        {
            var cookieContainer = new CookieContainer();
            foreach (var cookie in requestMessage.Cookies)
            {
                cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            using (var httpClient = new HttpClient(handler))
            {
                HttpContent httpContent;
                if (requestMessage.Body is RawBody body)
                {
                    var contentType = requestMessage.Headers.SingleOrDefault(header => header.Key == "Content-Type")?.Value ?? "text/plain";
                    httpContent = new StringContent(body.Text, requestMessage.Encoding, contentType);
                }
                else
                {
                    throw new NotSupportedException("Only raw body is currently supported");
                }

                var httpRequestMessage = new HttpRequestMessage
                {
                    Content = httpContent,
                    Method = requestMessage.HttpMethod,
                    RequestUri = requestMessage.Uri
                };

                if (requestMessage.AuthenticationMethod != null && !(requestMessage.AuthenticationMethod is NoAuthentication))
                {
                    if (requestMessage.AuthenticationMethod is BasicAuthentication authentication)
                    {
                        var byteArray = Encoding.ASCII.GetBytes(authentication.Username + ":" + authentication.Password);
                        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else
                    {
                        throw new NotSupportedException("Only basic authentication is currently supported");
                    }
                }

                foreach (var header in requestMessage.Headers)
                {
                    httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                var responseMessage = new ResponseMessage();
                responseMessage.RequestSent = DateTime.Now;

                try
                {
                    var httpResult = httpClient.SendAsync(httpRequestMessage).Result;

                    responseMessage.Received = DateTime.Now;
                    responseMessage.Content = httpResult.Content.ReadAsStringAsync().Result;
                    foreach (var httpHeader in httpResult.Headers)
                    {
                        var header = new Header();
                        header.Key = httpHeader.Key;
                        header.Value = string.Join(", ", httpHeader.Value);
                        responseMessage.Headers.Add(header);
                    }

                    foreach (var httpCookie in cookieContainer.GetCookies(requestMessage.Uri).Cast<System.Net.Cookie>())
                    {
                        var cookie = new Cookie();
                        cookie.Name = httpCookie.Name;
                        cookie.Value = httpCookie.Value;
                        cookie.Domain = httpCookie.Domain;
                        cookie.Path = httpCookie.Path;
                        responseMessage.Cookies.Add(cookie);
                    }

                    requestMessage.ResponseMessages.Add(responseMessage);
                }
                catch
                {
                    //log exception but keep going
                }
            }
        }

        private static void RemoveUnfuzzableRequestMessages(ICollection<RequestMessage> requestMessages)
        {
            var requestMessagesToRemove = requestMessages.Where(requestMessage => !(requestMessage is FuzzedRequestMessage) && !requestMessages.OfType<FuzzedRequestMessage>().Any(fuzzedRequestMessage => ReferenceEquals(fuzzedRequestMessage.SourceRequestMessage, requestMessage))).ToArray();
            foreach (var requestMessageToRemove in requestMessagesToRemove)
            {
                requestMessages.Remove(requestMessageToRemove);
            }
        }
    }
}
