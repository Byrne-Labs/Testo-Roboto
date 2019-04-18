using System.Windows;
using ByrneLabs.TestoRoboto.Desktop.ViewModels;

namespace ByrneLabs.TestoRoboto.Desktop
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel();
        }
    }
}
