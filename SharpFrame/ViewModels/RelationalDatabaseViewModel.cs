using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SharpFrame.Common;
using SharpFrame.Structure.Parameter;

namespace SharpFrame.ViewModels
{
    public class RelationalDatabaseViewModel : BindableBase
    {
        private IEventAggregator aggregator;

        private IRegionManager regionManager;

        public RelationalDatabaseViewModel(IEventAggregator eventaggregator, IRegionManager regionManager)
        {
            this.aggregator = eventaggregator;
            eventaggregator.GetEvent<ParameterUpdateEvent>().Subscribe((t) =>
            {
                SQL_Server.ConnectionString_Default = ParameterJsonTool.GetSystemValue<string>(t.SystemParameters_Obse[1]);

            }, ThreadOption.BackgroundThread);
            DatabaseTimeQuery = new DelegateCommand(() =>
            {
                aggregator.GetEvent<Loadingbar>().Publish(true);
                Data data = new Data();
                data.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                data.result = "pass";
                data.value = "312";
                SQL_Server.Write(data, "NewTable");
                var t = SQL_Server.SpecialQuery<Data>("NewTable", Time.Date, Time.AddDays(1).Date);
                DatabaseView = ParameterJsonTool.ConvertOBse(t);
                aggregator.GetEvent<Loadingbar>().Publish(false);
            });
            Export = new DelegateCommand<object>((obj) =>
            {
                aggregator.GetEvent<Loadingbar>().Publish(true);
                ObservableCollection<Data> data = obj as ObservableCollection<Data>;
                if (data != null)
                {
                    System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                    dialog.Description = "请选择文件路径";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExcelTool.WriteExcel(dialog.SelectedPath, DateTime.Now.ToString("yyyyMMddHHmmss"), data);
                    }
                    else
                    {

                    }
                }
                aggregator.GetEvent<Loadingbar>().Publish(false);
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
