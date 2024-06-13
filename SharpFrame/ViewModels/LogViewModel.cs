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
                Log.Info(x);
                if (MainLog.Count < 200)
                {
                    MainLog.Insert(0, x);
                }
                else
                {
                    MainLog.RemoveAt(MainLog.Count - 1);
                    MainLog.Insert(0, x);
                }
            });
        }

        private ObservableCollection<MainLogStructure> _mainlog = new ObservableCollection<MainLogStructure>();

        public ObservableCollection<MainLogStructure> MainLog
        {
            get { return _mainlog; }
            set { _mainlog = value; RaisePropertyChanged(); }
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
