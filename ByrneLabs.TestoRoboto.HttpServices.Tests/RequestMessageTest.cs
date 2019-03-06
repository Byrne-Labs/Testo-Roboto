using System;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class RequestMessageTest
    {
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
