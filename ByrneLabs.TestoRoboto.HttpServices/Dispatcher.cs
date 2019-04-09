using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Dispatcher
    {
        private static int _errorNumber = 1;
        private readonly TestRequest _testRequest;

        private Dispatcher(TestRequest testRequest)
        {
            _testRequest = testRequest;
        }

        public static IEnumerable<ResponseMessage> Dispatch(TestRequest testRequest) => new Dispatcher(testRequest).Dispatch();

        public static ResponseMessage Dispatch(RequestMessage requestMessage) => new Dispatcher(new TestRequest { Items = { requestMessage }, LogServerErrors = false, ExcludeUnfuzzableRequests = false }).Dispatch().Single();

        private static void RemoveUnfuzzableRequestMessages(ICollection<RequestMessage> requestMessages)
        {
            var requestMessagesToRemove = requestMessages.Where(requestMessage => !(requestMessage is FuzzedRequestMessage) && !requestMessages.OfType<FuzzedRequestMessage>().Any(fuzzedRequestMessage => ReferenceEquals(fuzzedRequestMessage.SourceRequestMessage, requestMessage))).ToArray();
            foreach (var requestMessageToRemove in requestMessagesToRemove)
            {
                requestMessages.Remove(requestMessageToRemove);
            }
        }

        private IEnumerable<ResponseMessage> Dispatch()
        {
            if (!string.IsNullOrWhiteSpace(_testRequest.LogDirectory))
            {
                Directory.CreateDirectory(_testRequest.LogDirectory);
            }

            var requestMessages = _testRequest.GetAllRequestMessages().OrderBy(x => _testRequest.RandomizeOrder ? BetterRandom.Next() : 1).ToList();

            if (_testRequest.ExcludeUnfuzzableRequests && !_testRequest.OnTheFlyMutators.Any())
            {
                RemoveUnfuzzableRequestMessages(requestMessages);
            }

            var responseMessages = new List<ResponseMessage>();

            if (_testRequest.TimeBetweenRequests > 0)
            {
                foreach (var requestMessage in requestMessages.Where(r => !(r is FuzzedRequestMessage)))
                {
                    var responseMessage = DispatchRequest(requestMessage);
                    responseMessages.Add(responseMessage);
                    if (requestMessage.ExpectedStatusCode != null && responseMessage.StatusCode != requestMessage.ExpectedStatusCode)
                    {
                        throw new HttpRequestException($"The unfuzzed message request returned a status code of {responseMessage.StatusCode} instead of the expected {requestMessage.ExpectedStatusCode}");
                    }

                    Thread.Sleep(_testRequest.TimeBetweenRequests);
                }
            }
            else
            {
                Parallel.ForEach(requestMessages.Where(r => !(r is FuzzedRequestMessage)), requestMessage =>
                {
                    var responseMessage = DispatchRequest(requestMessage);
                    responseMessages.Add(responseMessage);
                    if (requestMessage.ExpectedStatusCode != null && responseMessage.StatusCode != requestMessage.ExpectedStatusCode)
                    {
                        throw new HttpRequestException($"The unfuzzed message request returned a status code of {responseMessage.StatusCode} instead of the expected {requestMessage.ExpectedStatusCode}");
                    }
                });
            }

            if (_testRequest.TimeBetweenRequests > 0)
            {
                foreach (var requestMessage in requestMessages.OfType<FuzzedRequestMessage>())
                {
                    var responseMessage = DispatchRequest(requestMessage);
                    responseMessages.Add(responseMessage);
                    Thread.Sleep(_testRequest.TimeBetweenRequests);
                }

                foreach (var requestMessage in requestMessages.Where(r => !(r is FuzzedRequestMessage)))
                {
                    foreach (var mutator in _testRequest.OnTheFlyMutators.OrderBy(x => _testRequest.RandomizeOrder ? BetterRandom.Next() : 1))
                    {
                        foreach (var mutatedRequestMessage in mutator.MutateMessage(requestMessage).OrderBy(x => _testRequest.RandomizeOrder ? BetterRandom.Next() : 1))
                        {
                            var responseMessage = DispatchRequest(mutatedRequestMessage);
                            responseMessages.Add(responseMessage);
                            Thread.Sleep(_testRequest.TimeBetweenRequests);
                        }
                    }
                }
            }
            else
            {
                Parallel.ForEach(requestMessages.OfType<FuzzedRequestMessage>().OrderBy(x => _testRequest.RandomizeOrder ? BetterRandom.Next() : 1), fuzzedRequestMessage =>
                {
                    var responseMessage = DispatchRequest(fuzzedRequestMessage);
                    responseMessages.Add(responseMessage);
                });
                Parallel.ForEach(requestMessages.Where(r => !(r is FuzzedRequestMessage)), requestMessage =>
                {
                    Parallel.ForEach(_testRequest.OnTheFlyMutators.OrderBy(x => _testRequest.RandomizeOrder ? BetterRandom.Next() : 1), mutator =>
                    {
                        Parallel.ForEach(mutator.MutateMessage(requestMessage).OrderBy(x => _testRequest.RandomizeOrder ? BetterRandom.Next() : 1), mutatedRequestMessage =>
                        {
                            var responseMessage = DispatchRequest(mutatedRequestMessage);
                            responseMessages.Add(responseMessage);
                        });
                    });
                });
            }

            return responseMessages;
        }

        private ResponseMessage DispatchRequest(RequestMessage requestMessage)
        {
            var cookieContainer = new CookieContainer();
            foreach (var cookie in requestMessage.Cookies)
            {
                if (!_testRequest.SessionData.Cookies.Any(sc => sc.Name == cookie.Name && sc.Domain == cookie.Domain))
                {
                    try
                    {
                        cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                    }
                    catch (CookieException)
                    {
                        //oh well, let's try the next cookie
                    }
                }
            }

            foreach (var sessionCookie in _testRequest.SessionData.Cookies)
            {
                cookieContainer.Add(new System.Net.Cookie(sessionCookie.Name, sessionCookie.Value, sessionCookie.Path, sessionCookie.Domain));
            }

            ResponseMessage responseMessage;

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
            using (var httpClient = new HttpClient(handler))
            {
                HttpContent httpContent;
                if (requestMessage.Body is RawBody rawBody)
                {
                    var contentType = requestMessage.Headers.SingleOrDefault(header => header.Key == "Content-Type")?.Value ?? "text/plain";
                    httpContent = new StringContent(rawBody.Text, Encoding.GetEncoding(requestMessage.Encoding), contentType);
                }
                else if (requestMessage.Body is FormUrlEncodedBody formUrlEncodedBody)
                {
                    var parameters = formUrlEncodedBody.FormData.Select(parameter => new KeyValuePair<string, string>(parameter.Key, parameter.Value)).ToList();
                    httpContent = new FormUrlEncodedContent(parameters);
                }
                else if (requestMessage.Body is NoBody || requestMessage.Body == null)
                {
                    httpContent = null;
                }
                else
                {
                    throw new NotSupportedException("Only raw and form URL encoded bodies are currently supported");
                }

                var uri = requestMessage.Uri;

                foreach (var sessionQueryStringParameter in _testRequest.SessionData.QueryStringParameters)
                {
                    uri = uri.AddQueryParameter(sessionQueryStringParameter.Key, sessionQueryStringParameter.Value);
                }

                var httpRequestMessage = new HttpRequestMessage
                {
                    Content = httpContent,
                    Method = HttpTools.HttpMethodFromString(requestMessage.HttpMethod),
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
                    if (_testRequest.SessionData.Headers.All(sh => sh.Key != header.Key))
                    {
                        httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                foreach (var sessionHeader in _testRequest.SessionData.Headers)
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

                    LogResponse(requestMessage, responseMessage);
                }
                catch (Exception exception)
                {
                    responseMessage.Exception = exception;
                    //log exception but keep going
                }
            }

            return responseMessage;
        }

        private void LogResponse(RequestMessage requestMessage, ResponseMessage responseMessage)
        {
            if (!_testRequest.LogServerErrors || (int) responseMessage.StatusCode >= 600 || (int) responseMessage.StatusCode < 500 || _testRequest.ResponseErrorsToIgnore.Any(ignore => Regex.IsMatch(responseMessage.Content, ignore)))
            {
                return;
            }

            var failedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
            failedRequestMessage.SourceRequestMessage = null;
            failedRequestMessage.ResponseMessages.Clear();
            failedRequestMessage.ResponseMessages.Add(responseMessage.Clone());
            var jsonFailedRequestMessage = JObject.FromObject(failedRequestMessage);
            File.WriteAllText($"{_testRequest.LogDirectory}/error-{_errorNumber++}.json", jsonFailedRequestMessage.ToString());
        }
    }
}
