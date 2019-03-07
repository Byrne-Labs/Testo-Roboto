using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Collection : Item, IEntity<Collection>
    {
        private const string _fuzzedMessageCollectionName = "Fuzzed Messages";

        public IList<Item> Items { get; } = new List<Item>();

        public void AddFuzzedMessages(IEnumerable<Mutator> mutators, bool includeSubCollections)
        {
            if (Items.OfType<RequestMessage>().Any())
            {
                Collection fuzzedMessages;
                if (!Items.OfType<Collection>().Any(collection => collection.Name == _fuzzedMessageCollectionName))
                {
                    Items.Add(fuzzedMessages = new Collection { Description = "Fuzzed messages", Name = _fuzzedMessageCollectionName });
                }
                else
                {
                    fuzzedMessages = Items.OfType<Collection>().Single(collection => collection.Name == _fuzzedMessageCollectionName);
                }

                foreach (var nonFuzzedMessage in Items.OfType<RequestMessage>().Where(message => !message.FuzzedMessage))
                {
                    if (nonFuzzedMessage.Body is RawBody)
                    {
                        if (!string.IsNullOrWhiteSpace(((RawBody) nonFuzzedMessage.Body).Text))
                        {
                            foreach (var mutator in mutators)
                            {
                                foreach (var mutatedMessageContent in mutator.MutateMessage(((RawBody) nonFuzzedMessage.Body).Text))
                                {
                                    var fuzzedMessage = nonFuzzedMessage.Clone();
                                    fuzzedMessage.FuzzedMessage = true;
                                    fuzzedMessage.ExpectedStatusCode = null;
                                    fuzzedMessage.Body = new RawBody { Text = mutatedMessageContent };
                                    fuzzedMessages.Items.Add(fuzzedMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("Only raw body is currently supported for fuzzing");
                    }
                }

                if (includeSubCollections)
                {
                    foreach (var subcollection in Items.OfType<Collection>().Where(c => c.Name != _fuzzedMessageCollectionName))
                    {
                        subcollection.AddFuzzedMessages(mutators, true);
                    }
                }
            }
        }

        public new Collection Clone(CloneDepth depth = CloneDepth.Deep) => (Collection) base.Clone(depth);

        public IEnumerable<RequestMessage> DescendentRequestMessages() => Items.OfType<RequestMessage>().Union(Items.OfType<Collection>().SelectMany(collection => collection.DescendentRequestMessages()));
    }
}
