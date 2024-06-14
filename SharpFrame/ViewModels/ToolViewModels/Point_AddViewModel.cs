using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.ToolViews;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpFrame.ViewModels.ToolViewModels
{
    public class Point_AddViewModel : BindableBase
    {
        IEventAggregator aggregator;

        private Structure.Parameter.PointLocationParameter this_checkdata;
        public Point_AddViewModel(IEventAggregator aggregator, Structure.Parameter.PointLocationParameter system)
        {
            this.aggregator = aggregator;
            this.this_checkdata = system;
            PointDefaultArguments.Add(new Structure.Parameter.PointLocationParameter() { Name = "Null" });
            Close = new DelegateCommand(() =>
            {
                Point_AddView currentWindow = System.Windows.Application.Current.Windows.OfType<Point_AddView>().SingleOrDefault(w => w.IsActive);
                currentWindow.Close();
            });
            OkButton = new DelegateCommand(() =>
            {
                Point_AddView currentWindow = System.Windows.Application.Current.Windows.OfType<Point_AddView>().SingleOrDefault(w => w.IsActive);
                if (PointDefaultArguments.Count > 0)
                {
                    Structure.Parameter.PointLocationParameter pointLocations = PointDefaultArguments[0];
                    aggregator.GetEvent<PointLocationParameterAddEvent>().Publish(new Add_PointLocationIns() { InsertionParameter = this_checkdata, NewParameter = pointLocations });
                }
                currentWindow.Close();
            });
        }

        public DelegateCommand Close { get; set; }

        public DelegateCommand OkButton { get; set; }

        private ObservableCollection<Structure.Parameter.PointLocationParameter> _pointdefaultarguments = new ObservableCollection<Structure.Parameter.PointLocationParameter>();

        public ObservableCollection<Structure.Parameter.PointLocationParameter> PointDefaultArguments
        {
            get { return _pointdefaultarguments; }
            set { _pointdefaultarguments = value; RaisePropertyChanged(); }
        }
    }
}
