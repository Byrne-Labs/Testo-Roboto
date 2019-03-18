using System;

namespace ByrneLabs.TestoRoboto.Crawler
{
    public class PageItem : IEquatable<PageItem>
    {
        public PageItem(string id, string name, string onClick, string source, string tag, string text, string type, string handler)
        {
            Id = id;
            Name = name;
            OnClick = onClick;
            Source = source;
            Tag = tag;
            Text = text;
            Type = type;
            Handler = handler;
        }

        public string Handler { get; }

        public string Id { get; }

        public string Name { get; }

        public string OnClick { get; }

        public string Source { get; }

        public string Tag { get; }

        public string Text { get; }

        public string Type { get; }

        public static bool operator ==(PageItem left, PageItem right) => Equals(left, right);

        public static bool operator !=(PageItem left, PageItem right) => !Equals(left, right);

        public override bool Equals(object obj) => Equals(obj as PageItem);

        public bool Equals(PageItem other)
        {
            if (other == null || other.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(OnClick, other.OnClick) && string.Equals(Source, other.Source) && string.Equals(Tag, other.Tag) && string.Equals(Text, other.Text) && string.Equals(Type, other.Type) && string.Equals(Handler, other.Handler);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id != null ? Id.GetHashCode() : 0;
                hashCode = hashCode * 397 ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (OnClick != null ? OnClick.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Source != null ? Source.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Handler != null ? Handler.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
