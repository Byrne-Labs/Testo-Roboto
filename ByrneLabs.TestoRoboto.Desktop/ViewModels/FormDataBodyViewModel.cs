using System.Collections.ObjectModel;
using ByrneLabs.Commons.Collections;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class FormDataBodyViewModel : BodyViewModel
    {
        public FormDataBodyViewModel()
        {
            AddParameterCommand = new RelayCommand(param => AddParameter());
            DeleteSelectedParameterCommand = new RelayCommand(param => DeleteSelectedParameter(), param => CanDeleteSelectedParameter());
        }

        public RelayCommand AddParameterCommand { get; }

        public RelayCommand DeleteSelectedParameterCommand { get; }

        public override string Name => "Form Data";

        public ObservableCollection<BodyParameterViewModel> Parameters { get; } = new FullyObservableCollection<BodyParameterViewModel>();

        public BodyParameterViewModel SelectedParameter { get; set; }

        public void AddParameter()
        {
            Parameters.Add(new BodyParameterViewModel());
        }

        public bool CanDeleteSelectedParameter() => SelectedParameter != null;

        public void DeleteSelectedParameter()
        {
            Parameters.Remove(SelectedParameter);
        }
    }
}
