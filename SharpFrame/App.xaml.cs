using Prism.DryIoc;
using Prism.Ioc;
using SharpFrame.ViewModels;
using SharpFrame.Views;
using SharpFrame.Views.ToolViews;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using static SharpFrame.ViewModels.SystemLogInViewModel;

namespace SharpFrame
{
    //Copyright © 2024 Mr. Xu YiFan
    // 1. This software is intended for demonstration and educational purposes only and must not be used for commercial purposes.
    // 2. Modification, reproduction, or redistribution of this software without authorization from Mr.Xu YiFan is prohibited.
    // 3. Mr.Xu YiFan shall not be liable for any loss or damage resulting from the use of this software.
    // 4. In case of encountering bugs or providing suggestions for improvement, please contact Mr.Xu YiFan at awalkingonthecloud@gmail.com.
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
            SystemLogInViewModel.LogInLoad += (evt) =>
            {
                evt.Invoke(new Permission() { systemLog = systemLogIn });
            };
            systemLogIn.ShowDialog();
            base.OnInitialized();
            //Cloud_Client client = new Cloud_Client("122.51.121.66", 13222);
            //client.SuccessfuConnectEvent += (t, x) =>
            //{
            //    SystemLogIn.viewModel.Client_On_Line = true;
            //};
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
                return true;
            else
                return false;
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow>();
            containerRegistry.RegisterForNavigation<ParameterView>();
            containerRegistry.RegisterForNavigation<System_AddView>();
            containerRegistry.RegisterForNavigation<DebuggingView>();
            containerRegistry.RegisterForNavigation<RelationalDatabaseView>();
            containerRegistry.RegisterForNavigation<LogView>();
        }
    }
}
