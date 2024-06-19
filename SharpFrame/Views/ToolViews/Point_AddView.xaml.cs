using Prism.Events;
using SharpFrame.ViewModels.ToolViewModels;
using System.Windows;

namespace SharpFrame.Views.ToolViews
{
    /// <summary>
    /// Point_AddView.xaml 的交互逻辑
    /// </summary>
    public partial class Point_AddView : Window
    {
        public Point_AddView(IEventAggregator aggregator, Structure.Parameter.PointLocationParameter system, string filtrationmodel)
        {
            InitializeComponent();
            this.DataContext = new Point_AddViewModel(aggregator, system, filtrationmodel);

        }
    }
}
