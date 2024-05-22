using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Common;
using SharpFrame.Structure.Parameter;

namespace SharpFrame.ViewModels
{
    public class RelationalDatabaseViewModel : BindableBase
    {
        IEventAggregator aggregator;
        public RelationalDatabaseViewModel(IEventAggregator eventaggregator)
        {
            this.aggregator = eventaggregator;
            eventaggregator.GetEvent<ParameterUpdateEvent>().Subscribe((t) =>
            {
                SQL_Server.ConnectionString_Default = ParameterJsonTool.GetSystemValue<string>(t.SystemParameters_Obse[1]);

            }, ThreadOption.BackgroundThread);
            DatabaseTimeQuery = new DelegateCommand(() =>
            {
                Data data = new Data();
                data.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                data.result = "pass";
                data.value = "312";
                SQL_Server.Write(data, "NewTable");
                var t = SQL_Server.SpecialQuery<Data>("NewTable", Time.Date, Time.AddDays(1).Date);
                DatabaseView = ParameterJsonTool.ConvertOBse(t);
            });
            Export = new DelegateCommand<object>((obj) =>
            {
                ObservableCollection<Data> data = obj as ObservableCollection<Data>;
                if (data != null)
                {

                }
            });
        }

        /// <summary>
        /// 数据库时间查询命令
        /// </summary>
        public DelegateCommand DatabaseTimeQuery { get; set; }

        /// <summary>
        /// 数据导出Excel命令
        /// </summary>
        public DelegateCommand<object> Export { get; set; }

        private DateTime _time = DateTime.Now;

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<Data> _databaseview;

        public ObservableCollection<Data> DatabaseView
        {
            get { return _databaseview; }
            set { _databaseview = value; RaisePropertyChanged(); }
        }

    }

    public class Data
    {
        public string Time { get; set; }
        public string result { get; set; }
        public string value { get; set; }
    }
}
