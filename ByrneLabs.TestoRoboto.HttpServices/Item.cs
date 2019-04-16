using System;
using ByrneLabs.Commons;
using JetBrains.Annotations;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    [MessagePackObject]
    
    [Union(0, typeof(Collection))]
    [Union(1, typeof(FuzzedRequestMessage))]
    [Union(2, typeof(RequestMessage))]
    public abstract class Item : HandyObject<Item>
    {
        [Key(0)]
        public AuthenticationMethod AuthenticationMethod { get; set; } = new NoAuthentication();

        [Key(1)]
        public string Description { get; set; }

        [Key(2)]
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
