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
            public override IEnumerable<FuzzedRequestMessage> MutateMessage(RequestMessage requestMessage)
            {
                var mutatedRequestMessage = requestMessage.CloneIntoFuzzedRequestMessage();
                ((RawBody) mutatedRequestMessage.Body).Text = "{ \"xyz\": 456 }";

                return new[] { mutatedRequestMessage };
            }
        }

        [Fact]
        public void TestMutateMessage()
        {
            var requestMessage = new RequestMessage { Body = new RawBody { Text = "{ \"abc\": 123 }" } };

            var mutator = new MockMutator();
            var mutatedRequestMessages = mutator.MutateMessage(requestMessage);

            Assert.Single(mutatedRequestMessages);
            Assert.NotSame(requestMessage, mutatedRequestMessages.Single());
            Assert.NotSame(requestMessage.Body, mutatedRequestMessages.Single().Body);
            Assert.IsType<FuzzedRequestMessage>(mutatedRequestMessages.Single());
            Assert.NotEqual(((RawBody) requestMessage.Body).Text, ((RawBody) mutatedRequestMessages.Single().Body).Text);
        }
    }
}
