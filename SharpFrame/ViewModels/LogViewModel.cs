using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SharpFrame.log4Net;
using System.Collections.ObjectModel;

namespace SharpFrame.ViewModels
{
    public class LogViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private readonly IRegionManager regionManager;

        public LogViewModel(IEventAggregator aggregator, IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.eventAggregator = aggregator;
            eventAggregator.GetEvent<MainLogOutput>().Subscribe((MainLogStructure x) =>
            {
                MainLogStructure mainLog = new MainLogStructure();
                mainLog.Time = x.Time;
                mainLog.Level = x.Level;
                mainLog.Value = x.Value;
                Log.Info(mainLog);
                if (MainLog.Count < 200)
                {
                    MainLog.Insert(0, mainLog);
                }
                else
                {
                    MainLog.RemoveAt(MainLog.Count - 1);
                    MainLog.Insert(0, mainLog);
                }
            }, ThreadOption.UIThread);
        }

        private ObservableCollection<MainLogStructure> _mainlog = new ObservableCollection<MainLogStructure>();

        public ObservableCollection<MainLogStructure> MainLog
        {
            get { return _mainlog; }
            set { SetProperty(ref _mainlog, value); }
        }
    }

    /// <summary>
    /// 全局日志输出
    /// </summary>
    public class MainLogOutput : PubSubEvent<MainLogStructure> { }

    public class MainLogStructure
    {
        public string Time { get; set; }
        public string Level { get; set; }
        public string Value { get; set; }
    }
}
