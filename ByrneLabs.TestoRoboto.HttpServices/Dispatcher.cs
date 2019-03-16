using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public static class Dispatcher
    {
        public static IEnumerable<ResponseMessage> Dispatch(TestRequest testRequest)
        {
            if (!string.IsNullOrWhiteSpace(testRequest.LogDirectory))
            {
                Directory.CreateDirectory(testRequest.LogDirectory);
            }
            var requestMessages = testRequest.GetAllRequestMessages().ToList();
            if (testRequest.RandomizeOrder)
            {
                requestMessages = requestMessages.OrderBy(x => BetterRandom.Next()).ToList();
            }

            if (testRequest.ExcludeUnfuzzableRequests && !testRequest.OnTheFlyMutators.Any())
            {
                RemoveUnfuzzableRequestMessages(requestMessages);
            }

            var responseMessages = new List<ResponseMessage>();

            if (testRequest.TimeBetweenRequests > 0)
            {
                foreach (var requestMessage in requestMessages.Where(r => !(r is FuzzedRequestMessage)))
                {
                    var responseMessage = DispatchRequest(requestMessage, testRequest.SessionData, testRequest.LogServerErrors, testRequest.LogDirectory);
                    responseMessages.Add(responseMessage);
                    if (requestMessage.ExpectedStatusCode != null && responseMessage.StatusCode != requestMessage.ExpectedStatusCode)
                    {
                        throw new HttpRequestException($"The unfuzzed message request returned a status code of {responseMessage.StatusCode} instead of the expected {requestMessage.ExpectedStatusCode}");
                    }

                    Thread.Sleep(testRequest.TimeBetweenRequests);
                }
            }
            else
            {
                Parallel.ForEach(requestMessages.OfType<FuzzedRequestMessage>(), requestMessage =>
                {
                    var responseMessage = DispatchRequest(requestMessage, testRequest.SessionData, testRequest.LogServerErrors, testRequest.LogDirectory);
                    responseMessages.Add(responseMessage);
                    if (requestMessage.ExpectedStatusCode != null && responseMessage.StatusCode != requestMessage.ExpectedStatusCode)
                    {
                        throw new HttpRequestException($"The unfuzzed message request returned a status code of {responseMessage.StatusCode} instead of the expected {requestMessage.ExpectedStatusCode}");
                    }
                });
            }

            if (testRequest.TimeBetweenRequests > 0)
            {
                foreach (var requestMessage in requestMessages.OfType<FuzzedRequestMessage>())
                {
                    var responseMessage = DispatchRequest(requestMessage, testRequest.SessionData, testRequest.LogServerErrors, testRequest.LogDirectory);
                    responseMessages.Add(responseMessage);
                    Thread.Sleep(testRequest.TimeBetweenRequests);
                }

                foreach (var mutator in testRequest.OnTheFlyMutators)
                {
                    foreach (var requestMessage in requestMessages.Where(r => !(r is FuzzedRequestMessage)))
                    {
                        foreach (var mutatedRequestMessage in mutator.MutateMessage(requestMessage))
                        {
                            var responseMessage = DispatchRequest(mutatedRequestMessage, testRequest.SessionData, testRequest.LogServerErrors, testRequest.LogDirectory);
                            responseMessages.Add(responseMessage);
                            Thread.Sleep(testRequest.TimeBetweenRequests);
                        }
                    }
                }
            }
            else
            {
                Parallel.ForEach(requestMessages.OfType<FuzzedRequestMessage>(), fuzzedRequestMessage =>
                {
                    var responseMessage = DispatchRequest(fuzzedRequestMessage, testRequest.SessionData, testRequest.LogServerErrors, testRequest.LogDirectory);
                    responseMessages.Add(responseMessage);
                });
                Parallel.ForEach(testRequest.OnTheFlyMutators, mutator =>
                {
                    Parallel.ForEach(requestMessages.Where(r => !(r is FuzzedRequestMessage)), requestMessage =>
                    {
                        Parallel.ForEach(mutator.MutateMessage(requestMessage), mutatedRequestMessage =>
                        {
                            var responseMessage = DispatchRequest(mutatedRequestMessage, testRequest.SessionData, testRequest.LogServerErrors, testRequest.LogDirectory);
                            responseMessages.Add(responseMessage);
                        });
                    });
                });
            }

            return responseMessages;
        }

        public static ResponseMessage Dispatch(RequestMessage requestMessage) => DispatchRequest(requestMessage, new SessionData(), false, string.Empty);

        private static ResponseMessage DispatchRequest(RequestMessage requestMessage, SessionData sessionData, bool logServerError, string logDirectory)
        {
            var cookieContainer = new CookieContainer();
            foreach (var cookie in requestMessage.Cookies)
            {
                if (!sessionData.Cookies.Any(sc => sc.Name == cookie.Name && sc.Domain == cookie.Domain))
                {
                    cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                }
            }

            foreach (var sessionCookie in sessionData.Cookies)
            {
                cookieContainer.Add(new System.Net.Cookie(sessionCookie.Name, sessionCookie.Value, sessionCookie.Path, sessionCookie.Domain));
            }

            ResponseMessage responseMessage;

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            using (var httpClient = new HttpClient(handler))
            {
                HttpContent httpContent;
                if (requestMessage.Body is RawBody rawBody)
                {
                    var contentType = requestMessage.Headers.SingleOrDefault(header => header.Key == "Content-Type")?.Value ?? "text/plain";
                    httpContent = new StringContent(rawBody.Text, requestMessage.Encoding, contentType);
                }
                else if (requestMessage.Body is FormUrlEncodedBody formUrlEncodedBody)
                {
                    var parameters = formUrlEncodedBody.FormData.Select(parameter => new KeyValuePair<string, string>(parameter.Key, parameter.Value)).ToList();
                    httpContent = new FormUrlEncodedContent(parameters);
                }
                else
                {
                    throw new NotSupportedException("Only raw and form URL encoded bodies are currently supported");
                }

                var uri = requestMessage.Uri;

                foreach (var sessionQueryStringParameter in sessionData.QueryStringParameters)
                {
                    uri = uri.AddQueryParameter(sessionQueryStringParameter.Key, sessionQueryStringParameter.Value);
                }

                var httpRequestMessage = new HttpRequestMessage
                {
                    Content = httpContent,
                    Method = requestMessage.HttpMethod,
                    RequestUri = uri
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
                    if (!sessionData.Headers.Any(sh => sh.Key == header.Key))
                    {
                        httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                foreach (var sessionHeader in sessionData.Headers)
                {
                    httpRequestMessage.Headers.TryAddWithoutValidation(sessionHeader.Key, sessionHeader.Value);
                }

                responseMessage = new ResponseMessage();
                responseMessage.RequestSent = DateTime.Now;

                try
                {
                    var httpResult = httpClient.SendAsync(httpRequestMessage).Result;

                    responseMessage.Received = DateTime.Now;
                    responseMessage.StatusCode = httpResult.StatusCode;
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

                    if (logServerError && (int) responseMessage.StatusCode >= 500 && (int) responseMessage.StatusCode < 600)
                    {
                        responseMessage.EntityId = Guid.NewGuid();
                        var failedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                        failedRequestMessage.SourceRequestMessage = null;
                        failedRequestMessage.ResponseMessages.Clear();
                        failedRequestMessage.ResponseMessages.Add(responseMessage.Clone());
                        var jsonFailedRequestMessage = JObject.FromObject(failedRequestMessage);
                        File.WriteAllText($"{logDirectory}/error-{responseMessage.EntityId.ToString()}.json", jsonFailedRequestMessage.ToString());
                    }
                }
                catch (Exception exception)
                {
                    responseMessage.Exception = exception;
                    //log exception but keep going
                }
            }

            return responseMessage;
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
