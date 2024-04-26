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
                    for (int i = 0; i < system_list.Count; i++)
                    {
                        ParameterNameList.Add(new ComboxList { ID = i, Name = system_list[i] });
                    }
                    ParameterIndexes = index;
                    Parameter systems = new Parameter();
                    ParameterJsonTool.ReadJson(system_list[index], ref systems);
                    SystemArguments = new ObservableCollection<SystemParameter>(systems.SystemParameters_Obse.ToList());
                }
                else
                {
                    Parameter parameter = new Parameter();
                    parameter.DefaultModel = false;
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
                    ParameterJsonTool.NewJosn(DateTime.Now.ToString("yyyy_MM_dd"));
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
                SystemArguments = systems.SystemParameters_Obse;
            });
            ParameterSave = new DelegateCommand(() =>
            {
                var t = SystemArguments;
                foreach (var item in t)
                {
                    try
                    {
                        Type targetType = item.ValueType;
                        if (targetType != null)
                        {
                            object convertedValue = Convert.ChangeType(item.Value, targetType);
                        }
                        else
                        {
                            throw new Exception($"系统参数中输入值“{item.Value}”与“{item.ValueTypeName}”类型不符！");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"系统参数中输入值“{item.Value}”与“{item.ValueTypeName}”类型不符！");
                    }
                }
            });
            SystemArguments_Add_Line = new DelegateCommand<SystemParameter>((checkdata) =>
            {
                System_AddView system = new System_AddView(aggregator, checkdata);
                system.Show();
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
        }

        public DelegateCommand NewModel { get; set; }

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
    public class SystemParameterAddEvent : PubSubEvent<Add_Ins> { }

    public class Add_Ins
    {
        public SystemParameter NewParameter { get; set; }

        public SystemParameter InsertionParameter { get; set; }
    }
}
