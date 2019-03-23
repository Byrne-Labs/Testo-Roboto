﻿using System;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public abstract class Item : HandyObject<Item>
    {
        public AuthenticationMethod AuthenticationMethod { get; set; } = new NoAuthentication();

        public string Description { get; set; }

        public string Name { get; set; }

        public virtual void AssertValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException($"The {nameof(Name)} property cannot be null or whitespace");
            }
        }

        public virtual bool Validate() => !string.IsNullOrWhiteSpace(Name);
    }
}
