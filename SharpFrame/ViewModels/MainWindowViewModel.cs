using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProductAssembly.FunctionCall;
using System.Windows;

namespace SharpFrame.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IEventAggregator eventAggregator;

        public DelegateCommand Close { get; set; }

        public static DelegateCommand<Permission> PermissionCommand { get; set; }

        public MainWindowViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            Close = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<Close_MessageEvent>().Publish();
                System.Environment.Exit(0);
            });
            PermissionCommand = new DelegateCommand<Permission>((t) =>
            {
                eventAggregator.GetEvent<LoginPermission>().Publish(t);
            });
        }

        #region 启动按钮
        private bool _start_state = false;
        /// <summary>
        /// 启动按钮状态
        /// </summary>
        public bool Start_State
        {
            get { return _start_state; }
            set
            {
                if (!_start_state && value)
                {
                    Log.Info("软启动按钮触发");
                    eventAggregator.GetEvent<StartInform>().Publish();
                }
                _start_state = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 暂停按钮
        private bool _suspend_state = false;
        /// <summary>
        /// 暂停按钮状态
        /// </summary>
        public bool Suspend_State
        {
            get { return _suspend_state; }
            set
            {
                if (!_suspend_state && value)
                {
                    Log.Info("软暂停按钮触发");
                    eventAggregator.GetEvent<SuspendInform>().Publish();
                }
                _suspend_state = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 停止按钮
        private bool _stop_state = true;
        /// <summary>
        /// 停止按钮状态
        /// </summary>
        public bool Stop_State
        {
            get { return _stop_state; }
            set
            {
                if (!_stop_state && value)
                {
                    Log.Info("软停止按钮触发");
                    eventAggregator.GetEvent<StopInform>().Publish();
                }
                _stop_state = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 复位按钮
        private bool _reset_state = false;
        /// <summary>
        /// 复位按钮状态
        /// </summary>
        public bool Reset_State
        {
            get { return _reset_state; }
            set
            {
                if (!_reset_state && value)
                {
                    Log.Info("软复位按钮触发");
                    LoadingBarState = true;
                    eventAggregator.GetEvent<ResetInform>().Publish();
                    LoadingBarState = false;
                }
                _reset_state = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private string _title = "Prism Application";
        /// <summary>
        /// 应用程序抬头
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }

        private bool _loadingbarstate = false;
        /// <summary>
        /// 加载条状态
        /// </summary>
        public bool LoadingBarState
        {
            get { return _loadingbarstate; }
            set { _loadingbarstate = value; RaisePropertyChanged(); }
        }

        #region 获取显示器分辨率
        private double _height = SystemParameters.PrimaryScreenHeight - 30;
        public double Height
        {
            get { return _height; }
            set { _height = value; RaisePropertyChanged(); }
        }

        private double _width = SystemParameters.PrimaryScreenWidth + 10;

        public double Width
        {
            get { return _width; }
            set { _width = value; RaisePropertyChanged(); }
        }
        #endregion
    }

    /// <summary>
    /// 应用程序关闭通知
    /// </summary>
    public class Close_MessageEvent : PubSubEvent { }
    /// <summary>
    /// 登入权限通知
    /// </summary>
    public class LoginPermission : PubSubEvent<Permission> { }
    /// <summary>
    /// 启动通知
    /// </summary>
    public class StartInform : PubSubEvent { }
    /// <summary>
    /// 暂停通知
    /// </summary>
    public class SuspendInform : PubSubEvent { }
    /// <summary>
    /// 停止通知
    /// </summary>
    public class StopInform : PubSubEvent { }
    /// <summary>
    /// 复位通知
    /// </summary>
    public class ResetInform : PubSubEvent { }

    public class Permission
    {
        public PermissionType type { get; set; }
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
