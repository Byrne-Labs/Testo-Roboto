using System.Linq;
using ByrneLabs.TestoRoboto.Json.Mutators;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
{
    public class MutatorTest
    {
        protected void TestMessageCountReturned<T>(int expectedMessageCount) where T : JsonMutator, new()
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
            var jsonObject = JObject.Parse(message);

            var mutator = new T();

            var mutatedMessages = mutator.MutateJsonMessage(jsonObject);

            Assert.NotNull(mutatedMessages);
            Assert.NotEmpty(mutatedMessages);
            Assert.Equal(expectedMessageCount, mutatedMessages.Count());

            foreach (var mutatedMessage in mutatedMessages)
            {
                Assert.NotEqual(jsonObject.ToString(), mutatedMessage.ToString());
            }
        }
    }
}
