﻿using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    [PublicAPI]
    public class KeyValue : HandyObject<KeyValue>
    {
        [Key(0)]
        public string Description { get; set; }

        [Key(1)]
        public string Key { get; set; }

        [Key(2)]
        public string Value { get; set; }
    }
}
