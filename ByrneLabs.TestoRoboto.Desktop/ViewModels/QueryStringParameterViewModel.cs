using System.ComponentModel;
using System.Web;
using JetBrains.Annotations;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    [PublicAPI]
    public class QueryStringParameterViewModel : INotifyPropertyChanged
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string UriEncodedValue { get; set; }

        public string Value
        {
            get => HttpUtility.UrlDecode(UriEncodedValue);
            set => UriEncodedValue = HttpUtility.UrlEncode(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
