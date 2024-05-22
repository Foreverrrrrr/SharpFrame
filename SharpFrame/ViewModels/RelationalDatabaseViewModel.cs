using System;
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
                var t = SQL_Server.SpecialQuery<Data>("NewTable", Time.Date, Time.AddDays(1).Date);
            });
        }

        /// <summary>
        /// 数据库时间查询命令
        /// </summary>
        public DelegateCommand DatabaseTimeQuery { get; set; }

        private DateTime _time = DateTime.Now;

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; RaisePropertyChanged(); }
        }
    }

    public class Data
    {
        public string Time { get; set; }
        public string result { get; set; }
        public string value { get; set; }
    }
}
