using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageCollectionViewModel : RequestMessageHierarchyItemViewModel
    {
        public RequestMessageCollectionViewModel()
        {
            Items = new ObservableCollection<RequestMessageHierarchyItemViewModel>();
            Items.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems.Cast<RequestMessageHierarchyItemViewModel>())
                    {
                        newItem.PropertyChanged += ItemPropertyChanged;
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.NewItems.Cast<RequestMessageHierarchyItemViewModel>())
                    {
                        oldItem.PropertyChanged -= ItemPropertyChanged;
                    }
                }
            };
        }

        public bool IsExpanded { get; set; }

        public ObservableCollection<RequestMessageHierarchyItemViewModel> Items { get; }

        public void SortItems()
        {
            var sortedItems = Items.OrderBy(item => item is RequestMessageCollectionViewModel ? 1 : 2).ThenBy(item => item.Name).ToList();
            foreach (var item in sortedItems)
            {
                Items.Move(Items.IndexOf(item), sortedItems.IndexOf(item));
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name))
            {
                SortItems();
            }
        }
    }
}
