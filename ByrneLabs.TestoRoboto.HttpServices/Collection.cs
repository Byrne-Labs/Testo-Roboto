using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Collection : Item, ICloneable<Collection>
    {
        public List<Item> Items { get; } = new List<Item>();

        private bool FuzzedMessageCollection { get; set; }

        public void AddFuzzedMessages(IEnumerable<Mutator> mutators, bool includeSubCollections)
        {
            if (Items.OfType<RequestMessage>().Any())
            {
                Collection fuzzedMessages;
                if (Items.OfType<Collection>().All(collection => collection.FuzzedMessageCollection))
                {
                    Items.Add(fuzzedMessages = new Collection { FuzzedMessageCollection = true, Description = "Fuzzed messages", Name = "Fuzzed Messages" });
                }
                else
                {
                    fuzzedMessages = Items.OfType<Collection>().Single(collection => collection.FuzzedMessageCollection);
                }

                foreach (var nonFuzzedMessage in Items.OfType<RequestMessage>().Where(message => !(message is FuzzedRequestMessage)))
                {
                    foreach (var mutator in mutators)
                    {
                        fuzzedMessages.Items.AddRange(mutator.MutateMessage(nonFuzzedMessage));
                    }
                }

                if (includeSubCollections)
                {
                    foreach (var subCollection in Items.OfType<Collection>().Where(c => !c.FuzzedMessageCollection))
                    {
                        subCollection.AddFuzzedMessages(mutators, true);
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

        public void RemoveDuplicateFingerprints(bool includeSubCollections)
        {
            var fingerprints = new List<string>();
            var duplicateRequestMessages = new List<RequestMessage>();
            foreach (var requestMessage in Items.OfType<RequestMessage>().Where(rm => !(rm is FuzzedRequestMessage)))
            {
                if (!fingerprints.Contains(requestMessage.Fingerprint))
                {
                    fingerprints.Add(requestMessage.Fingerprint);
                }
                else
                {
                    duplicateRequestMessages.Add(requestMessage);
                }
            }

            foreach (var duplicateRequestMessage in duplicateRequestMessages)
            {
                Items.Remove(duplicateRequestMessage);
            }

            var fuzzedMessages = Items.OfType<Collection>().SingleOrDefault(collection => collection.FuzzedMessageCollection);
            if (fuzzedMessages != null)
            {
                var duplicatedFuzzedRequestMessages = new List<FuzzedRequestMessage>();
                foreach (var fuzzedRequestMessage in fuzzedMessages.Items.OfType<FuzzedRequestMessage>())
                {
                    if (duplicateRequestMessages.Contains(fuzzedRequestMessage.SourceRequestMessage))
                    {
                        duplicatedFuzzedRequestMessages.Add(fuzzedRequestMessage);
                    }
                }

                foreach (var duplicatedFuzzedRequestMessage in duplicatedFuzzedRequestMessages)
                {
                    fuzzedMessages.Items.Remove(duplicatedFuzzedRequestMessage);
                }
            }

            if (includeSubCollections)
            {
                foreach (var subCollection in Items.OfType<Collection>().Where(sc => !sc.FuzzedMessageCollection))
                {
                    subCollection.RemoveDuplicateFingerprints(includeSubCollections);
                }
            }
        }

        public override bool Validate() => base.Validate() && Items.All(item => item.Validate());
    }
}
