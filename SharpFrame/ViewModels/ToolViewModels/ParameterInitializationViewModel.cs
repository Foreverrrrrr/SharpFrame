using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Structure.Parameter;
using SharpFrame.Views.ToolViews;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SharpFrame.ViewModels.ToolViewModels
{
    public class ParameterInitializationViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        public ParameterInitializationViewModel(IEventAggregator aggregator)
        {
            this.eventAggregator = aggregator;
            Create = new DelegateCommand(() =>
            {
                Parameter parameter = new Parameter();
                var path = System.Environment.CurrentDirectory + @"\Parameter";
                if (Directory.Exists(path))
                {
                    DirectoryInfo root = new DirectoryInfo(path);
                    FileInfo[] files = root.GetFiles();
                    if (files.Length > 0)
                        if (MessageBox.Show($"程序根目录“Parameter”文件夹中已经存在参数文件，是否重新生成默认参数？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                        {
                            parameter.SystemParameters_Obse = SystemArguments;
                            parameter.PointLocationParameter_Obse = PointLocationArguments;
                            parameter.TestParameter_Obse = TestParameterArguments;
                            ParameterJsonTool.Set_NullJson(parameter);
                        }
                }
                else
                {
                    parameter.SystemParameters_Obse = SystemArguments;
                    parameter.PointLocationParameter_Obse = PointLocationArguments;
                    parameter.TestParameter_Obse = TestParameterArguments;
                    ParameterJsonTool.Set_NullJson(parameter);
                }
            });
            SystemArguments_Add_Line = new DelegateCommand<Structure.Parameter.SystemParameter>((checkdata) =>
            {
                System_AddView system = new System_AddView(aggregator, checkdata, "new");
                system.Show();
            });
            SystemArguments_Remove_Line = new DelegateCommand<Structure.Parameter.SystemParameter>((removedata) =>
            {
                if (MessageBox.Show($"是否移除名称：{removedata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    SystemArguments.Remove(removedata);
                    ObservableCollection<Structure.Parameter.SystemParameter> systemStructures = new ObservableCollection<Structure.Parameter.SystemParameter>();
                    int index = 0;
                    foreach (Structure.Parameter.SystemParameter item in SystemArguments.Select(x => new Structure.Parameter.SystemParameter(x)))
                    {
                        item.ID = index;
                        systemStructures.Add(item);
                        index++;
                    }
                    SystemArguments = null;
                    SystemArguments = systemStructures;
                }
            });
            aggregator.GetEvent<SystemParameterAddEvent>().Subscribe((t) =>
            {
                int k = 0;
                if (t.InsertionParameter != null)
                {
                    k = SystemArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
                }
                SystemArguments.Insert(k, t.NewParameter);
                ObservableCollection<ZX24010DualTrackLaserMarking.Structure.Parameter.SystemParameter> systemStructures = new ObservableCollection<ZX24010DualTrackLaserMarking.Structure.Parameter.SystemParameter>();
                int index = 0;
                foreach (ZX24010DualTrackLaserMarking.Structure.Parameter.SystemParameter item in SystemArguments.Select(x => new ZX24010DualTrackLaserMarking.Structure.Parameter.SystemParameter(x)))
                {
                    item.ID = index;
                    systemStructures.Add(item);
                    index++;
                }
                SystemArguments = null;
                SystemArguments = systemStructures;
            }, ThreadOption.UIThread, false, (filtration) => filtration.FiltrationModel == "new");

            PointLocationArguments_Add_Line = new DelegateCommand<PointLocationParameter>((checkdata) =>
            {
                Point_AddView addView = new Point_AddView(aggregator, checkdata, "new");
                addView.Show();
            });
            PointLocationArguments_Remove_Line = new DelegateCommand<PointLocationParameter>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    PointLocationArguments.Remove(checkdata);
                    ObservableCollection<PointLocationParameter> pointStructures = new ObservableCollection<PointLocationParameter>();
                    int index = 0;
                    foreach (PointLocationParameter item in PointLocationArguments.Select(x => new PointLocationParameter(x)))
                    {
                        item.ID = index;
                        pointStructures.Add(item);
                        index++;
                    }
                    PointLocationArguments = null;
                    PointLocationArguments = pointStructures;
                }
            });
            aggregator.GetEvent<PointLocationParameterAddEvent>().Subscribe((t) =>
            {
                int k = 0;
                if (t.InsertionParameter != null)
                {
                    k = PointLocationArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
                }
                PointLocationArguments.Insert(k, t.NewParameter);
                ObservableCollection<PointLocationParameter> pointStructures = new ObservableCollection<PointLocationParameter>();
                int index = 0;
                foreach (PointLocationParameter item in PointLocationArguments.Select(x => new PointLocationParameter(x)))
                {
                    item.ID = index;
                    pointStructures.Add(item);
                    index++;
                }
                PointLocationArguments = null;
                PointLocationArguments = pointStructures;
            }, ThreadOption.UIThread, false, (filtration) => filtration.FiltrationModel == "new");
            TestParameterArguments_Add_Line = new DelegateCommand<TestParameter>((checkdata) =>
            {
                Test_AddView addView = new Test_AddView(aggregator, checkdata, "new");
                addView.Show();
            });
            TestParameterArguments_Remove_Line = new DelegateCommand<TestParameter>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    TestParameterArguments.Remove(checkdata);
                    ObservableCollection<TestParameter> pointStructures = new ObservableCollection<TestParameter>();
                    int index = 0;
                    foreach (TestParameter item in TestParameterArguments.Select(x => new TestParameter(x)))
                    {
                        item.ID = index;
                        pointStructures.Add(item);
                        index++;
                    }
                    TestParameterArguments = null;
                    TestParameterArguments = pointStructures;
                }
            });
            aggregator.GetEvent<TestParameterAddEvent>().Subscribe((t) =>
            {
                int k = 0;
                if (t.InsertionParameter != null)
                {
                    k = TestParameterArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
                }
                TestParameterArguments.Insert(k, t.NewParameter);
                ObservableCollection<TestParameter> pointStructures = new ObservableCollection<TestParameter>();
                int index = 0;
                foreach (TestParameter item in TestParameterArguments.Select(x => new TestParameter(x)))
                {
                    item.ID = index;
                    pointStructures.Add(item);
                    index++;
                }
                TestParameterArguments = null;
                TestParameterArguments = pointStructures;
            }, ThreadOption.UIThread, false, (filtration) => filtration.FiltrationModel == "new");
        }

        /// <summary>
        /// 参数生成
        /// </summary>
        public DelegateCommand Create { get; set; }

        #region 系统参数表
        /// <summary>
        /// 系统参数添加行请求
        /// </summary>
        public DelegateCommand<Structure.Parameter.SystemParameter> SystemArguments_Add_Line { get; set; }

        /// <summary>
        /// 系统参数移除行请求
        /// </summary>
        public DelegateCommand<Structure.Parameter.SystemParameter> SystemArguments_Remove_Line { get; set; }

        private ObservableCollection<Structure.Parameter.SystemParameter> _systemarguments = new ObservableCollection<Structure.Parameter.SystemParameter>();
        /// <summary>
        /// 系统参数表
        /// </summary>
        public ObservableCollection<Structure.Parameter.SystemParameter> SystemArguments
        {
            get { return _systemarguments; }
            set { SetProperty(ref _systemarguments, value); }
        }
        #endregion

        #region 点位表

        /// <summary>
        /// 点位参数添加行请求
        /// </summary>
        public DelegateCommand<PointLocationParameter> PointLocationArguments_Add_Line { get; set; }

        /// <summary>
        /// 点位参数移除行请求
        /// </summary>
        public DelegateCommand<PointLocationParameter> PointLocationArguments_Remove_Line { get; set; }

        private ObservableCollection<PointLocationParameter> _pointlocationarguments = new ObservableCollection<PointLocationParameter>();
        /// <summary>
        /// 点位参数表
        /// </summary>
        public ObservableCollection<PointLocationParameter> PointLocationArguments
        {
            get { return _pointlocationarguments; }
            set { SetProperty(ref _pointlocationarguments, value); }
        }
        #endregion

        #region 测试判定表
        /// <summary>
        /// 测试参数添加行请求
        /// </summary>
        public DelegateCommand<TestParameter> TestParameterArguments_Add_Line { get; set; }

        /// <summary>
        /// 测试参数移除行请求
        /// </summary>
        public DelegateCommand<TestParameter> TestParameterArguments_Remove_Line { get; set; }

        private ObservableCollection<TestParameter> _testparameterarguments = new ObservableCollection<TestParameter>();
        /// <summary>
        /// 测试参数表
        /// </summary>
        public ObservableCollection<TestParameter> TestParameterArguments
        {
            get { return _testparameterarguments; }
            set { SetProperty(ref _testparameterarguments, value); }
        }
        #endregion
    }
}
