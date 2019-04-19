using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            OpenRequestMessages = new ObservableCollection<RequestMessageViewModel>();
            OpenRequestMessages.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var item in args.NewItems.Cast<RequestMessageViewModel>())
                    {
                        item.PropertyChanged += RequestMessageOnPropertyChanged;
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (var item in args.OldItems.Cast<RequestMessageViewModel>())
                    {
                        item.PropertyChanged -= RequestMessageOnPropertyChanged;
                    }
                }
            };

            OpenRequestMessages.Add(new RequestMessageViewModel { Name = "Request 1" });
            OpenRequestMessages.Add(new RequestMessageViewModel { Name = "Request 2" });
        }

        public bool BackStageVisible { get; set; } = false;

        public ObservableCollection<RequestMessageViewModel> OpenRequestMessages { get; }

        public bool RibbonTabsVisible { get; set; } = false;

        public bool StartScreenVisible { get; set; } = true;

        private void RequestMessageOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RequestMessageViewModel.IsClosed))
            {
                var requestMessageViewModel = sender as RequestMessageViewModel;
                if (requestMessageViewModel.IsClosed)
                {
                    OpenRequestMessages.Remove(requestMessageViewModel);
                }
            }
        }
    }
}
