using System.ComponentModel;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public abstract class AuthenticationViewModel : INotifyPropertyChanged
    {
        public abstract string Name { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
