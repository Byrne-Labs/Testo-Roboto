using ByrneLabs.TestoRoboto.Desktop.ViewModels;

namespace ByrneLabs.TestoRoboto.Desktop
{
    public partial class RequestMessage
    {
        public RequestMessage()
        {
            InitializeComponent();

            var requestMessageViewModel = new RequestMessageViewModel();
            requestMessageViewModel.Name = "name";
            requestMessageViewModel.QueryParameters.Add(new KeyValueViewModel { Key = "key1", Value = "value1", Description = "description" });

            DataContext = requestMessageViewModel;
        }
    }
}
