using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.ParameterJson;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
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
                    for (int i = 0; i < system_list.Count; i++)
                    {
                        ParameterNameList.Add(new ComboxList { ID = i, Name = system_list[i] });
                    }
                    string paramValue = GetParameterValue();
                    Parameter systems = new Parameter();
                    if (paramValue != "Null")
                    {
                        ParameterName = paramValue;
                        ParameterJsonTool.ReadJson(paramValue, ref systems);
                        ParameterIndexes = system_list.FindIndex(x => x == paramValue);
                        SystemArguments = new ObservableCollection<SystemParameter>(systems.SystemParameters_Obse.ToList());
                        PointLocationArguments = new ObservableCollection<PointLocationParameter>(systems.PointLocationParameter_Obse.ToList());
                    }
                    else
                    {
                        ParameterName = system_list[0];
                        ParameterJsonTool.ReadJson(ParameterName, ref systems);
                        ParameterIndexes = 0;
                        SystemArguments = new ObservableCollection<SystemParameter>(systems.SystemParameters_Obse.ToList());
                        PointLocationArguments = new ObservableCollection<PointLocationParameter>(systems.PointLocationParameter_Obse.ToList());
                    }
                }
                else
                {
                    Parameter parameter = new Parameter();
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
                ParameterName = system_list[ParameterIndexes];
                SetParameterValue(ParameterName);
                ParameterJsonTool.ReadJson(ParameterName, ref systems);
                SystemArguments = systems.SystemParameters_Obse;
                PointLocationArguments = systems.PointLocationParameter_Obse;
            });
            ParameterSave = new DelegateCommand(() =>
            {
                Parameter parameter = new Parameter();
                parameter.SystemParameters_Obse = SystemArguments;
                foreach (var item in parameter.SystemParameters_Obse)
                {
                    var bool_ret = TypeAndValueCheck(item);
                    if (!bool_ret)
                    {
                        MessageBox.Show($"保存失败\r\n参数名称“{item.Name}”中值“{item.Value}”无法转换为类型“{item.ValueType.Name}”");
                        return;
                    }
                }
                parameter.PointLocationParameter_Obse = PointLocationArguments;
                ParameterJsonTool.WriteJson(ParameterName, parameter);
                MessageBox.Show($"“{ParameterName}”参数保存完成");
            });
            ParameterDelete = new DelegateCommand(() =>
            {

            });
            NewModel = new DelegateCommand(() =>
            {
                var parameter_list = ParameterJsonTool.GetJson();
                NewParameterModelView modelView = new NewParameterModelView(aggregator, parameter_list);
                modelView.Show();
            });
            aggregator.GetEvent<NewModelEvent>().Subscribe((model) =>
            {
                Parameter par = new Parameter();
                ParameterJsonTool.NewJosn(model.NewName, model.InitialParameter);
                var parameter_list = ParameterJsonTool.GetJson();
                ParameterNameList.Clear();
                for (int i = 0; i < parameter_list.Count; i++)
                {
                    ParameterNameList.Add(new ComboxList { ID = i, Name = parameter_list[i] });
                }
                ParameterIndexes = parameter_list.FindIndex(x => x == model.NewName);
                ParameterName = model.NewName;
                ParameterJsonTool.ReadJson(model.NewName, ref par);
                SystemArguments = new ObservableCollection<SystemParameter>(par.SystemParameters_Obse.ToList());
                PointLocationArguments = new ObservableCollection<PointLocationParameter>(par.PointLocationParameter_Obse.ToList());
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

        public void SetParameterValue(string value)
        {
            string configFile = $"{Application.StartupPath}\\SystemParameter\\Parameter.config";
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            config.AppSettings.Settings["ParameterName"].Value = value;
            config.Save();
        }

        public string GetParameterValue()
        {
            string configFile = $"{Application.StartupPath}\\SystemParameter\\Parameter.config";
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            return config.AppSettings.Settings["ParameterName"].Value;
        }

        public static bool TypeAndValueCheck(SystemParameter TypeModel)
        {
            string input_value = Convert.ToString(TypeModel.Value);
            switch (TypeModel.SelectedValue)
            {
                case 0:
                    if (input_value != "Null" || input_value != "" || input_value != null)
                        return true;
                    else
                        return false;
                case 1:
                    bool bool_value = false;
                    if (bool.TryParse(input_value, out bool_value))
                        return true;
                    else
                        return false;
                case 2:
                    int int_value = 0;
                    if (int.TryParse(input_value, out int_value))
                        return true;
                    else
                        return false;
                case 3:
                    float float_value = 0f;
                    if (float.TryParse(input_value, out float_value))
                        return true;
                    else
                        return false;
                case 4:
                    double double_value = 0d;
                    if (double.TryParse(input_value, out double_value))
                        return true;
                    else
                        return false;
                default:
                    return false;
            }

        }

        /// <summary>
        /// 新建参数
        /// </summary>
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

        /// <summary>
        /// 参数删除
        /// </summary>
        public DelegateCommand ParameterDelete { get; set; }

        /// <summary>
        /// 当前参数名称
        /// </summary>
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
                this.ValueType = system.ValueType;
                this.SelectedValue = system.SelectedValue;
            }
        }

        public SystemParameter(int iD, string name, object value)
        {
            ID = iD;
            Name = name;
            Value = value;
            ValueType = Value.GetType();
            switch (ValueType.Name)
            {
                case "String":
                    SelectedValue = 0; break;
                case "Boolean":
                    SelectedValue = 1; break;
                case "Int32":
                    SelectedValue = 2; break;
                case "Single":
                    SelectedValue = 3; break;
                case "Double":
                    SelectedValue = 4; break;
            }
        }

        public int ID { get; set; }

        public string Name { get; set; }

        private object value;
        private Type valueType;

        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public Type ValueType
        {
            get { return valueType; }
            set
            {
                this.valueType = value;
                switch (valueType.Name)
                {
                    case "String":
                        SelectedValue = 0; break;
                    case "Boolean":
                        SelectedValue = 1; break;
                    case "Int32":
                        SelectedValue = 2; break;
                    case "Single":
                        SelectedValue = 3; break;
                    case "Double":
                        SelectedValue = 4; break;
                }
            }
        }

        public int SelectedValue { get; set; }
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

        public PointLocationParameter() { }
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

    public class NewModelEvent : PubSubEvent<Add_Model> { }

    public class Add_Model
    {
        public string NewName { get; set; }
        public string InitialParameter { get; set; }
    }


}
