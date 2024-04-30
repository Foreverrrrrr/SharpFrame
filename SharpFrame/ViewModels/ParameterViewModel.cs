using ImTools;
using Prism.Commands;
using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.ParameterJson;
using SharpFrame.ViewModels.ToolViewModels;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFrame.ViewModels
{
    public class ParameterViewModel : BindableBase
    {
        IEventAggregator aggregator;

        public ParameterViewModel(IEventAggregator eventaggregator)
        {
            aggregator = eventaggregator;
            aggregator.GetEvent<LoginPermission>().Subscribe((type) =>
            {

            });
            aggregator.GetEvent<PageLoadEvent>().Subscribe(() =>
            {
                var system_list = ParameterJsonTool.GetJson();
                if (system_list.Count > 0)
                {
                    var all_parameters = ParameterJsonTool.ReadAllJson(new Parameter());
                    int index = all_parameters.Select((item, idx) => new { Item = item, Index = idx }).FirstOrDefault(x => x.Item.DefaultModel == true)?.Index ?? -1;
                    ParameterName = all_parameters[index].ParameterName;
                    for (int i = 0; i < system_list.Count; i++)
                    {
                        ParameterNameList.Add(new ComboxList { ID = i, Name = system_list[i] });
                    }
                    ParameterIndexes = index;
                    Parameter systems = new Parameter();
                    ParameterJsonTool.ReadJson(system_list[index], ref systems);
                    SystemArguments = new ObservableCollection<SystemParameter>(systems.SystemParameters_Obse.ToList());
                    PointLocationArguments = new ObservableCollection<PointLocationParameter>(systems.PointLocationParameter_Obse.ToList());
                }
                else
                {
                    Parameter parameter = new Parameter();
                    parameter.DefaultModel = false;
                    parameter.ParameterName = DateTime.Now.ToString("yyyy_MM_dd");
                    parameter.SystemParameters_Obse.Add(new SystemParameter(0, "0", 0.211f));
                    parameter.SystemParameters_Obse.Add(new SystemParameter(1, "1", 131.00));
                    parameter.SystemParameters_Obse.Add(new SystemParameter(2, "2", "dsadas"));
                    parameter.PointLocationParameter_Obse.Add(new PointLocationParameter() { ID = 0, Name = "安全点", Enable = true });
                    parameter.PointLocationParameter_Obse.Add(new PointLocationParameter() { ID = 1, Name = "P1", Enable = true });
                    parameter.PointLocationParameter_Obse.Add(new PointLocationParameter() { ID = 2, Name = "P2", Enable = true });
                    parameter.TestParameter_Obse.Add(new TestParameter() { ID = 0, Name = "0", Value = 0.211f });
                    parameter.TestParameter_Obse.Add(new TestParameter() { ID = 1, Name = "1", Value = 0.333 });
                    parameter.TestParameter_Obse.Add(new TestParameter() { ID = 2, Name = "2", Value = "sasda" });
                    ParameterJsonTool.Set_NullJson(parameter);
                    ParameterJsonTool.NewJosn(DateTime.Now.ToString(parameter.ParameterName));
                }
                system_list = ParameterJsonTool.GetJson();
            });
            aggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {

            });
            ModelSwitching = new DelegateCommand(() =>
            {
                Parameter systems = new Parameter();
                var system_list = ParameterJsonTool.GetJson();
                ParameterJsonTool.ReadJson(system_list[ParameterIndexes], ref systems);
                ParameterName = systems.ParameterName;
                SystemArguments = systems.SystemParameters_Obse;
                PointLocationArguments = systems.PointLocationParameter_Obse;
            });
            ParameterSave = new DelegateCommand(() =>
            {
                Parameter parameter = new Parameter();
                parameter.ParameterName = ParameterName;
                parameter.DefaultModel = true;
                parameter.SystemParameters_Obse = SystemArguments;
                parameter.PointLocationParameter_Obse = PointLocationArguments;
                ParameterJsonTool.WriteJson(ParameterName, parameter);
                MessageBox.Show($"“{ParameterName}”参数保存完成");
            });
            SystemArguments_Add_Line = new DelegateCommand<SystemParameter>((checkdata) =>
            {
                if (checkdata != null)
                {
                    System_AddView system = new System_AddView(aggregator, checkdata);
                    system.Show();
                }
            });
            SystemArguments_Remove_Line = new DelegateCommand<SystemParameter>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    SystemArguments.Remove(checkdata);
                    ObservableCollection<SystemParameter> systemStructures = new ObservableCollection<SystemParameter>();
                    int index = 0;
                    foreach (SystemParameter item in SystemArguments.Select(x => new SystemParameter(x)))
                    {
                        item.ID = index;
                        systemStructures.Add(item);
                        index++;
                    }
                    SystemArguments = null;
                    SystemArguments = systemStructures;
                }
            });
            PointLocationArguments_Add_Line = new DelegateCommand<PointLocationParameter>((checkdata) =>
            {
                if (checkdata != null)
                {
                    Point_AddView addView = new Point_AddView(aggregator, checkdata);
                    addView.Show();
                }
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
            aggregator.GetEvent<SystemParameterAddEvent>().Subscribe((t) =>
            {
                var k = SystemArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
                SystemArguments.Insert(k, t.NewParameter);
                ObservableCollection<SystemParameter> systemStructures = new ObservableCollection<SystemParameter>();
                int index = 0;
                foreach (SystemParameter item in SystemArguments.Select(x => new SystemParameter(x)))
                {
                    item.ID = index;
                    systemStructures.Add(item);
                    index++;
                }
                SystemArguments = null;
                SystemArguments = systemStructures;
            }, ThreadOption.UIThread);

            aggregator.GetEvent<PointLocationParameterAddEvent>().Subscribe((t) =>
            {
                var k = PointLocationArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
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
            }, ThreadOption.UIThread);
        }

        public DelegateCommand NewModel { get; set; }

        #region 系统参数表

        /// <summary>
        /// 系统参数添加行请求
        /// </summary>
        public DelegateCommand<SystemParameter> SystemArguments_Add_Line { get; set; }

        /// <summary>
        /// 系统参数移除行请求
        /// </summary>
        public DelegateCommand<SystemParameter> SystemArguments_Remove_Line { get; set; }

        private ObservableCollection<SystemParameter> _systemarguments = new ObservableCollection<SystemParameter>();
        /// <summary>
        /// 系统参数表
        /// </summary>
        public ObservableCollection<SystemParameter> SystemArguments
        {
            get { return _systemarguments; }
            set { _systemarguments = value; RaisePropertyChanged(); }
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
            set { _pointlocationarguments = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 测试判定表

        #endregion

        private int _parameterindexes = 0;
        /// <summary>
        /// 参数型号选择ComboBox
        /// </summary>
        public int ParameterIndexes
        {
            get { return _parameterindexes; }
            set { _parameterindexes = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ComboxList> _parameterNameList = new ObservableCollection<ComboxList>();
        /// <summary>
        /// 参数型号选择集合
        /// </summary>
        public ObservableCollection<ComboxList> ParameterNameList
        {
            get { return _parameterNameList; }
            set { _parameterNameList = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 型号切换DropDownClosed命令
        /// </summary>
        public DelegateCommand ModelSwitching { get; set; }

        /// <summary>
        /// 参数保存
        /// </summary>
        public DelegateCommand ParameterSave { get; set; }

        public string ParameterName { get; set; }
    }

    public struct ComboxList
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    /// <summary>
    /// 程序参数
    /// </summary>
    public class Parameter : BindableBase
    {
        public Parameter()
        {
            SystemParameters_Obse = new ObservableCollection<SystemParameter>();
            PointLocationParameter_Obse = new ObservableCollection<PointLocationParameter>();
            TestParameter_Obse = new ObservableCollection<TestParameter>();
        }
        public string ParameterName { get; set; }

        public bool DefaultModel { get; set; }
        private ObservableCollection<SystemParameter> _systemparameters_obse;

        public ObservableCollection<SystemParameter> SystemParameters_Obse
        {
            get { return _systemparameters_obse; }
            set { _systemparameters_obse = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<PointLocationParameter> _pointLocationparameter_obse;

        public ObservableCollection<PointLocationParameter> PointLocationParameter_Obse
        {
            get { return _pointLocationparameter_obse; }
            set { _pointLocationparameter_obse = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<TestParameter> _testParameter_obse;

        public ObservableCollection<TestParameter> TestParameter_Obse
        {
            get { return _testParameter_obse; }
            set { _testParameter_obse = value; RaisePropertyChanged(); }
        }
    }

    /// <summary>
    /// 系统参数结构
    /// </summary>
    public class SystemParameter
    {
        public SystemParameter() { }

        public SystemParameter(SystemParameter system)
        {
            if (system != null)
            {
                this.ID = system.ID;
                this.Name = system.Name;
                this.Value = system.Value;
            }
        }

        public SystemParameter(int iD, string name, object value)
        {
            ID = iD;
            Name = name;
            Value = value;
            ValueType = Value.GetType();
            ValueTypeName = ValueType.Name;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        private object value;
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public Type ValueType { get; set; }

        public string ValueTypeName { get; set; }
    }

    /// <summary>
    /// 点位参数结构
    /// </summary>
    public class PointLocationParameter
    {
        public PointLocationParameter(PointLocationParameter system)
        {
            if (system != null)
            {
                this.ID = system.ID;
                this.Name = system.Name;
                this.Enable = system.Enable;
                this.PointA = system.PointA;
                this.PointB = system.PointB;
                this.PointC = system.PointC;
                this.PointD = system.PointD;
                this.PointE = system.PointE;
                this.PointF = system.PointF;
            }
        }

        public PointLocationParameter()
        {

        }
        public int ID { get; set; }

        public string Name { get; set; }

        public bool Enable { get; set; }

        public double PointA { get; set; }

        public double PointB { get; set; }

        public double PointC { get; set; }

        public double PointD { get; set; }

        public double PointE { get; set; }

        public double PointF { get; set; }
    }

    /// <summary>
    /// 测试参数结构
    /// </summary>
    public class TestParameter
    {
        public TestParameter() { }

        public TestParameter(TestParameter system)
        {
            if (system != null)
            {
                this.ID = system.ID;
                this.Name = system.Name;
                this.Value = system.Value;
            }
        }

        public int ID { get; set; }

        public string Name { get; set; }

        private object value;
        public object Value
        {
            get { return value; }
            set
            {
                this.value = value;
                ValueType = value.GetType();
                ValueTypeName = ValueType.Name;
            }
        }

        public string ValueTypeName { get; set; }

        public Type ValueType { get; set; }
    }

    /// <summary>
    /// 系统参数添加行通知
    /// </summary>
    public class SystemParameterAddEvent : PubSubEvent<Add_SystemIns> { }

    public class Add_SystemIns
    {
        public SystemParameter NewParameter { get; set; }

        public SystemParameter InsertionParameter { get; set; }
    }

    /// <summary>
    /// 点位参数添加行通知
    /// </summary>
    public class PointLocationParameterAddEvent : PubSubEvent<Add_PointLocationIns> { }

    public class Add_PointLocationIns
    {
        public PointLocationParameter NewParameter { get; set; }

        public PointLocationParameter InsertionParameter { get; set; }
    }
}
