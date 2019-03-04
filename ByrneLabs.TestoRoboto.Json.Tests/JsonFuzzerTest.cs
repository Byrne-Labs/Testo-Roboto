using System.Linq;
using Xunit;

namespace ByrneLabs.TestoRoboto.Json.Tests
{
    public class JsonFuzzerTest
    {
        [Fact]
        public void TestJsonFuzzerTest()
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
            var fuzzer = new JsonFuzzer();

            var mutatedMessages = fuzzer.Fuzz(message);

            Assert.NotNull(mutatedMessages);
            Assert.NotEmpty(mutatedMessages);
            Assert.Equal(6134, mutatedMessages.Count());

            foreach (var mutatedMessage in mutatedMessages)
            {
                Assert.NotEqual(message, mutatedMessage);
            }
        }
    }
}
