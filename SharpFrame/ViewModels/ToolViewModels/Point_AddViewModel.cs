using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SharpFrame.ViewModels.ToolViewModels
{
    public class Point_AddViewModel : BindableBase
    {
        IEventAggregator aggregator;

        private PointLocationParameter this_checkdata;
        public Point_AddViewModel(IEventAggregator aggregator, PointLocationParameter system)
        {
            this.aggregator = aggregator;
            this.this_checkdata = system;
            PointDefaultArguments.Add(new PointLocationParameter() { Name = "Null" });
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
                    PointLocationParameter pointLocations = PointDefaultArguments[0];
                    aggregator.GetEvent<PointLocationParameterAddEvent>().Publish(new Add_PointLocationIns() { InsertionParameter = this_checkdata, NewParameter = pointLocations });
                }
                currentWindow.Close();
            });
        }

        public DelegateCommand Close { get; set; }

        public DelegateCommand OkButton { get; set; }

        private ObservableCollection<PointLocationParameter> _pointdefaultarguments = new ObservableCollection<PointLocationParameter>();

        public ObservableCollection<PointLocationParameter> PointDefaultArguments
        {
            get { return _pointdefaultarguments; }
            set { _pointdefaultarguments = value; RaisePropertyChanged(); }
        }
    }
}
