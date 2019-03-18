using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class ActionChain : Entity<ActionChain>, IEquatable<ActionChain>
    {
        public bool IsLooped => Items.Count > Items.Distinct().Count();

        public IList<ActionChainItem> Items { get; } = new List<ActionChainItem>();

        public static bool operator ==(ActionChain left, ActionChain right) => Equals(left, right);

        public static bool operator !=(ActionChain left, ActionChain right) => !Equals(left, right);

        public override bool Equals(object obj) => Equals(obj as ActionChain);

        public new bool Equals(ActionChain other)
        {
            if (other == null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || Items.SequenceEqual(other.Items);
        }

        public override int GetHashCode() => GetType().GetHashCode();
    }
}
