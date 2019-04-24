using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class MainWorkspaceViewModel : ViewModelBase
    {
        public MainWorkspaceViewModel()
        {
            OpenRequestMessages = new ObservableCollection<RequestMessageViewModel>();
            OpenRequestMessages.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var item in args.NewItems.Cast<RequestMessageViewModel>())
                    {
                        item.PropertyChanged += RequestMessageOnPropertyChanged;
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (var item in args.OldItems.Cast<RequestMessageViewModel>())
                    {
                        item.PropertyChanged -= RequestMessageOnPropertyChanged;
                    }
                }
            };

            RegisterCommand(ApplicationCommands.Open, null, null, OpenCommandCanExecute, OpenCommandExecuted);
            RegisterCommand(TestoRobotoCommands.NewRequestMessageCollection, null, null, null, NewRequestMessageCollectionCommandExecuted);
            RegisterCommand(TestoRobotoCommands.NewRequestMessage, null, null, null, NewRequestMessageCommandExecuted);
        }

        public bool BackStageVisible { get; set; } = false;

        public ObservableCollection<RequestMessageViewModel> OpenRequestMessages { get; }

        public ObservableCollection<RequestMessageHierarchyItemViewModel> RequestMessageHierarchyItemViewModels { get; } = new ObservableCollection<RequestMessageHierarchyItemViewModel>();

        public bool RibbonTabsVisible { get; set; } = false;

        public RequestMessageHierarchyItemViewModel SelectedRequestMessageHierarchyItem { get; set; }

        public bool StartScreenVisible { get; set; } = true;

        private void NewRequestMessageCollectionCommandExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            RequestMessageCollectionViewModel selectedCollectionViewModel;
            if (eventArgs.Parameter is RequestMessageViewModel requestMessageViewModel)
            {
                selectedCollectionViewModel = requestMessageViewModel.ParentCollection;
            }
            else if (eventArgs.Parameter is RequestMessageCollectionViewModel requestMessageCollectionViewModel)
            {
                selectedCollectionViewModel = requestMessageCollectionViewModel;
            }
            else if (eventArgs.Parameter == null)
            {
                selectedCollectionViewModel = null;
            }
            else
            {
                throw new ArgumentException($"The parameter must be null or of type {nameof(RequestMessageCollectionViewModel)} or {nameof(requestMessageViewModel)}", nameof(eventArgs.Parameter));
            }

            var newRequestMessageCollection = new RequestMessageCollectionViewModel { Name = "New Request Message Collection", IsSelected = true, ParentCollection = selectedCollectionViewModel };
            if (selectedCollectionViewModel == null)
            {
                RequestMessageHierarchyItemViewModels.Add(newRequestMessageCollection);
            }
            else
            {
                selectedCollectionViewModel.Items.Add(newRequestMessageCollection);
                selectedCollectionViewModel.IsExpanded = true;
            }
        }

        private void NewRequestMessageCommandExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            RequestMessageCollectionViewModel selectedCollectionViewModel;
            if (eventArgs.Parameter is RequestMessageViewModel requestMessageViewModel)
            {
                selectedCollectionViewModel = requestMessageViewModel.ParentCollection;
            }
            else if (eventArgs.Parameter is RequestMessageCollectionViewModel requestMessageCollectionViewModel)
            {
                selectedCollectionViewModel = requestMessageCollectionViewModel;
            }
            else if (eventArgs.Parameter == null)
            {
                selectedCollectionViewModel = null;
            }
            else
            {
                throw new ArgumentException($"The parameter must be null or of type {nameof(RequestMessageCollectionViewModel)} or {nameof(requestMessageViewModel)}", nameof(eventArgs.Parameter));
            }

            var newRequestMessage = new RequestMessageViewModel { Name = "New Request Message", IsSelected = true, ParentCollection = selectedCollectionViewModel };
            if (selectedCollectionViewModel == null)
            {
                RequestMessageHierarchyItemViewModels.Add(newRequestMessage);
            }
            else
            {
                selectedCollectionViewModel.Items.Add(newRequestMessage);
                selectedCollectionViewModel.IsExpanded = true;
            }

            OpenRequestMessages.Add(newRequestMessage);
        }

        private void OpenCommandCanExecute(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = eventArgs.Parameter != null;
        }

        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var requestMessage = eventArgs.Parameter as RequestMessageViewModel;

            if (!OpenRequestMessages.Contains(requestMessage))
            {
                requestMessage.IsClosed = false;
                OpenRequestMessages.Add(requestMessage);
            }
        }

        private void RequestMessageOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RequestMessageViewModel.IsClosed))
            {
                var requestMessageViewModel = sender as RequestMessageViewModel;
                if (requestMessageViewModel.IsClosed)
                {
                    OpenRequestMessages.Remove(requestMessageViewModel);
                }
            }
        }
    }
}
