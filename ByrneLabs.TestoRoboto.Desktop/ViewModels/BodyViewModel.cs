using System.ComponentModel;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public abstract class BodyViewModel : INotifyPropertyChanged
    {
        public abstract string Name { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
