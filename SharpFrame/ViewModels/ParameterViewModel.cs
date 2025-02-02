﻿using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.Structure.Parameter;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using Parameter = SharpFrame.Structure.Parameter.Parameter;
using SystemParameter = SharpFrame.Structure.Parameter.SystemParameter;

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
                if (type.type == PermissionType.Supplier || type.type == PermissionType.Engineer)
                    IsVisible = true;
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
                        TestParameterArguments = new ObservableCollection<TestParameter>(systems.TestParameter_Obse.ToList());

                    }
                    else
                    {
                        ParameterName = system_list[0];
                        ParameterJsonTool.ReadJson(ParameterName, ref systems);
                        ParameterIndexes = 0;
                        SystemArguments = new ObservableCollection<SystemParameter>(systems.SystemParameters_Obse.ToList());
                        PointLocationArguments = new ObservableCollection<PointLocationParameter>(systems.PointLocationParameter_Obse.ToList());
                        TestParameterArguments = new ObservableCollection<TestParameter>(systems.TestParameter_Obse.ToList());

                    }
                    TestComboBox_DropDownClosed_Evt += ((row) =>
                    {
                        var bool_ret = TypeAndValueCheck(row);
                        if (!bool_ret.Item3)
                        {
                            MessageBox.Show($"修改测试参数类型错误\r\n“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"修改参数类型错误，“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型" });
                        }
                    });
                    SystemComboBox_DropDownClosed_Evt += ((row) =>
                    {
                        var bool_ret = TypeAndValueCheck(row);
                        if (!bool_ret.Item3)
                        {
                            MessageBox.Show($"修改系统参数类型错误\r\n“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"修改参数类型错误，“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型" });
                        }
                    });
                    aggregator.GetEvent<ParameterUpdateEvent>().Publish(systems);
                }
                else
                {
                    //Parameter parameter = new Parameter();
                    //parameter.SystemParameters_Obse.Add(new SystemParameter(0, "0", 0.211f));
                    //parameter.SystemParameters_Obse.Add(new SystemParameter(1, "1", 131.00));
                    //parameter.SystemParameters_Obse.Add(new SystemParameter(2, "2", "dsadas"));
                    //parameter.PointLocationParameter_Obse.Add(new PointLocationParameter() { ID = 0, Name = "安全点", Enable = true });
                    //parameter.PointLocationParameter_Obse.Add(new PointLocationParameter() { ID = 1, Name = "P1", Enable = true });
                    //parameter.PointLocationParameter_Obse.Add(new PointLocationParameter() { ID = 2, Name = "P2", Enable = true });
                    //parameter.TestParameter_Obse.Add(new TestParameter() { ID = 0, Name = "0", Value = 0.211f });
                    //parameter.TestParameter_Obse.Add(new TestParameter() { ID = 1, Name = "1", Value = 0.333 });
                    //parameter.TestParameter_Obse.Add(new TestParameter() { ID = 2, Name = "2", Value = "sasda" });
                    //ParameterJsonTool.Set_NullJson(parameter);
                    //ParameterJsonTool.NewJosn(DateTime.Now.ToString("yyyy_MM_dd"));
                    //throw new FileNotFoundException($"{System.Environment.CurrentDirectory + @"\Parameter"}下参数配置文件不存在");
                }
                system_list = ParameterJsonTool.GetJson();
            });
            aggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {
                SetParameterValue(ParameterName);
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
                TestParameterArguments = systems.TestParameter_Obse;
                aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "正常", Value = $"切换参数型号“{ParameterName}”" });
            });
            ParameterSave = new DelegateCommand(() =>
            {
                Parameter parameter = new Parameter();
                parameter.SystemParameters_Obse = SystemArguments;
                foreach (var item in parameter.SystemParameters_Obse)
                {
                    var bool_ret = TypeAndValueCheck(item);
                    if (!bool_ret.Item3)
                    {
                        MessageBox.Show($"保存系统参数错误\r\n“{item.Name}”中值“{item.Value}”无法转换为类型“{item.ValueType.Name}”", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"保存失败\r\n参数名称“{item.Name}”中值“{item.Value}”无法转换为类型“{item.ValueType.Name}”" });
                        return;
                    }
                    else
                    {
                        object value = item.Value;
                        switch (item.SelectedValue)
                        {
                            case 0:
                                value = item.Value.ToString(); break;
                            case 1:
                                value = Convert.ToBoolean(item.Value); break;
                            case 2:
                                value = Convert.ToInt32(item.Value); break;
                            case 3:
                                value = Convert.ToSingle(item.Value); break;
                            case 4:
                                value = Convert.ToDouble(item.Value); break;
                        }
                        item.Value = value;
                        item.ValueType = item.Value.GetType();
                    }
                }
                parameter.PointLocationParameter_Obse = PointLocationArguments;

                parameter.TestParameter_Obse = TestParameterArguments;
                foreach (var item in parameter.TestParameter_Obse)
                {
                    var bool_ret = TypeAndValueCheck(item);
                    if (!bool_ret.Item3)
                    {
                        MessageBox.Show($"保存测试参数错误\r\n“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"修改参数类型错误，“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型" });
                        return;
                    }
                    else
                    {
                        object value = item.Value;
                        switch (item.SelectedValue)
                        {
                            case 0:
                                value = item.Value.ToString(); break;
                            case 1:
                                value = Convert.ToBoolean(item.Value); break;
                            case 2:
                                value = Convert.ToInt32(item.Value); break;
                            case 3:
                                value = Convert.ToSingle(item.Value); break;
                            case 4:
                                value = Convert.ToDouble(item.Value); break;
                        }
                        item.Value = value;
                        item.ValueType = item.Value.GetType();
                    }
                }
                ParameterJsonTool.WriteJson(ParameterName, parameter);
                aggregator.GetEvent<ParameterUpdateEvent>().Publish(parameter);
                aggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "正常", Value = $"“{ParameterName}”参数保存完成" });
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
                TestParameterArguments = new ObservableCollection<TestParameter>(par.TestParameter_Obse.ToList());
            });
            SystemArguments_Add_Line = new DelegateCommand<SystemParameter>((checkdata) =>
            {
                if (checkdata != null)
                {
                    System_AddView system = new System_AddView(aggregator, checkdata, "former");
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
                    Point_AddView addView = new Point_AddView(aggregator, checkdata, "former");
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
            TestParameterArguments_Add_Line = new DelegateCommand<TestParameter>((checkdata) =>
            {
                if (checkdata != null)
                {
                    Test_AddView addView = new Test_AddView(aggregator, checkdata, "former");
                    addView.Show();
                }
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
            }, ThreadOption.UIThread, false, (filtration) => filtration.FiltrationModel == "former");

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
            }, ThreadOption.UIThread, false, (filtration) => filtration.FiltrationModel == "former");
            aggregator.GetEvent<TestParameterAddEvent>().Subscribe((t) =>
            {
                var k = PointLocationArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
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
            }, ThreadOption.UIThread, false, (filtration) => filtration.FiltrationModel == "former");
            ParameterGgeneration = new DelegateCommand(() =>
            {
                ParameterInitializationView initializationView = new ParameterInitializationView(aggregator);
                initializationView.Show();
            });
        }

        public void SetParameterValue(string value)
        {
            string configFile = $"{Application.StartupPath}\\Structure\\Parameter\\Parameter.config";
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            config.AppSettings.Settings["ParameterName"].Value = value;
            config.Save();
        }

        public string GetParameterValue()
        {
            string configFile = $"{Application.StartupPath}\\Structure\\Parameter\\Parameter.config";
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            return config.AppSettings.Settings["ParameterName"].Value;
        }

        public static (string, string, bool) TypeAndValueCheck(ParameterTypeBase TypeModel)
        {
            string input_value = Convert.ToString(TypeModel.Value);
            switch (TypeModel.SelectedValue)
            {
                case 0:
                    if (input_value != "Null" || input_value != "" || input_value != null)
                        return ("String", input_value, true);
                    else
                        return ("String", input_value, false);
                case 1:
                    bool bool_value = false;
                    if (bool.TryParse(input_value, out bool_value))
                        return ("Bool", input_value, true);
                    else
                        return ("Bool", input_value, false);
                case 2:
                    int int_value = 0;
                    if (int.TryParse(input_value, out int_value))
                        return ("Int", input_value, true);
                    else
                        return ("Int", input_value, false);
                case 3:
                    float float_value = 0f;
                    if (float.TryParse(input_value, out float_value))
                        return ("Float", input_value, true);
                    else
                        return ("Float", input_value, false);
                case 4:
                    double double_value = 0d;
                    if (double.TryParse(input_value, out double_value))
                        return ("Double", input_value, true);
                    else
                        return ("Double", input_value, false);
                default:
                    return ("未知", input_value, false);
            }

        }


        public event Action<SystemParameter> SystemComboBox_DropDownClosed_Evt;

        public event Action<TestParameter> TestComboBox_DropDownClosed_Evt;

        public DelegateCommand ParameterGgeneration { get; set; }

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
            set
            {
                _systemarguments = value;
                foreach (var item in _systemarguments)
                {
                    item.ComboBoxChanged = new DelegateCommand<object>((rowdata) =>
                    {
                        SystemParameter parameter = rowdata as SystemParameter;
                        SystemComboBox_DropDownClosed_Evt?.Invoke(parameter);
                    });
                }
                RaisePropertyChanged();
            }
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
            set
            {
                _testparameterarguments = value;
                foreach (var item in _testparameterarguments)
                {
                    item.ComboBoxChanged = new DelegateCommand<object>((rowdata) =>
                    {
                        TestParameter parameter = rowdata as TestParameter;
                        TestComboBox_DropDownClosed_Evt?.Invoke(parameter);
                    });
                }
                RaisePropertyChanged();
            }
        }
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

        private bool _isvisible;

        public bool IsVisible
        {
            get { return _isvisible; }
            set { _isvisible = value; RaisePropertyChanged(); }
        }

    }

    public struct ComboxList
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    /// <summary>
    /// 系统参数添加行通知
    /// </summary>
    public class SystemParameterAddEvent : PubSubEvent<Add_SystemIns> { }

    public class Add_SystemIns
    {
        public string FiltrationModel { get; set; }

        public Structure.Parameter.SystemParameter NewParameter { get; set; }

        public Structure.Parameter.SystemParameter InsertionParameter { get; set; }
    }

    /// <summary>
    /// 点位参数添加行通知
    /// </summary>
    public class PointLocationParameterAddEvent : PubSubEvent<Add_PointLocationIns> { }

    public class Add_PointLocationIns
    {
        public string FiltrationModel { get; set; }

        public PointLocationParameter NewParameter { get; set; }

        public PointLocationParameter InsertionParameter { get; set; }
    }

    /// <summary>
    /// 测试参数添加行通知
    /// </summary>
    public class TestParameterAddEvent : PubSubEvent<Add_Test> { }

    public class Add_Test
    {
        public string FiltrationModel { get; set; }

        public TestParameter NewParameter { get; set; }

        public TestParameter InsertionParameter { get; set; }
    }

    public class NewModelEvent : PubSubEvent<Add_Model> { }

    public class Add_Model
    {
        public string NewName { get; set; }
        public string InitialParameter { get; set; }
    }

    /// <summary>
    /// 参数更新通知
    /// </summary>
    public class ParameterUpdateEvent : PubSubEvent<Parameter> { }
}
