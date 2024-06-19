using Prism.Events;
using SharpFrame.ViewModels.ToolViewModels;
using System.Windows;

namespace SharpFrame.Views.ToolViews
{
    /// <summary>
    /// Interaction logic for ParameterInitializationView.xaml
    /// </summary>
    public partial class ParameterInitializationView : Window
    {
        public ParameterInitializationView(IEventAggregator aggregator)
        {
            InitializeComponent();
            this.DataContext = new ParameterInitializationViewModel(aggregator);
        }
    }
}
