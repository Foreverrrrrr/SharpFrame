using Prism.Events;
using SharpFrame.ViewModels.ToolViewModels;
using System.Collections.Generic;
using System.Windows;

namespace SharpFrame.Views.ToolViews
{
    /// <summary>
    /// NewParameterModelView.xaml 的交互逻辑
    /// </summary>
    public partial class NewParameterModelView : Window
    {
        public NewParameterModelView(IEventAggregator aggregator, List<string> parameterslist)
        {
            InitializeComponent();
            this.DataContext = new NewParameterModelViewModel(aggregator, parameterslist);

        }
    }
}
