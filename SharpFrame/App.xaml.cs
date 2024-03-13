using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using SharpFrame.Common;
using SharpFrame.ViewModels;
using SharpFrame.Views;
using System.Windows;

namespace SharpFrame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SystemLogInViewModel.LogInLoad += (x, y) =>
            {

            };
            SystemLogIn systemLogIn = new SystemLogIn();
            systemLogIn.Show();
            Cloud_Client client = new Cloud_Client("122.51.121.66", 13222);
            client.SuccessfuConnectEvent += (t, x) =>
            {
                SystemLogIn.viewModel.Client_On_Line = true;
            };
        }

        protected override Window CreateShell()
        {
            return null;
            return Container.Resolve<Window>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
