using Prism.Events;
using SharpFrame.Structure.Parameter;
using SharpFrame.ViewModels.ToolViewModels;
using System.Windows;

namespace SharpFrame.Views.ToolViews
{
    /// <summary>
    /// System_AddView.xaml 的交互逻辑
    /// </summary>
    public partial class System_AddView : Window
    {
        public System_AddView(IEventAggregator aggregator, SystemParameter system, string filtrationmodel)
        {
            InitializeComponent();
            this.DataContext = new System_AddViewModel(aggregator, system, filtrationmodel);
        }
    }
}
