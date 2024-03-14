using Microsoft.Win32;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using SharpFrame.Common;
using SharpFrame.ViewModels;
using SharpFrame.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SharpFrame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
            if (HaveRunningInstance())
            {
                System.Windows.Forms.MessageBox.Show($"请勿重复打开应用程序", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Process.GetCurrentProcess().Kill();
            }
        }

        protected override void OnInitialized()
        {
            SystemLogIn systemLogIn = new SystemLogIn();
            Permission_Check();
            SystemLogInViewModel.LogInLoad += (x, y) =>
            {
                switch (x)
                {
                    case "生产员":
                        if (y == "666888")
                        {
                            base.OnInitialized();
                            MainWindowViewModel.PermissionCommand.Execute(new Permission() { type = PermissionType.ProductionStaff });
                        }
                        break;
                    case "工程师":
                        if (y == "2024")
                        {
                            base.OnInitialized();
                            MainWindowViewModel.PermissionCommand.Execute(new Permission() { type = PermissionType.Engineer });
                        }
                        break;
                    case "供应商":
                        if (y == "ksrhck")
                        {
                            base.OnInitialized();
                            MainWindowViewModel.PermissionCommand.Execute(new Permission() { type = PermissionType.Supplier });
                        }
                        break;
                    default:
                        break;
                }
                systemLogIn.Close();
            };
            systemLogIn.Show();
            //Cloud_Client client = new Cloud_Client("122.51.121.66", 13222);
            //client.SuccessfuConnectEvent += (t, x) =>
            //{
            //    SystemLogIn.viewModel.Client_On_Line = true;
            //};
        }

        private void Permission_Check()
        {
            ClientKeyMaker.ClientToken clientToken = new ClientKeyMaker.ClientToken();
            clientToken.LoginEvent += ((b, t1, t2) =>
            {
                if (!b)
                {
                    SystemLogIn.viewModel.Warranty = false;
                    System.Windows.Forms.MessageBox.Show($"未授权，请联系管理员进行授权!\r\n30分钟后将自动退出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    SystemLogIn.viewModel.Warranty = true;
                }
            });
            clientToken.Initialize();
        }

        /// <summary>
        /// 判断是否已经存在运行的实例
        /// </summary>
        /// <returns>存在返回true，不存在返回false</returns>
        public static bool HaveRunningInstance()
        {
            System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(current.ProcessName);
            if (processes.Length >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<SystemLogIn>();
            containerRegistry.RegisterSingleton<MainWindow>();
            containerRegistry.RegisterSingleton<ParameterPage>();
        }
    }
}
