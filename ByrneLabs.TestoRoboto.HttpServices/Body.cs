﻿using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class Body : Entity
    {
        public abstract string Fingerprint { get; }
    }
}
