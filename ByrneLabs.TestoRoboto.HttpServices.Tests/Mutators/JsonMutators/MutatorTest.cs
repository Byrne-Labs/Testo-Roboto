using System.Linq;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators.JsonMutators
{
    public class MutatorTest
    {
        protected void TestMessageCountReturned<T>(int expectedMessageCount) where T : Mutator, new()
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
            var mutatedMessages = mutator.MutateMessage(requestMessage);

            Assert.NotNull(mutatedMessages);
            Assert.NotEmpty(mutatedMessages);
            Assert.Equal(expectedMessageCount, mutatedMessages.Count());

            foreach (var mutatedMessage in mutatedMessages)
            {
                Assert.NotEqual(message, ((RawBody) mutatedMessage.Body).Text);
            }
        }
    }
}
