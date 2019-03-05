using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Collection : Item, IEntity<Collection>
    {
        private const string _fuzzedMessageCollectionName = "Fuzzed Messages";

        public List<Item> Items { get; set; } = new List<Item>();

        public static Collection ImportFromPostman(FileInfo file) => throw new NotImplementedException();

        public void AddFuzzedMessages(IEnumerable<Mutator> mutators)
        {
            if (Items.OfType<RequestMessage>().Any())
            {
                Collection fuzzedMessages;
                if (!Items.OfType<Collection>().Any(collection => collection.Name == _fuzzedMessageCollectionName))
                {
                    Items.Add(fuzzedMessages=new Collection { Description = "Fuzzed messages", Name = _fuzzedMessageCollectionName });
                }
                else
                {
                    fuzzedMessages = Items.OfType<Collection>().Single(collection => collection.Name == _fuzzedMessageCollectionName);
                }

                foreach (var nonFuzzedMessage in Items.OfType<RequestMessage>().Where(message => !message.FuzzedMessage))
                {
                    foreach (var mutator in mutators)
                    {
                        foreach(var mutatedMessageContent in mutator.MutateMessage(nonFuzzedMessage.Body))
                        {
                            var fuzzedMessage = nonFuzzedMessage.Clone();
                            fuzzedMessage.FuzzedMessage = true;
                            fuzzedMessage.Body = mutatedMessageContent;
                            fuzzedMessages.Items.Add(fuzzedMessage);
                        }
                    }
                }
            }
        }

        public new Collection Clone(CloneDepth depth = CloneDepth.Deep) => (Collection) base.Clone(depth);

        public void ExportToPostman(FileInfo file) => throw new NotImplementedException();
    }
}
