using System;
using System.Text;

namespace ByrneLabs.TestoRoboto.Crawler.PageItems
{
    public class PageItem : IEquatable<PageItem>
    {
        public PageItem(string @class, string id, string name, string onClick, string title, string tag, string type, string handler)
        {
            Class = @class;
            Id = id;
            Name = name;
            OnClick = onClick;
            Tag = tag;
            Title = title;
            Type = type;
            Handler = handler;
        }

        public string Class { get; }

        public string Handler { get; }

        public string Id { get; }

        public string Name { get; }

        public string OnClick { get; }

        public string Tag { get; }

        public string Title { get; }

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

            return string.Equals(Class, other.Class) && string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(OnClick, other.OnClick) && string.Equals(Class, other.Class) && string.Equals(Tag, other.Tag) && string.Equals(Type, other.Type) && string.Equals(Handler, other.Handler);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id != null ? Id.GetHashCode() : 0;
                hashCode = hashCode * 397 ^ (Class != null ? Class.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (OnClick != null ? OnClick.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Class != null ? Class.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = hashCode * 397 ^ (Handler != null ? Handler.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"<{Tag} ");
            if (!string.IsNullOrWhiteSpace(Id))
            {
                stringBuilder.Append($"id='{Id}' ");
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                stringBuilder.Append($"name='{Name}' ");
            }

            if (!string.IsNullOrWhiteSpace(Title))
            {
                stringBuilder.Append($"title='{Title}' ");
            }

            if (!string.IsNullOrWhiteSpace(Class))
            {
                stringBuilder.Append($"class='{Class}' ");
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                stringBuilder.Append($"type='{Type}' ");
            }

            if (!string.IsNullOrWhiteSpace(OnClick))
            {
                stringBuilder.Append($"onclick='{OnClick}' ");
            }

            stringBuilder.Append("/>");

            return stringBuilder.ToString();
        }
    }
}
