using System.Collections.Generic;
using System.Linq;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests.Mutators
{
    public class MutatorTest
    {
        private class MockMutator : Mutator
        {
            protected override IEnumerable<RequestMessage> MutateMessage(RequestMessage requestMessage)
            {
                var mutatedRequestMessage = requestMessage.Clone();
                ((RawBody) mutatedRequestMessage.Body).Text = "{ \"xyz\": 456 }";

                return new[] { mutatedRequestMessage };
            }
        }

        [Fact]
        public void TestMutateMessage()
        {
            var requestMessage = new RequestMessage { Body = new RawBody { Text = "{ \"abc\": 123 }" } };

            var mutator = new MockMutator();
            var mutatedRequestMessages = mutator.MutateMessages(requestMessage);

            Assert.Single(mutatedRequestMessages);
            Assert.NotSame(requestMessage, mutatedRequestMessages.Single());
            Assert.NotSame(requestMessage.Body, mutatedRequestMessages.Single().Body);
            Assert.True(mutatedRequestMessages.Single().FuzzedMessage);
            Assert.NotEqual(((RawBody) requestMessage.Body).Text, ((RawBody) mutatedRequestMessages.Single().Body).Text);
        }
    }
}
