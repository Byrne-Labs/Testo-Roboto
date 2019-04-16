using System.ComponentModel;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class CookieViewModel : INotifyPropertyChanged
    {
        public string Description { get; set; }

        public string Domain { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
