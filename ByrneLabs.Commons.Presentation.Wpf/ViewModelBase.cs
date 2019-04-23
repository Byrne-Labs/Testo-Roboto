using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.Presentation.Wpf
{
    public class ViewModelBase : INotifyPropertyChanged, ICommandHandler
    {
        private struct CommandCallbacks
        {
            public readonly CanExecuteRoutedEventHandler CanExecute;
            public readonly ExecutedRoutedEventHandler Execute;
            public readonly CanExecuteRoutedEventHandler PreviewCanExecute;
            public readonly ExecutedRoutedEventHandler PreviewExecute;

            public CommandCallbacks(CanExecuteRoutedEventHandler previewCanExecute, ExecutedRoutedEventHandler previewExecute, CanExecuteRoutedEventHandler canExecute, ExecutedRoutedEventHandler execute)
            {
                PreviewCanExecute = previewCanExecute;
                PreviewExecute = previewExecute;
                CanExecute = canExecute;
                Execute = execute;
            }
        }

        private readonly Dictionary<ICommand, List<CommandCallbacks>> _commandToCallbacksMap = new Dictionary<ICommand, List<CommandCallbacks>>();

        protected ViewModelBase()
        {
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RegisterCommand(ICommand command, CanExecuteRoutedEventHandler previewCanExecute, ExecutedRoutedEventHandler previewExecute, CanExecuteRoutedEventHandler canExecute, ExecutedRoutedEventHandler execute)
        {
            if (!_commandToCallbacksMap.ContainsKey(command))
            {
                _commandToCallbacksMap.Add(command, new List<CommandCallbacks>());
            }

            _commandToCallbacksMap[command].Add(new CommandCallbacks(previewCanExecute, previewExecute, canExecute, execute));
        }

        void ICommandHandler.InvokeCanExecuteCommandDelegate(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            if (_commandToCallbacksMap.ContainsKey(eventArgs.Command))
            {
                foreach (var commandCallback in _commandToCallbacksMap[eventArgs.Command].Where(commandCallbackCheck => commandCallbackCheck.CanExecute != null))
                {
                    commandCallback.CanExecute(sender, eventArgs);
                }
            }
        }

        void ICommandHandler.InvokeExecuteCommandDelegate(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_commandToCallbacksMap.ContainsKey(eventArgs.Command))
            {
                foreach (var commandCallback in _commandToCallbacksMap[eventArgs.Command].Where(commandCallbackCheck => commandCallbackCheck.Execute != null))
                {
                    commandCallback.Execute(sender, eventArgs);
                }
            }
        }

        void ICommandHandler.InvokePreviewCanExecuteCommandDelegate(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            if (_commandToCallbacksMap.ContainsKey(eventArgs.Command))
            {
                foreach (var commandCallback in _commandToCallbacksMap[eventArgs.Command].Where(commandCallbackCheck => commandCallbackCheck.PreviewCanExecute != null))
                {
                    commandCallback.PreviewCanExecute(sender, eventArgs);
                }
            }
        }

        void ICommandHandler.InvokePreviewExecuteCommandDelegate(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (_commandToCallbacksMap.ContainsKey(eventArgs.Command))
            {
                foreach (var commandCallback in _commandToCallbacksMap[eventArgs.Command].Where(commandCallbackCheck => commandCallbackCheck.PreviewExecute != null))
                {
                    commandCallback.PreviewExecute(sender, eventArgs);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
