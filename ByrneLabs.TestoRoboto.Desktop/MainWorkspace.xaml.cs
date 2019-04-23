using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ByrneLabs.TestoRoboto.Desktop
{
    public partial class MainWorkspace
    {
        public MainWorkspace()
        {
            InitializeComponent();
        }

        private void TreeViewItem_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (e.OriginalSource is TextBlock treeViewItemTextBlock)
                {
                    ApplicationCommands.Open.Execute(treeViewItemTextBlock.DataContext, sender as IInputElement);
                }
            }
        }

        private void TreeViewItem_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;

            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            if (source is TreeViewItem treeViewItem)
            {
                treeViewItem.IsSelected = true;
            }
        }
    }
}
