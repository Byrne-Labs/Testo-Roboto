﻿using ByrneLabs.Commons;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class NoBody : Body, IEntity<NoBody>
    {
        public new NoBody Clone(CloneDepth depth = CloneDepth.Deep) => (NoBody) base.Clone(depth);
    }
}