using System;
using System.Collections.Generic;
using System.Linq;
using ByrneLabs.TestoRoboto.Crawler.PageItems;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class ActionChainItem : IEquatable<ActionChainItem>
    {
        public ActionChainItem(IEnumerable<PageItem> availableActionItems, IEnumerable<PageItem> dataInputItems, PageItem chosenActionItem, string url)
        {
            AvailableActionItems = availableActionItems;
            DataInputItems = dataInputItems;
            ChosenActionItem = chosenActionItem;
            Url = url;
        }

        public IEnumerable<PageItem> AvailableActionItems { get; }

        public PageItem ChosenActionItem { get; set; }

        public bool Crawled { get; set; }

        public IEnumerable<PageItem> DataInputItems { get; }

        public string Url { get; }

        public static bool operator ==(ActionChainItem left, ActionChainItem right) => Equals(left, right);

        public static bool operator !=(ActionChainItem left, ActionChainItem right) => !Equals(left, right);

        public override bool Equals(object obj) => Equals(obj as ActionChainItem);

        public bool Equals(ActionChainItem other)
        {
            if (other == null || other.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(ChosenActionItem, other.ChosenActionItem) && AvailableActionItems.SequenceEqual(other.AvailableActionItems) && Crawled == other.Crawled && DataInputItems.SequenceEqual(other.DataInputItems) && Equals(Url, other.Url);
        }

        public override int GetHashCode() => Url != null ? Url.GetHashCode() : 0;

        public bool SameState(ActionChainItem other)
        {
            if (other == null || other.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(ChosenActionItem, other.ChosenActionItem) && AvailableActionItems.SequenceEqual(other.AvailableActionItems) && DataInputItems.SequenceEqual(other.DataInputItems) && Equals(Url, other.Url);
        }

        public override string ToString() => $"{Url} - {ChosenActionItem}";
    }
}
