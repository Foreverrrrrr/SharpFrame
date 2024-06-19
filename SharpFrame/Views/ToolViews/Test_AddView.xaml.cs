using Prism.Events;
using SharpFrame.Structure.Parameter;
using SharpFrame.ViewModels.ToolViewModels;
using System.Windows;

namespace SharpFrame.Views.ToolViews
{
    /// <summary>
    /// Test_AddView.xaml 的交互逻辑
    /// </summary>
    public partial class Test_AddView : Window
    {
        public Test_AddView(IEventAggregator aggregator, TestParameter test, string filtrationmodel)
        {
            InitializeComponent();
            this.DataContext = new Test_AddViewModel(aggregator, test, filtrationmodel);
        }
    }
}
