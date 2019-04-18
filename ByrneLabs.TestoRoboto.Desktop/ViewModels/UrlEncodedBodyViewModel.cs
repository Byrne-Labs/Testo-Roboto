using System.Collections.ObjectModel;
using ByrneLabs.Commons.Collections;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class UrlEncodedBodyViewModel : BodyViewModel
    {
        public UrlEncodedBodyViewModel()
        {
            AddParameterCommand = new RelayCommand(param => AddParameter());
            DeleteSelectedParameterCommand = new RelayCommand(param => DeleteSelectedParameter(), param => CanDeleteSelectedParameter());
        }

        public RelayCommand AddParameterCommand { get; }

        public RelayCommand DeleteSelectedParameterCommand { get; }

        public override string Name => "URL Encoded";

        public ObservableCollection<KeyValueViewModel> Parameters { get; } = new FullyObservableCollection<KeyValueViewModel>();

        public KeyValueViewModel SelectedParameter { get; set; }

        public void AddParameter()
        {
            Parameters.Add(new KeyValueViewModel());
        }

        public bool CanDeleteSelectedParameter() => SelectedParameter != null;

        public void DeleteSelectedParameter()
        {
            Parameters.Remove(SelectedParameter);
        }
    }
}
