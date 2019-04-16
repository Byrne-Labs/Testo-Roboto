using System.ComponentModel;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class HeaderViewModel : INotifyPropertyChanged
    {
        public string Description { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
