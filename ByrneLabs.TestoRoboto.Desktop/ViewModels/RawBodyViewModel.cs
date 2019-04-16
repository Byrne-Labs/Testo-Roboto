using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RawBodyViewModel : BodyViewModel
    {
        public string Content { get; set; }

        public string ContentType { get; set; }

        public IEnumerable<string> ContentTypes { get; } = new[] { "application/javascript", "application/json", "application/xml", "text/plain", "text/html", "text/xml" };

        public override string Name => "Raw";
    }
}
