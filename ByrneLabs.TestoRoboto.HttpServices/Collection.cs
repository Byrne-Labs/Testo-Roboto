using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Collection : Item, IEntity<Collection>
    {
        private const string _fuzzedMessageCollectionName = "Fuzzed Messages";

        public List<Item> Items { get; } = new List<Item>();

        public void AddFuzzedMessages(IEnumerable<Mutator> mutators, bool includeSubCollections)
        {
            if (Items.OfType<RequestMessage>().Any())
            {
                Collection fuzzedMessages;
                if (Items.OfType<Collection>().All(collection => collection.Name != _fuzzedMessageCollectionName))
                {
                    Items.Add(fuzzedMessages = new Collection { Description = "Fuzzed messages", Name = _fuzzedMessageCollectionName });
                }
                else
                {
                    fuzzedMessages = Items.OfType<Collection>().Single(collection => collection.Name == _fuzzedMessageCollectionName);
                }

                foreach (var nonFuzzedMessage in Items.OfType<RequestMessage>().Where(message => !message.FuzzedMessage))
                {
                    foreach (var mutator in mutators)
                    {
                        fuzzedMessages.Items.AddRange(mutator.MutateMessages(nonFuzzedMessage));
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

        public override void AssertValid()
        {
            base.AssertValid();
            foreach (var item in Items)
            {
                item.AssertValid();
            }
        }

        public new Collection Clone(CloneDepth depth = CloneDepth.Deep) => (Collection) base.Clone(depth);

        public IEnumerable<RequestMessage> DescendentRequestMessages() => Items.OfType<RequestMessage>().Union(Items.OfType<Collection>().SelectMany(collection => collection.DescendentRequestMessages()));

        public override bool Validate() => base.Validate() && Items.All(item => item.Validate());
    }
}
