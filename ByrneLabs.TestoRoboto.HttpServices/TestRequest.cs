﻿using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class TestRequest : HandyObject<TestRequest>
    {
        [Key(0)]
        public bool ExcludeUnfuzzableRequests { get; set; } = true;

        [Key(1)]
        public List<RequestMessageHierarchyItem> Items { get; } = new List<RequestMessageHierarchyItem>();

        [Key(2)]
        public string LogDirectory { get; set; }

        [Key(3)]
        public bool LogServerErrors { get; set; } = true;

        [Key(4)]
        public List<Mutator> OnTheFlyMutators { get; } = new List<Mutator>();

        [Key(5)]
        public bool RandomizeOrder { get; set; } = true;

        [Key(6)]
        public List<string> ResponseErrorsToIgnore { get; } = new List<string>();

        [Key(7)]
        public SessionData SessionData { get; set; } = new SessionData();

        [Key(8)]
        public int TimeBetweenRequests { get; set; }

        public IEnumerable<RequestMessage> GetAllRequestMessages() => Items.OfType<RequestMessage>().Union(Items.OfType<RequestMessageCollection>().SelectMany(collection => collection.DescendentRequestMessages())).ToList();
    }
}
