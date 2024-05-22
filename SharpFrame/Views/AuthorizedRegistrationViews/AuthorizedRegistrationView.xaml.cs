using System.Windows;
using Prism.Events;
using SharpFrame.ViewModels.AuthorizedRegistrationViewModels;

namespace SharpFrame.Views.AuthorizedRegistrationViews
{
    /// <summary>
    /// AuthorizedRegistrationView.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorizedRegistrationView : Window
    {
        public AuthorizedRegistrationView(IEventAggregator aggregator)
        {
            InitializeComponent();
            this.DataContext = new AuthorizedRegistrationViewModel(aggregator);
        }
    }
}
