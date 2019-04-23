using System;
using System.Windows;
using System.Windows.Input;

namespace ByrneLabs.Commons.Presentation.Wpf
{
    public class ViewModelCommandBinding : CommandBinding
    {
        public static readonly DependencyProperty CommandHandlerProperty = DependencyProperty.RegisterAttached("CommandHandler", typeof(ICommandHandler), typeof(ViewModelCommandBinding), new UIPropertyMetadata(null, OnCommandHandlerChanged));
        private ICommandHandler _commandHandler;

        public ViewModelCommandBinding()
        {
            PreviewExecuted += ViewModelCommandBinding_PreviewExecuted;
        }

        public ICommandHandler CommandHandler
        {
            get => _commandHandler;
            set
            {
                if (value == null && _commandHandler != null)
                {
                    PreviewCanExecute -= _commandHandler.InvokePreviewCanExecuteCommandDelegate;
                    CanExecute -= _commandHandler.InvokeCanExecuteCommandDelegate;
                    Executed -= _commandHandler.InvokeExecuteCommandDelegate;
                }
                else if (value != null)
                {
                    PreviewCanExecute += value.InvokePreviewCanExecuteCommandDelegate;
                    CanExecute += value.InvokeCanExecuteCommandDelegate;
                    Executed += value.InvokeExecuteCommandDelegate;
                }

                _commandHandler = value;
            }
        }

        public static ICommandHandler GetCommandHandler(DependencyObject dependencyObject) => (ICommandHandler) dependencyObject.GetValue(CommandHandlerProperty);

        public static void SetCommandHandler(DependencyObject dependencyObject, ICommandHandler value)
        {
            dependencyObject.SetValue(CommandHandlerProperty, value);
        }

        private static void OnCommandHandlerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var commandHandler = eventArgs.NewValue as ICommandHandler;
            if (dependencyObject is FrameworkElement frameworkElement && !frameworkElement.IsLoaded)
            {
                void Handler(object sender, RoutedEventArgs e)
                {
                    frameworkElement.Loaded -= Handler;
                    ProcessCommandHandlerChanged(dependencyObject, commandHandler);
                }

                frameworkElement.Loaded += Handler;
            }
            else
            {
                ProcessCommandHandlerChanged(dependencyObject, commandHandler);
            }
        }

        private static void ProcessCommandHandlerChanged(DependencyObject dependencyObject, ICommandHandler commandHandler)
        {
            var commandBindings = dependencyObject is FrameworkElement frameworkElement ? frameworkElement.CommandBindings : null;

            if (commandBindings == null)
            {
                throw new ArgumentException("Invalid dependency object");
            }

            foreach (CommandBinding commandBinding in commandBindings)
            {
                if (commandBinding is ViewModelCommandBinding viewModelCommandBinding && viewModelCommandBinding.CommandHandler == null)
                {
                    viewModelCommandBinding.CommandHandler = commandHandler;
                }
            }
        }

        private void ViewModelCommandBinding_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_commandHandler != null)
            {
                _commandHandler.InvokePreviewExecuteCommandDelegate(sender, e);

                if (!e.Handled)
                {
                    _commandHandler.InvokeExecuteCommandDelegate(sender, e);
                }
            }
        }
    }
}
