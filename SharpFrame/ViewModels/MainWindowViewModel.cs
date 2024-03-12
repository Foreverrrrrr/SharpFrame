using Prism.Mvvm;

namespace SharpFrame.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
