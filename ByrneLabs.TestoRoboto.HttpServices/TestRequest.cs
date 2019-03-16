using System.Collections.Generic;
using System.Linq;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class TestRequest
    {
        public bool ExcludeUnfuzzableRequests { get; set; } = true;

        public IList<Item> Items { get; set; } = new List<Item>();

        public string LogDirectory { get; set; }

        public bool LogServerErrors { get; set; } = true;

        public IList<Mutator> OnTheFlyMutators { get; set; } = new List<Mutator>();

        public bool RandomizeOrder { get; set; } = true;

        public SessionData SessionData { get; set; } = new SessionData();

        public int TimeBetweenRequests { get; set; }

        public IEnumerable<RequestMessage> GetAllRequestMessages() => Items.OfType<RequestMessage>().Union(Items.OfType<Collection>().SelectMany(collection => collection.DescendentRequestMessages())).ToList();
    }
}
