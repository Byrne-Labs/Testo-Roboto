using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ByrneLabs.Commons.Presentation.Wpf
{
    /// <summary>
    ///     Base class for all ViewModel classes in the application.
    ///     It provides support for property change notifications
    ///     and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable, ICommandHandler
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
            RegisterCommands();
        }

        public virtual bool IsDirty { get; set; }

        /// <summary>
        ///     Returns whether an exception is thrown, or if a Debug.Fail() is used
        ///     when an invalid property name is passed to the VerifyPropertyName method.
        ///     The default value is false, but subclasses used by unit tests might
        ///     override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        protected static void VerifyArgumentNotNull(object arguement, string arguementName)
        {
            if (arguement == null)
            {
                throw new ArgumentNullException(arguementName);
            }
        }

        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        ///     Warns the developer if this object does not have
        ///     a public property with the specified name. This
        ///     method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                var msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                {
                    throw new Exception(msg);
                }

                Debug.Fail(msg);
            }
        }

        protected virtual bool CheckIfCommandOriginatesHere(object sender) => sender is FrameworkElement element && element.DataContext == this;

        protected virtual bool CheckIfCommandOriginatesHere(CanExecuteRoutedEventArgs e) => e.Parameter == this;

        protected virtual bool CheckIfCommandOriginatesHere(ExecutedRoutedEventArgs e) => e.Parameter == this;

        protected virtual bool CheckIfCommandOriginatesHere(object sender, CanExecuteRoutedEventArgs e) => CheckIfCommandOriginatesHere(sender) || CheckIfCommandOriginatesHere(e);

        protected virtual bool CheckIfCommandOriginatesHere(object sender, ExecutedRoutedEventArgs e) => CheckIfCommandOriginatesHere(sender) || CheckIfCommandOriginatesHere(e);

        protected void ClearAllCommandCallbacks(ICommand command)
        {
            VerifyArgumentNotNull(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                _commandToCallbacksMap.Remove(command);
            }
        }

        protected virtual bool CommandCanExecuteIfOnSelf(object sender, ExecutedRoutedEventArgs e) => CheckIfCommandOriginatesHere(sender, e);

        protected virtual void CommandCanExecuteIfOnSelf(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CheckIfCommandOriginatesHere(sender, e);
        }

        /// <summary>
        ///     Child classes can override this method to perform
        ///     clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            IsDirty = true;
            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected void RegisterCommand(ICommand command, CanExecuteRoutedEventHandler previewCanExecute, ExecutedRoutedEventHandler previewExecute, CanExecuteRoutedEventHandler canExecute, ExecutedRoutedEventHandler execute)
        {
            VerifyArgumentNotNull(command, "command");

            if (!_commandToCallbacksMap.ContainsKey(command))
            {
                _commandToCallbacksMap.Add(command, new List<CommandCallbacks>());
            }

            _commandToCallbacksMap[command].Add(new CommandCallbacks(previewCanExecute, previewExecute, canExecute, execute));
        }

        protected virtual void RegisterCommands()
        {
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
