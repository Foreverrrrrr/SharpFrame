using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Common;
using SharpFrame.Logic.Base;
using SharpFrame.Structure.Parameter;
using SharpFrame.Views;
using SharpFrame.Views.AuthorizedRegistrationViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SharpFrame.Common.Geometry;

namespace SharpFrame.ViewModels
{
    public class SystemLogInViewModel : BindableBase
    {
        public static event Action<Action<Permission>> LogInLoad;

        public DelegateCommand<string> Login_button { get; set; }

        public DelegateCommand Close { get; set; }

        public SystemLogInViewModel(IEventAggregator aggregators)
        {
            Token();
            Login_button = new DelegateCommand<string>(async (x) =>
            {
                switch (Username)
                {
                    case "生产员":
                        Lonin_Log = "登入生产员成功";
                        ExecutePermission(PermissionType.ProductionStaff);
                        break;
                    case "工程师":
                        if (PassWord == "2024")
                        {
                            Lonin_Log = "登入工程师成功";
                            ExecutePermission(PermissionType.Engineer);
                        }
                        else
                        {
                            Lonin_Log = "输入密码错误";
                            return;
                        }
                        break;
                    case "供应商":
                        if (PassWord == "2025")
                        {
                            Lonin_Log = "登入供应商成功";
                            ExecutePermission(PermissionType.Supplier);
                        }
                        else
                        {
                            Lonin_Log = "输入密码错误";
                            return;
                        }
                        break;
                    default:
                        break;
                }
                Upload = false;
                List<object> pageLoadparameter = new List<object>();
                await Task.Run(() =>
                {
                    Lonin_Log = "正在初始化......";
                    var system_list = ParameterJsonTool.GetJson();
                    pageLoadparameter.Add(system_list);
                    Thread_Auto_Base.NewClass();
                    InfoStructure info = new InfoStructure();
                    ProductionInformation.ReadProductionInfo(ref info);
                    pageLoadparameter.Add(info);
                });
                aggregators.GetEvent<PageLoadEvent>().Publish(pageLoadparameter.ToArray());
                LogInLoad?.Invoke((passtype) =>
                {
                    passtype.systemLog.Close();
                });
            });
            Close = new DelegateCommand(() =>
            {
                System.Environment.Exit(0);
            });
            AuthorizationShow = new DelegateCommand(() =>
            {
                AuthorizedRegistrationView registrationView = new AuthorizedRegistrationView(aggregators);
                registrationView.ShowDialog();
                Token();
            });
        }

        private void Token()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzgwNTQyMEAzMjM5MmUzMDJlMzAzYjMyMzkzYmhGTE1JZ2NlRlRtbG50bnllZ040L0pqMnN2am5qVkgva3RoRGhmS0xXQUU9");
            ClientKeyMaker.ClientToken clientToken = new ClientKeyMaker.ClientToken();
            clientToken.LoginEvent += ((b, t1, t2) =>
            {
                if (!b)
                {
                    Warranty = false;
                    System.Windows.Forms.MessageBox.Show($"未授权，请联系管理员进行授权!\r\n30分钟后将自动退出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    Warranty = true;
                }
            });
            clientToken.InAdvanceEvent += ((b, t1, t2) =>
            {

            });
            clientToken.ExpireEvent += ((b, t1, t2) =>
            {
                System.Windows.Forms.MessageBox.Show($"软件授权到期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemLogIn.viewModel.Warranty = null;
            });
            clientToken.Initialize();
            Lonin_Log = "剩余时间:" + clientToken.GetTimeRemaining();
            ViewModels.AuthorizedRegistrationViewModels.AuthorizedRegistrationViewModel.token = clientToken;
        }

        private void ExecutePermission(PermissionType permissionType)
        {
            MainWindowViewModel.PermissionCommand.Execute(new Permission() { type = permissionType });
        }

        public DelegateCommand AuthorizationShow { get; set; }

        private string _longin_log;
        /// <summary>
        /// 日志输出
        /// </summary>
        public string Lonin_Log
        {
            get { return _longin_log; }
            set { SetProperty(ref _longin_log, value); }
        }

        private string _title = "Sharp Frame";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _password = "666888";
        /// <summary>
        /// 输入密码
        /// </summary>
        public string PassWord
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _username;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private bool _client_on_line;

        public bool Client_On_Line
        {
            get { return _client_on_line; }
            set { SetProperty(ref _client_on_line, value); }
        }

        private object _warranty = null;
        /// <summary>
        /// 授权状态
        /// </summary>
        public object Warranty
        {
            get { return _warranty; }
            set { SetProperty(ref _warranty, value); }
        }
        private bool _upload = true;
        /// <summary>
        /// 加载标志位
        /// </summary>
        public bool Upload
        {
            get { return _upload; }
            set
            {
                if (value)
                {
                    UploadProgressBar = false;
                }
                else
                {
                    UploadProgressBar = true;
                }
                SetProperty(ref _upload, value);
            }
        }


        private bool _uploadprogressbar = false;
        public bool UploadProgressBar
        {
            get { return _uploadprogressbar; }
            set { SetProperty(ref _uploadprogressbar, value); }
        }
        public class Permission
        {
            public PermissionType type { get; set; }
            public SystemLogIn systemLog { get; set; }
        }

        public enum PermissionType
        {
            /// <summary>
            /// 工程师
            /// </summary>
            Engineer,
            /// <summary>
            /// 供应商
            /// </summary>
            Supplier,
            /// <summary>
            /// 生产员
            /// </summary>
            ProductionStaff
        }
    }
}
