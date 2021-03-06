﻿using ByrneLabs.Commons;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    public class NoBody : Body, ICloneable<NoBody>
    {
        [IgnoreMember]
        public override string Fingerprint => string.Empty;

        public new NoBody Clone(CloneDepth depth = CloneDepth.Deep) => (NoBody) base.Clone(depth);
    }
}
