using System.Windows.Input;

namespace ByrneLabs.Commons.Presentation.Wpf
{
    public interface ICommandHandler
    {
        void InvokeCanExecuteCommandDelegate(object sender, CanExecuteRoutedEventArgs eventArgs);

        void InvokeExecuteCommandDelegate(object sender, ExecutedRoutedEventArgs eventArgs);

        void InvokePreviewCanExecuteCommandDelegate(object sender, CanExecuteRoutedEventArgs eventArgs);

        void InvokePreviewExecuteCommandDelegate(object sender, ExecutedRoutedEventArgs eventArgs);
    }
}
