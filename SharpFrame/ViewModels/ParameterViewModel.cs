using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SharpFrame.Flow_of_Execution;
using SharpFrame.Structure.Parameter;
using SharpFrame.Views;
using SharpFrame.Views.SharpStyle;
using SharpFrame.Views.ToolViews;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using static SharpFrame.ViewModels.SystemLogInViewModel;
using Parameter = SharpFrame.Structure.Parameter.Parameter;
using SystemParameter = SharpFrame.Structure.Parameter.SystemParameter;

namespace SharpFrame.ViewModels
{
    public class ParameterViewModel : BindableBase
    {
        private IEventAggregator eventAggregator;
        private IDialogService dialogService;
        /// <summary>
        /// 参数更新静态通知
        /// </summary>
        public static event Action<Parameter> ParameterUpdate;
        public ParameterViewModel(IEventAggregator aggregator1, IDialogService dialog)
        {
            this.eventAggregator = aggregator1;
            this.dialogService = dialog;
            eventAggregator.GetEvent<LoginPermission>().Subscribe((type) =>
            {
                if (type.type == PermissionType.Supplier || type.type == PermissionType.Engineer)
                    IsVisible = true;
            });
            eventAggregator.GetEvent<PageLoadEvent>().Subscribe((classobj) =>
            {
                List<string> system_list = classobj[0] as List<string>;
                if (system_list.Count > 0)
                {
                    for (int i = 0; i < system_list.Count; i++)
                    {
                        ParameterNameList.Add(new ComboxList { ID = i, Name = system_list[i] });
                    }
                    string paramValue = GetParameterValue();
                    Parameter systems = new Parameter();
                    if (paramValue != "Null")
                        ParameterName = paramValue;
                    else
                        ParameterName = system_list[0];
                    ParameterJsonTool.ReadJson(ParameterName, ref systems);
                    ParameterIndexes = system_list.FindIndex(x => x == ParameterName);
                    SystemArguments = new ObservableCollection<SystemParameter>(systems.SystemParameters_Obse.ToList());
                    PointLocationArguments = new ObservableCollection<PointLocationParameter>(systems.PointLocationParameter_Obse.ToList());
                    TestParameterArguments = new ObservableCollection<TestParameter>(systems.TestParameter_Obse.ToList());
                    FlowGraphArguments = new FlowGraphParameter(systems.FlowGraph_Obse);
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        var quor = FlowGraphArguments.NodesStructure.Where(x => (x.ID as string) == Nodes[i].ID.ToString()).FirstOrDefault();
                        Nodes[i].OffsetX = quor.OffsetX;
                        Nodes[i].OffsetY = quor.OffsetY;
                        if (Nodes[i] is ComboBoxNodeViewModel comboBoxViewModel)
                            comboBoxViewModel.SelectedItem = quor.Value?.ToString();
                    }
                    for (int i = 0; i < FlowGraphArguments.Connectors.Count; i++)
                    {
                        FlowChart.CreateConnectors(Connectors, Nodes,
                            FlowGraphArguments.Connectors[i].SourceID,
                            FlowGraphArguments.Connectors[i].TargetID,
                            FlowGraphArguments.Connectors[i].SourcePortID,
                            FlowGraphArguments.Connectors[i].TargetPortID);
                    }
                    TestComboBox_DropDownClosed_Evt += ((row) =>
                    {
                        var bool_ret = TypeAndValueCheck(row);
                        if (!bool_ret.Item3)
                        {
                            MessageBox.Show($"修改测试参数类型错误\r\n“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            eventAggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"修改参数类型错误，“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型" });
                        }
                    });
                    SystemComboBox_DropDownClosed_Evt += ((row) =>
                    {
                        var bool_ret = TypeAndValueCheck(row);
                        if (!bool_ret.Item3)
                        {
                            MessageBox.Show($"修改系统参数类型错误\r\n“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            eventAggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"修改参数类型错误，“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型" });
                        }
                    });
                    eventAggregator.GetEvent<ParameterUpdateEvent>().Publish(systems);
                    ParameterUpdate?.Invoke(systems);
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
                }
                system_list = ParameterJsonTool.GetJson();
            }, ThreadOption.UIThread);
            eventAggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {
                SetParameterValue(ParameterName);
            });
            ModelSwitching = new DelegateCommand(() =>
            {
                Parameter systems = new Parameter();
                var system_list = ParameterJsonTool.GetJson();
                ParameterName = system_list[ParameterIndexes];
                ParameterJsonTool.ReadJson(ParameterName, ref systems);
                SystemArguments = systems.SystemParameters_Obse;
                PointLocationArguments = systems.PointLocationParameter_Obse;
                TestParameterArguments = systems.TestParameter_Obse;
                FlowGraphArguments = new FlowGraphParameter(systems.FlowGraph_Obse);
                for (int i = 0; i < Nodes.Count; i++)
                {
                    var quor = FlowGraphArguments.NodesStructure.Where(x => (x.ID as string) == Nodes[i].ID.ToString()).FirstOrDefault();
                    Nodes[i].OffsetX = quor.OffsetX;
                    Nodes[i].OffsetY = quor.OffsetY;
                    if (Nodes[i] is ComboBoxNodeViewModel comboBoxViewModel)
                        comboBoxViewModel.SelectedItem = quor.Value?.ToString();
                }
                Connectors.Clear();
                for (int i = 0; i < FlowGraphArguments.Connectors.Count; i++)
                {
                    FlowChart.CreateConnectors(Connectors, Nodes,
                        FlowGraphArguments.Connectors[i].SourceID,
                        FlowGraphArguments.Connectors[i].TargetID,
                        FlowGraphArguments.Connectors[i].SourcePortID,
                        FlowGraphArguments.Connectors[i].TargetPortID
                        );
                }
                eventAggregator.GetEvent<ParameterUpdateEvent>().Publish(systems);
                ParameterUpdate?.BeginInvoke(systems, null, null);
                eventAggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "正常", Value = $"切换参数型号“{ParameterName}”" });

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
                        eventAggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"保存失败\r\n参数名称“{item.Name}”中值“{item.Value}”无法转换为类型“{item.ValueType.Name}”" });
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
                        eventAggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "异常", Value = $"修改参数类型错误，“{bool_ret.Item2}”无法转换为“{bool_ret.Item1}”类型" });
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
                parameter.FlowGraph_Obse = FlowGraphPath.ConnectortoFlowGraphParameter(Nodes, Connectors);
                ParameterJsonTool.WriteJson(ParameterName, parameter);
                eventAggregator.GetEvent<ParameterUpdateEvent>().Publish(parameter);
                ParameterUpdate?.Invoke(parameter);
                eventAggregator.GetEvent<MainLogOutput>().Publish(new MainLogStructure() { Time = DateTime.Now.ToString(), Level = "正常", Value = $"“{ParameterName}”参数保存完成" });
                eventAggregator.GetEvent<Notification>().Publish(new Notification() { Type = Notification.InfoType.Info, Message = $"“{ParameterName}”参数保存完成" });
            });
            ParameterDelete = new DelegateCommand(() =>
            {

            });
            NewModel = new DelegateCommand(() =>
            {
                var parameter_list = ParameterJsonTool.GetJson();
                NewParameterModelView modelView = new NewParameterModelView(eventAggregator, parameter_list);
                modelView.Show();
            });
            eventAggregator.GetEvent<NewModelEvent>().Subscribe((model) =>
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
                    System_AddView system = new System_AddView(eventAggregator, checkdata, "former");
                    system.Show();
                }
            });
            SystemArguments_Remove_Line = new DelegateCommand<SystemParameter>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
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
                    Point_AddView addView = new Point_AddView(eventAggregator, checkdata, "former");
                    addView.Show();
                }
            });
            PointLocationArguments_Remove_Line = new DelegateCommand<PointLocationParameter>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
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
                    Test_AddView addView = new Test_AddView(eventAggregator, checkdata, "former");
                    addView.Show();
                }
            });
            TestParameterArguments_Remove_Line = new DelegateCommand<TestParameter>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
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
            eventAggregator.GetEvent<SystemParameterAddEvent>().Subscribe((t) =>
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
            eventAggregator.GetEvent<PointLocationParameterAddEvent>().Subscribe((t) =>
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
            eventAggregator.GetEvent<TestParameterAddEvent>().Subscribe((t) =>
            {
                var k = TestParameterArguments.ToList().Find(x => x.ID == t.InsertionParameter.ID).ID;
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
                ParameterInitializationView initializationView = new ParameterInitializationView(eventAggregator);
                initializationView.Show();
            });
            InitializationCompleteCommand = new DelegateCommand(() =>
            {
                Constraints = (GraphConstraints.Default | GraphConstraints.Routing | GraphConstraints.Bridging) & ~GraphConstraints.ContextMenu;
                RoutingNodeViewModel start = FlowChart.CreateTextBoxNodes(Nodes, 100, 100, "启动", "#FF00FF08");
                RoutingNodeViewModel marking = FlowChart.CreateTextBoxNodes(Nodes, 100, 200, "打标", "#FF00FCFA");
                RoutingNodeViewModel front_calibration = FlowChart.CreateTextBoxNodes(Nodes, 300, 100, "前流道视觉标定", "#5BA5F0");
                RoutingNodeViewModel front_mark = FlowChart.CreateTextBoxNodes(Nodes, 300, 200, "前流道Mark点区域", "#5BA5F0");
                ComboBoxNodeViewModel front_runner_feed = FlowChart.CreateComboBoxNodes(Nodes, 300, 300, "前流道进料模式", new List<string>() { "左进", "右进" }, "左进", "#5BA5F0");
                ComboBoxNodeViewModel front_runner_discharge = FlowChart.CreateComboBoxNodes(Nodes, 300, 400, "前流道出料模式", new List<string>() { "左出", "右出" }, "左出", "#5BA5F0");
                RoutingNodeViewModel behind_calibration = FlowChart.CreateTextBoxNodes(Nodes, 500, 100, "后流道视觉标定", "#D5535D");
                RoutingNodeViewModel behind_mark = FlowChart.CreateTextBoxNodes(Nodes, 500, 200, "后流道Mark点区域", "#D5535D");
                ComboBoxNodeViewModel behind_runner_feed = FlowChart.CreateComboBoxNodes(Nodes, 500, 300, "后流道进料模式", new List<string>() { "左进", "右进" }, "左进", "#D5535D");
                ComboBoxNodeViewModel behind_runner_discharge = FlowChart.CreateComboBoxNodes(Nodes, 500, 400, "后流道出料模式", new List<string>() { "左出", "右出" }, "左出", "#D5535D");
                foreach (var item in Nodes)
                {
                    FlowChart.CreateNodePort(item, FlowChart.PortDirection.left);
                    FlowChart.CreateNodePort(item, FlowChart.PortDirection.right);
                    FlowChart.CreateNodePort(item, FlowChart.PortDirection.up);
                    FlowChart.CreateNodePort(item, FlowChart.PortDirection.down);
                }
                ItemDoubleTappedCommand = new DelegateCommand<object>((parameter) =>
                {
                    ItemDoubleTappedEventArgs routing = parameter as ItemDoubleTappedEventArgs;
                    RoutingNodeViewModel eventArgs = routing.Item as RoutingNodeViewModel;
                    if (eventArgs != null)
                    {
                        if (eventArgs.ID as string == "前流道视觉标定" || eventArgs.ID as string == "后流道视觉标定")
                        {
                            var nodename = eventArgs.ID as string;
                            IDialogParameters dialogParameters = new DialogParameters();
                            dialogParameters.Add("ID", nodename);
                            try
                            {
                                dialog.ShowDialog("VisualCalibrationView", dialogParameters, new Action<IDialogResult>((x) =>
                                {
                                    if (x.Result == ButtonResult.OK)
                                    {
                                        string customData = x.Parameters.GetValue<string>("Carmatrix");
                                        var tempCollection = new ObservableCollection<TestParameter>(TestParameterArguments);
                                        var parameter = tempCollection.Where(x => x.Name == nodename[0] + "流道相机标定参数").FirstOrDefault();
                                        if (parameter != null)
                                        {
                                            parameter.Value = customData;
                                        }
                                        TestParameterArguments = null;
                                        TestParameterArguments = tempCollection;
                                    }
                                }));
                            }
                            catch (Exception ex)
                            {
                                eventAggregator.GetEvent<Notification>().Publish(new Notification() { Type = Notification.InfoType.Info, Message = ex.Message });
                            }
                        }
                    }
                });
                ConnectorEditingCommand = new DelegateCommand<object>((obj) =>
                {
                    var args = obj as ConnectorEditingEventArgs;
                    var t = args.Item as ConnectorViewModel;
                    List<IGroupable> deleteableObjects = new List<IGroupable>
                    {
                      t
                    };
                    if (args.DragState == DragState.Completed)
                    {
                        if (args.ControlPointType == ControlPointType.TargetPoint || args.ControlPointType == ControlPointType.SourcePoint)
                        {
                            var Source = t.SourceNode as RoutingNodeViewModel;
                            var Target = t.TargetNode as RoutingNodeViewModel;
                            if (Source != null && Target != null)
                            {
                                if (Source.ID.ToString() != Target.ID.ToString())
                                {
                                    FlowGraphPath.AddEdge(Source.ID.ToString(), Target.ID.ToString());
                                    Connectors[Connectors.IndexOf(t)].Constraints = (ConnectorConstraints.Default | ConnectorConstraints.Bridging) & ~ConnectorConstraints.SourceDraggable & ~ConnectorConstraints.TargetDraggable;
                                }
                                else
                                {
                                    FlowChart.Delete(deleteableObjects);
                                }
                            }
                            else
                            {
                                if (Source != null && Target != null)
                                    FlowGraphPath.RemoveEdge(Source.ID.ToString(), Target.ID.ToString());
                                FlowChart.Delete(deleteableObjects);
                            }
                        }
                    }
                });
                ItemDeletedCommand = new DelegateCommand<object>((obj) =>
                {
                    var args = obj as ItemDeletedEventArgs;
                    if (args.Item is IConnector)
                    {
                        var t = args.Item as ConnectorViewModel;
                        var Source = t.SourceNode as RoutingNodeViewModel;
                        var Target = t.TargetNode as RoutingNodeViewModel;
                        if (Source != null && Target != null)
                            FlowGraphPath.RemoveEdge(Source.ID.ToString(), Target.ID.ToString());
                    }
                });
            });
        }
        public void Log(FlowNode obj)
        {
            Console.WriteLine(obj.Name.ToString());
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

        /// <summary>
        /// 新参数生成
        /// </summary>
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
                if (_systemarguments != null)
                {
                    foreach (var item in _systemarguments)
                    {
                        item.ComboBoxChanged = new DelegateCommand<object>((rowdata) =>
                        {
                            SystemParameter parameter = rowdata as SystemParameter;
                            SystemComboBox_DropDownClosed_Evt?.Invoke(parameter);
                        });
                    }
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
            set
            {
                _testparameterarguments = value;
                if (_testparameterarguments != null)
                {
                    foreach (var item in _testparameterarguments)
                    {
                        item.ComboBoxChanged = new DelegateCommand<object>((rowdata) =>
                        {
                            TestParameter parameter = rowdata as TestParameter;
                            TestComboBox_DropDownClosed_Evt?.Invoke(parameter);
                        });
                    }
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 流程图

        private ObservableCollection<RoutingNodeViewModel> mNodes = new ObservableCollection<RoutingNodeViewModel>();
        /// <summary>
        /// 流程图节点集合
        /// </summary>
        public ObservableCollection<RoutingNodeViewModel> Nodes
        {
            get { return mNodes; }
            set { SetProperty(ref mNodes, value); }
        }

        private ObservableCollection<ConnectorViewModel> mConnectors = new ObservableCollection<ConnectorViewModel>();
        /// <summary>
        /// 流程图连接器集合
        /// </summary>
        public ObservableCollection<ConnectorViewModel> Connectors
        {
            get { return mConnectors; }
            set { SetProperty(ref mConnectors, value); }
        }

        private GraphConstraints mConstraints;

        public GraphConstraints Constraints
        {
            get { return mConstraints; }
            set { SetProperty(ref mConstraints, value); }
        }

        private ICommand mViewPortChangedCommand;

        public ICommand ViewPortChangedCommand
        {
            get { return mViewPortChangedCommand; }
            set { SetProperty(ref mViewPortChangedCommand, value); }
        }

        private ICommand mItemDoubleTappedCommand;
        /// <summary>
        /// 流程图节点双击命令
        /// </summary>
        public ICommand ItemDoubleTappedCommand
        {
            get { return mItemDoubleTappedCommand; }
            set { SetProperty(ref mItemDoubleTappedCommand, value); }
        }

        private ICommand mConnectorEditingCommand;
        /// <summary>
        /// 流程图连接器连接动作命令
        /// </summary>
        public ICommand ConnectorEditingCommand
        {
            get { return mConnectorEditingCommand; }
            set { SetProperty(ref mConnectorEditingCommand, value); }
        }

        private ICommand mItemDeletedCommand;
        /// <summary>
        /// 流程图连接器删除命令
        /// </summary>
        public ICommand ItemDeletedCommand
        {
            get { return mItemDeletedCommand; }
            set { SetProperty(ref mItemDeletedCommand, value); }
        }

        private ICommand mInitializationcompleteCommand;
        /// <summary>
        /// 流程图初始化完成命令
        /// </summary>
        public ICommand InitializationCompleteCommand
        {
            get { return mInitializationcompleteCommand; }
            set { SetProperty(ref mInitializationcompleteCommand, value); }
        }

        private FlowGraphParameter _flowgrapharguments = new FlowGraphParameter();
        /// <summary>
        /// 流程图连接器
        /// </summary>
        public FlowGraphParameter FlowGraphArguments
        {
            get { return _flowgrapharguments; }
            set { _flowgrapharguments = value; }
        }
        #endregion

        private int _parameterindexes = 0;
        /// <summary>
        /// 参数型号选择ComboBox
        /// </summary>
        public int ParameterIndexes
        {
            get { return _parameterindexes; }
            set { SetProperty(ref _parameterindexes, value); }
        }

        private ObservableCollection<ComboxList> _parameterNameList = new ObservableCollection<ComboxList>();
        /// <summary>
        /// 参数型号选择集合
        /// </summary>
        public ObservableCollection<ComboxList> ParameterNameList
        {
            get { return _parameterNameList; }
            set { SetProperty(ref _parameterNameList, value); }
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
        /// <summary>
        /// 控件权限
        /// </summary>
        public bool IsVisible
        {
            get { return _isvisible; }
            set { SetProperty(ref _isvisible, value); }
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
