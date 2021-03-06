﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class HarSerializer : ICollectionSerializer
    {
        public bool BinaryOnly => false;

        public RequestMessageCollection Read(byte[] bytes) => ReadFromString(Encoding.Unicode.GetString(bytes));

        public RequestMessageCollection ReadFromFile(string fileName) => ReadFromString(File.ReadAllText(fileName));

        public RequestMessageCollection ReadFromString(string collectionText)
        {
            var json = JObject.Parse(collectionText);
            var collection = new RequestMessageCollection();
            collection.Name = "Import from HTTP archive";
            var entries = (JArray) json["log"]["entries"];
            foreach (var harEntry in entries.Cast<JObject>())
            {
                var requestMessage = new RequestMessage();
                requestMessage.HttpMethod = harEntry["request"]["method"].ToString();
                requestMessage.Uri = new Uri(harEntry["request"]["url"].ToString());

                foreach (var harHeader in harEntry["request"]["headers"].Cast<JObject>())
                {
                    var header = new Header();
                    header.Key = harHeader["name"].ToString();
                    header.Value = harHeader["value"].ToString();
                    requestMessage.Headers.Add(header);
                }

                foreach (var harCookie in harEntry["request"]["cookies"].Cast<JObject>())
                {
                    var cookie = new Cookie();
                    cookie.Name = harCookie["name"].ToString();
                    cookie.Value = harCookie["value"].ToString();
                    cookie.Domain = requestMessage.Uri.Host;
                    requestMessage.Cookies.Add(cookie);
                }

                if (harEntry["request"]["postData"] is JObject harPostData)
                {
                    var postMimeType = harPostData["mimeType"].ToString();
                    if (postMimeType.Contains("application/x-www-form-urlencoded") || requestMessage.Headers.Any(header => header.Key == "Content-Type" && header.Value.Contains("application/x-www-form-urlencoded")))
                    {
                        var urlEncodedBody = new FormUrlEncodedBody();
                        foreach (var harParameter in harPostData["params"].Cast<JObject>())
                        {
                            var keyValue = new KeyValue();
                            keyValue.Key = harParameter["name"].ToString();
                            keyValue.Value = harParameter["value"].ToString();
                            urlEncodedBody.FormData.Add(keyValue);
                        }

                        requestMessage.Body = urlEncodedBody;
                    }
                    else
                    {
                        var rawBody = new RawBody();
                        rawBody.Text = harPostData["text"].ToString();

                        requestMessage.Body = rawBody;
                    }
                }
                else
                {
                    requestMessage.Body = new NoBody();
                }

                var responseMessage = new ResponseMessage();
                responseMessage.StatusCode = (HttpStatusCode) harEntry["response"]["status"].ToObject<int>();

                foreach (var harHeader in harEntry["response"]["headers"].Cast<JObject>())
                {
                    var header = new Header();
                    header.Key = harHeader["name"].ToString();
                    header.Value = harHeader["value"].ToString();
                    responseMessage.Headers.Add(header);
                }

                foreach (var harCookie in harEntry["response"]["cookies"].Cast<JObject>())
                {
                    var cookie = new Cookie();
                    cookie.Name = harCookie["name"].ToString();
                    cookie.Value = harCookie["value"].ToString();
                    responseMessage.Cookies.Add(cookie);
                }

                responseMessage.Content = harEntry["response"]["content"]["text"]?.ToString();
                responseMessage.RequestSent = harEntry["startedDateTime"].ToObject<DateTime>();
                responseMessage.Received = responseMessage.RequestSent.AddMilliseconds(harEntry["time"].ToObject<int>());

                requestMessage.ResponseMessages.Add(responseMessage);

                collection.Items.Add(requestMessage);
            }

            return collection;
        }

        public byte[] Write(RequestMessageCollection requestMessageCollection) => Encoding.Unicode.GetBytes(WriteToString(requestMessageCollection));

        public void WriteToFile(RequestMessageCollection requestMessageCollection, string fileName) => File.WriteAllText(fileName, WriteToString(requestMessageCollection), Encoding.Unicode);

        public string WriteToString(RequestMessageCollection requestMessageCollection) => throw new NotSupportedException();
    }
}
