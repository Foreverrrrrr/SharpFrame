using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SharpFrame.Common;
using SharpFrame.log4Net;
using SharpFrame.Logic.Base;
using SharpFrame.Views;
using SharpFrame.Views.SharpStyle;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static SharpFrame.Logic.Base.Thread_Auto_Base;
using static SharpFrame.ViewModels.SystemLogInViewModel;

namespace SharpFrame.ViewModels
{
    public delegate bool LogicStates();

    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private readonly IRegionManager regionManager;

        public LogicStates ButtonLogic { get; set; }

        /// <summary>
        /// 页面导航Command
        /// </summary>
        public DelegateCommand<string> VisionSwitching { get; set; }

        /// <summary>
        /// 软件关闭前Command
        /// </summary>
        public DelegateCommand Close { get; set; }

        /// <summary>
        /// 页面初始化加载完成Command
        /// </summary>
        public DelegateCommand PageLoadFinish { get; set; }

        /// <summary>
        /// 登入权限Command
        /// </summary>
        public static DelegateCommand<Permission> PermissionCommand { get; set; }

        public MainWindowViewModel(IEventAggregator aggregator, IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.eventAggregator = aggregator;
            eventAggregator.GetEvent<PageLoadEvent>().Subscribe((classobj) =>
            {
                Stop_State = true;
                aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "正常", Value = "程序加载完成" });
                Thread_Auto_Base.NewClass_RunEvent += (t, s, ob) =>
                {
                    aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = t.ToString(), Level = "正常", Value = s });
                };
                Thread_Auto_Base.DataConfigurationEvent += ((t, s) =>
                {
                    aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = t.ToString(), Level = "正常", Value = s });
                });
            });
            aggregator.GetEvent<Notification>().Subscribe((t) =>
            {
                NotificationModel model = null;
                switch (t.Type)
                {
                    case Notification.InfoType.Info:
                        model = new NotificationInfoModel() { ID = IsNotice.Count, Message = t.Message, MessageTime = t.MessageTime };
                        break;
                    case Notification.InfoType.Warning:
                        model = new NotificationWarningModel() { ID = IsNotice.Count, Message = t.Message, MessageTime = t.MessageTime };
                        break;
                    case Notification.InfoType.Error:
                        model = new NotificationErrorModel()
                        {
                            ID = IsNotice.Count,
                            Message = t.Message,
                            MessageTime = t.MessageTime,
                        };
                        model.Delete = new DelegateCommand<object>((obj) =>
                        {
                            var rmove = IsNotice.Where(x => x.ID == Convert.ToInt32(obj)).First();
                            IsNotice.Remove(rmove);
                        });
                        break;
                    case Notification.InfoType.Fatal:
                        model = new NotificationFatalModel() { ID = IsNotice.Count, Message = t.Message, MessageTime = t.MessageTime };
                        break;
                }
                IsNotice.Insert(IsNotice.Count, model);
            }, ThreadOption.UIThread);
            VisionSwitching = new DelegateCommand<string>((ManagerName) =>
            {
                aggregator.GetEvent<Notification>().Publish(new Notification() { Type = Notification.InfoType.Error, Message = ManagerName });
                try
                {
                    regionManager.Regions["MainRegion"].RequestNavigate(ManagerName);
                }
                catch (Exception ex)
                {

                }
            });
            Close = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<Close_MessageEvent>().Publish();
                //System.Environment.Exit(0);
                Application.Current.Shutdown();
            });
            PermissionCommand = new DelegateCommand<Permission>((t) =>
            {
                eventAggregator.GetEvent<LoginPermission>().Publish(t);
            });
            aggregator.GetEvent<Loadingbar>().Subscribe((t) =>
            {
                if (t)
                    LoadingBarState = true;
                else
                    LoadingBarState = false;
            });
            IsNotice.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is NotificationModel timedItem)
                        {
                            timedItem.AutoRemoveRequested += (sender, args) =>
                                Application.Current.Dispatcher.Invoke(() => IsNotice.Remove((NotificationModel)item));
                        }
                    }
                }
            };
            Viewinitial();
        }

        /// <summary>
        /// 页面初始化加载
        /// </summary>
        private void Viewinitial()
        {
            regionManager.RegisterViewWithRegion("MainRegion", typeof(ParameterView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(DebuggingView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(RelationalDatabaseView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(LogView));
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
                if (!_start_state && value && Exchange.External_IO(Send_Variable.Start))
                {

                    Log.Info("启动按钮触发");
                    eventAggregator.GetEvent<StartInform>().Publish();
                    _start_state = value;
                    RaisePropertyChanged();
                    SystemState = "自动运行";
                }
                else if (!value)
                {
                    _start_state = value;
                    RaisePropertyChanged();
                }
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
                if (!_suspend_state && value && Exchange.External_IO(Send_Variable.Suspend))
                {
                    Log.Info("暂停按钮触发");
                    eventAggregator.GetEvent<SuspendInform>().Publish();
                    _suspend_state = value;
                    RaisePropertyChanged();
                    SystemState = "暂停";
                }
                else if (!value)
                {
                    _suspend_state = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 停止按钮
        private bool _stop_state;
        /// <summary>
        /// 停止按钮状态
        /// </summary>
        public bool Stop_State
        {
            get { return _stop_state; }
            set
            {
                if (!_stop_state && value && Exchange.External_IO(Send_Variable.Stop))
                {
                    Log.Info("停止按钮触发");
                    eventAggregator.GetEvent<StopInform>().Publish();
                    _stop_state = value;
                    RaisePropertyChanged();
                    SystemState = "停止";
                }
                else if (!value)
                {
                    _stop_state = value;
                    RaisePropertyChanged();
                }
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
                if (!_reset_state && value && Exchange.External_IO(Send_Variable.Reset))
                {
                    SystemState = "复位中";
                    Log.Info("复位按钮触发");
                    LoadingBarState = true;
                    eventAggregator.GetEvent<ResetInform>().Publish();
                    LoadingBarState = false;
                    _reset_state = value;
                    RaisePropertyChanged();
                    SystemState = "待启动";
                }
                else if (!value)
                {
                    _reset_state = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 紧急停止按钮
        private bool _urgencystop_state;
        /// <summary>
        /// 紧急停止按钮状态
        /// </summary>
        public bool UrgencyStop_State
        {
            get { return _urgencystop_state; }
            set { _urgencystop_state = value; }
        }
        #endregion

        private string _title = "Prism Application";
        /// <summary>
        /// 应用程序抬头
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _loadingbarstate = false;
        /// <summary>
        /// 滑动加载条状态
        /// </summary>
        public bool LoadingBarState
        {
            get { return _loadingbarstate; }
            set { SetProperty(ref _loadingbarstate, value); }
        }

        private string _system_state;

        public string SystemState
        {
            get { return _system_state; }
            set { SetProperty(ref _system_state, value); }
        }

        private ObservableCollection<NotificationModel> _isNotice = new ObservableCollection<NotificationModel>();
        /// <summary>
        /// 悬浮弹窗集合
        /// </summary>
        public ObservableCollection<NotificationModel> IsNotice
        {
            get { return _isNotice; }
            set { SetProperty(ref _isNotice, value); }
        }

        #region 获取显示器分辨率
        private double _height = SystemParameters.PrimaryScreenHeight - 30;
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private double _width = SystemParameters.PrimaryScreenWidth + 10;

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }
        #endregion
    }

    /// <summary>
    /// 页面初始化完成通知 object[]初始化对象
    /// </summary>
    public class PageLoadEvent : PubSubEvent<object[]> { }
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
    /// <summary>
    /// 进度条动画通知
    /// </summary>
    public class Loadingbar : PubSubEvent<bool> { }
}
