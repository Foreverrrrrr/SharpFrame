using Prism.Events;
using Prism.Mvvm;

namespace SharpFrame.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        IEventAggregator eventAggregator;

        public MainWindowViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            eventAggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {

            });
        }

        private string _title = "Prism Application";

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }
    }
}
