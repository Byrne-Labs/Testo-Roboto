using System;
using System.Linq;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators
{
    public class MutatorTestBase
    {
        protected void TestFormMessageCountReturned<T>(int expectedMessageCount) where T : Mutator, new()
        {
            var mutator = new T();
            var requestMessage = new RequestMessage();
            var formBody = new FormDataBody();
            formBody.FormData.Add(new KeyValue { Key = "Key1", Value = "Value 1" });
            formBody.FormData.Add(new KeyValue { Key = "Key2", Value = "Value 2" });
            requestMessage.Body = formBody;
            requestMessage.Headers.Add(new Header { Key = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Key2", Value = "Value 2" });
            requestMessage.Uri = new Uri("http://some.domain/path1/path2/resource");
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "Parameter1", Value = "Value1" });
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "Parameter2", Value = "Value2" });

            var mutatedMessages = mutator.MutateMessages(requestMessage);

            Assert.NotNull(mutatedMessages);
            Assert.NotEmpty(mutatedMessages);
            Assert.Equal(expectedMessageCount, mutatedMessages.Count());

            foreach (var mutatedMessage in mutatedMessages)
            {
                Assert.NotEqual(requestMessage, mutatedMessage);
            }
        }

        protected void TestJsonMessageCountReturned<T>(int expectedMessageCount) where T : Mutator, new()
        {
            const string message = @"
                {
                    'level1a': 'asdf',
                    'level1b': 123,
                    'level1c': 
                    {
                        'level2a': 'asdf',
                        'level2b': 123,
                        'level2c': 
                        [
                            {
                                'level3a': 'asdf',
                                'level3b': 123,
                                'level3c':
                                [
                                    {
                                        'level4a': 'asdf',
                                        'level4b': 123
                                    },
                                    {
                                        'level4a': 'asdf',
                                        'level4b': 123
                                    }
                                ]
                            }
                        ]
                    }
                }
            ";

            var mutator = new T();
            var requestMessage = new RequestMessage { Body = new RawBody { Text = message } };
            requestMessage.Headers.Add(new Header { Key = "Key1", Value = "Value 1" });
            requestMessage.Headers.Add(new Header { Key = "Key2", Value = "Value 2" });
            requestMessage.Uri = new Uri("http://some.domain/path1/path2/resource");
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "Parameter1", Value = "Value1" });
            requestMessage.QueryStringParameters.Add(new QueryStringParameter { Key = "Parameter2", Value = "Value2" });

            var mutatedMessages = mutator.MutateMessages(requestMessage);

            Assert.NotNull(mutatedMessages);
            Assert.NotEmpty(mutatedMessages);
            Assert.Equal(expectedMessageCount, mutatedMessages.Count());

            foreach (var mutatedMessage in mutatedMessages)
            {
                Assert.NotEqual(requestMessage, mutatedMessage);
            }
        }
    }
}
