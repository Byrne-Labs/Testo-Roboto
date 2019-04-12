using System.ComponentModel;

namespace ByrneLabs.Commons.Presentation.Wpf
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
