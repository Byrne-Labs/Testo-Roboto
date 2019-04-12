using System.Windows.Input;
using ByrneLabs.TestoRoboto.Desktop.ViewModels;

namespace ByrneLabs.TestoRoboto.Desktop
{
    public partial class RequestMessage
    {
        public RequestMessage()
        {
            InitializeComponent();

            var requestMessageViewModel = new RequestMessageViewModel();
            DataContext = requestMessageViewModel;
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
