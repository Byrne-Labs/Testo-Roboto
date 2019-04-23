using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageHierarchyItemViewModel : ViewModelBase
    {
        public AuthenticationViewModel AuthenticationViewModel { get; set; }

        public string Description { get; set; }

        public bool IsSelected { get; set; }

        public string Name { get; set; }

        public RequestMessageCollectionViewModel ParentCollection { get; set; }
    }
}
