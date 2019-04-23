using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class ActionChain : HandyObject<ActionChain>
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

        public Exception Exception { get; set; }

        public bool IsLooped => Items.Count > Items.Distinct(new ActionChainLoopFinder()).Count();

        public IList<ActionChainItem> Items { get; } = new List<ActionChainItem>();

        public int Priority
        {
            get
            {
                var lastChainItem = Items.Last();
                if (lastChainItem.ChosenActionItem.Tag == "input" && lastChainItem.ChosenActionItem.Type == "submit")
                {
                    return 2;
                }

                if (lastChainItem.ChosenActionItem.Tag == "input" && lastChainItem.ChosenActionItem.Type == "button")
                {
                    return 3;
                }

                if (lastChainItem.ChosenActionItem.Tag == "a" && !string.IsNullOrWhiteSpace(lastChainItem.ChosenActionItem.Href) && lastChainItem.Url.SubstringBeforeLast("?").StartsWith(lastChainItem.ChosenActionItem.Href.SubstringBeforeLast("?"), StringComparison.Ordinal))
                {
                    return 4;
                }

                if (lastChainItem.ChosenActionItem.Tag == "a" && !string.IsNullOrWhiteSpace(lastChainItem.ChosenActionItem.Href) && lastChainItem.Url.StartsWith(lastChainItem.ChosenActionItem.Href, StringComparison.Ordinal))
                {
                    return 5;
                }

                if (lastChainItem.ChosenActionItem.Tag == "a" && string.IsNullOrWhiteSpace(lastChainItem.ChosenActionItem.Href))
                {
                    return 6;
                }

                if (lastChainItem.ChosenActionItem.Tag == "input")
                {
                    return 7;
                }

                return 1;
            }
        }

        public string TerminationReason { get; set; }

        public static bool operator ==(ActionChain left, ActionChain right) => Equals(left, right);

        public static bool operator !=(ActionChain left, ActionChain right) => !Equals(left, right);

        public override bool Equals(object obj) => Equals(obj as ActionChain);

        public override bool Equals(ActionChain other)
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
