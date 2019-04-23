using System.Collections.ObjectModel;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageCollectionViewModel : RequestMessageHierarchyItemViewModel
    {
        public bool IsExpanded { get; set; }

        public ObservableCollection<RequestMessageHierarchyItemViewModel> Items { get; } = new ObservableCollection<RequestMessageHierarchyItemViewModel>();
    }
}
