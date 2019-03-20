using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class ActionChain : Entity<ActionChain>, IEquatable<ActionChain>
    {
        private class ActionChainLoopFinder : IEqualityComparer<ActionChainItem>
        {
            public bool Equals(ActionChainItem x, ActionChainItem y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x == null || y == null || y.GetType() != x.GetType())
                {
                    return false;
                }

                return x.AvailableActionItems.SequenceEqual(y.AvailableActionItems) && x.DataInputItems.SequenceEqual(y.DataInputItems);
            }

            public int GetHashCode(ActionChainItem obj) => obj.Url.GetHashCode();
        }

        public bool IsLooped => Items.Count > Items.Distinct(new ActionChainLoopFinder()).Count();

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

        public override string ToString() => string.Join(" ==> ", Items.Select(item => item.ToString()));
    }
}
