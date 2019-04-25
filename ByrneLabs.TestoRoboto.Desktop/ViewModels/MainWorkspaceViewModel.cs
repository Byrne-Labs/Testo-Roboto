﻿using System;
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
            OpenRequestMessageHierarchyItems = new ObservableCollection<RequestMessageHierarchyItemViewModel>();
            OpenRequestMessageHierarchyItems.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var item in args.NewItems.Cast<RequestMessageHierarchyItemViewModel>())
                    {
                        item.PropertyChanged += OpenRequestMessageOnPropertyChanged;
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (var item in args.OldItems.Cast<RequestMessageHierarchyItemViewModel>())
                    {
                        item.PropertyChanged -= OpenRequestMessageOnPropertyChanged;
                    }
                }
            };

            RequestMessageHierarchyItemViewModels = new ObservableCollection<RequestMessageHierarchyItemViewModel>();
            RequestMessageHierarchyItemViewModels.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems.Cast<RequestMessageHierarchyItemViewModel>())
                    {
                        newItem.PropertyChanged += RequestMessageHierarchyItemPropertyChanged;
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.NewItems.Cast<RequestMessageHierarchyItemViewModel>())
                    {
                        oldItem.PropertyChanged -= RequestMessageHierarchyItemPropertyChanged;
                    }
                }
            };

            RegisterCommand(ApplicationCommands.Open, null, null, OpenCommandCanExecute, OpenCommandExecuted);
            RegisterCommand(TestoRobotoCommands.NewRequestMessageCollection, null, null, null, NewRequestMessageCollectionCommandExecuted);
            RegisterCommand(TestoRobotoCommands.NewRequestMessage, null, null, null, NewRequestMessageCommandExecuted);
        }

        public bool BackStageVisible { get; set; } = false;

        public ObservableCollection<RequestMessageHierarchyItemViewModel> OpenRequestMessageHierarchyItems { get; }

        public ObservableCollection<RequestMessageHierarchyItemViewModel> RequestMessageHierarchyItemViewModels { get; }

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
                SortRequestMessageHierarchyItemViewModels();
            }
            else
            {
                selectedCollectionViewModel.Items.Add(newRequestMessage);
                selectedCollectionViewModel.IsExpanded = true;
                selectedCollectionViewModel.SortItems();
            }

            OpenRequestMessageHierarchyItems.Add(newRequestMessage);
        }

        private void OpenCommandCanExecute(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = eventArgs.Parameter != null;
        }

        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var requestMessage = eventArgs.Parameter as RequestMessageHierarchyItemViewModel;

            if (!OpenRequestMessageHierarchyItems.Contains(requestMessage))
            {
                requestMessage.IsClosed = false;
                OpenRequestMessageHierarchyItems.Add(requestMessage);
            }
        }

        private void OpenRequestMessageOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RequestMessageViewModel.IsClosed))
            {
                var requestMessageViewModel = sender as RequestMessageViewModel;
                if (requestMessageViewModel.IsClosed)
                {
                    OpenRequestMessageHierarchyItems.Remove(requestMessageViewModel);
                }
            }
        }

        private void RequestMessageHierarchyItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RequestMessageHierarchyItemViewModel.Name))
            {
                SortRequestMessageHierarchyItemViewModels();
            }
        }

        private void SortRequestMessageHierarchyItemViewModels()
        {
            var sortedItems = RequestMessageHierarchyItemViewModels.OrderBy(item => item is RequestMessageCollectionViewModel ? 1 : 2).ThenBy(item => item.Name).ToList();
            foreach (var item in sortedItems)
            {
                RequestMessageHierarchyItemViewModels.Move(RequestMessageHierarchyItemViewModels.IndexOf(item), sortedItems.IndexOf(item));
            }
        }
    }
}
