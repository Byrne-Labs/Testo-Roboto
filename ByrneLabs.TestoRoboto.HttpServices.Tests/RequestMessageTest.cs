using System;
using System.Net.Http;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class RequestMessageTest
    {
        [Fact]
        public void TestFormUrlEncodedPostRequestFingerprint()
        {
            var requestMessage = new RequestMessage();
            requestMessage.HttpMethod = HttpMethod.Get.ToString();
            requestMessage.Uri = new Uri("https://some.domain/path1/path2/resource?Key1=Value%201");
            requestMessage.Cookies.Add(new Cookie { Name = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            var body = new FormUrlEncodedBody();
            body.FormData.Add(new KeyValue { Key = "Key1", Value = "Value 1" });
            body.FormData.Add(new KeyValue { Key = "Key2", Value = "Value 2" });
            body.FormData.Add(new KeyValue { Key = "Key2", Value = "Value 3" });
            requestMessage.Body = body;

            var expected =
                @"method: GET
URL: https://some.domain/path1/path2/resource
query string parameters: Key1
headers: Key1, Content-Type
cookies: Key1
body: Key1, Key2, Key2
";
            Assert.Equal(expected, requestMessage.Fingerprint);
        }

        [Fact]
        public void TestGetRequestFingerprint()
        {
            var requestMessage = new RequestMessage();
            requestMessage.HttpMethod = HttpMethod.Get.ToString();
            requestMessage.Uri = new Uri("https://some.domain/path1/path2/resource?Key1=Value%201");
            requestMessage.Cookies.Add(new Cookie { Name = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            var body = new NoBody();
            requestMessage.Body = body;

            var expected =
                @"method: GET
URL: https://some.domain/path1/path2/resource
query string parameters: Key1
headers: Key1, Content-Type
cookies: Key1
";
            Assert.Equal(expected, requestMessage.Fingerprint);
        }

        [Fact]
        public void TestJsonPostRequestFingerprint()
        {
            var requestMessage = new RequestMessage();
            requestMessage.HttpMethod = HttpMethod.Post.ToString();
            requestMessage.Uri = new Uri("https://some.domain/path1/path2/resource?Key1=Value%201");
            requestMessage.Cookies.Add(new Cookie { Name = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/json" });
            var body = new RawBody();
            body.Text = "{ \"prop1\": 123, \"prop2\": [ { \"prop3\": 123 }, { \"prop3\": \"asdf\" } ] }";
            requestMessage.Body = body;

            var expected =
                @"method: POST
URL: https://some.domain/path1/path2/resource
query string parameters: Key1
headers: Key1, Content-Type
cookies: Key1
body: {""prop1"":null,""prop2"":[{""prop3"":null}]}
";
            Assert.Equal(expected, requestMessage.Fingerprint);
        }

        [Fact]
        public void TestRequestMessageAddQueryStringParameters()
        {
            var requestMessage = new RequestMessage();
            requestMessage.Uri = new Uri("https://some.domain.com/patha/pathb/resource.something?");
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "a", Value = "1", Description = "x" });
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "b", Value = "2", Description = "y" });
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "c", Value = "3", Description = "z" });

            Assert.Equal(requestMessage.Uri, new Uri("https://some.domain.com/patha/pathb/resource.something?a=1&b=2&c=3"));
        }

        [Fact]
        public void TestRequestMessageClearQueryStringParameters()
        {
            var requestMessage = new RequestMessage();
            requestMessage.Uri = new Uri("https://some.domain.com/patha/pathb/resource.something?a=1&b=2&c=3");
            requestMessage.QueryStringParameters.Clear();

            Assert.Equal(requestMessage.Uri, new Uri("https://some.domain.com/patha/pathb/resource.something"));
        }

        [Fact]
        public void TestRequestMessageSetUri()
        {
            var requestMessage = new RequestMessage();
            requestMessage.Uri = new Uri("https://some.domain.com/patha/pathb/resource.something?a=1&b=2&c=3");
            Assert.Equal(3, requestMessage.QueryStringParameters.Count);
            Assert.Equal("a", requestMessage.QueryStringParameters[0].Key);
            Assert.Equal("1", requestMessage.QueryStringParameters[0].Value);
            Assert.Null(requestMessage.QueryStringParameters[0].Description);
            Assert.Equal("b", requestMessage.QueryStringParameters[1].Key);
            Assert.Equal("2", requestMessage.QueryStringParameters[1].Value);
            Assert.Null(requestMessage.QueryStringParameters[1].Description);
            Assert.Equal("c", requestMessage.QueryStringParameters[2].Key);
            Assert.Equal("3", requestMessage.QueryStringParameters[2].Value);
            Assert.Null(requestMessage.QueryStringParameters[2].Description);
            requestMessage.Uri = new Uri("https://some.domain.com/patha/pathb/resource.something?");
            Assert.Empty(requestMessage.QueryStringParameters);
        }
    }
}
