using ImTools;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SharpFrame.ViewModels.ToolViewModels;
using SharpFrame.Views.ToolViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                var system_list = SystemParameter.ParameterJsonTool.GetJson();
                if (system_list.Count > 0)
                {
                    for (int i = 0; i < system_list.Count; i++)
                    {
                        ParameterNameList.Add(new ComboxList { ID = i, Name = system_list[i] });
                    }
                    ParameterIndexes = ParameterNameList.ToList().Find(x => x.Name == "ModelName").ID;
                    ObservableCollection<SystemStructure> systems = new ObservableCollection<SystemStructure>();
                    SystemParameter.ParameterJsonTool.ReadJson(system_list[0], ref systems);
                    SystemArguments = systems;
                }
                else
                {
                    SystemArguments.Add(new SystemStructure() { ID = 0, Name = "0", Value = 0.211f });
                    SystemArguments.Add(new SystemStructure() { ID = 1, Name = "1", Value = 0.333 });
                    SystemArguments.Add(new SystemStructure() { ID = 2, Name = "2", Value = "sasda" });
                    SystemParameter.ParameterJsonTool.NewJosn("2024_4_24");
                }
                system_list = SystemParameter.ParameterJsonTool.GetJson();
            });
            aggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {

            });
            SystemArguments_Add_Line = new DelegateCommand<SystemStructure>((checkdata) =>
            {
                System_AddView system = new System_AddView(aggregator, checkdata);
                system.Show();
            });
            SystemArguments_Remove_Line = new DelegateCommand<SystemStructure>((checkdata) =>
            {
                if (MessageBox.Show($"是否移除名称：{checkdata.Name}的项？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    SystemArguments.Remove(checkdata);
                    ObservableCollection<SystemStructure> systemStructures = new ObservableCollection<SystemStructure>();
                    int index = 0;
                    foreach (SystemStructure item in SystemArguments.Select(x => new SystemStructure(x)))
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
                ObservableCollection<SystemStructure> systemStructures = new ObservableCollection<SystemStructure>();
                int index = 0;
                foreach (SystemStructure item in SystemArguments.Select(x => new SystemStructure(x)))
                {
                    item.ID = index;
                    systemStructures.Add(item);
                    index++;
                }
                SystemArguments = null;
                SystemArguments = systemStructures;
            }, ThreadOption.UIThread);
        }

        /// <summary>
        /// 系统参数添加行请求
        /// </summary>
        public DelegateCommand<SystemStructure> SystemArguments_Add_Line { get; set; }

        /// <summary>
        /// 系统参数移除行请求
        /// </summary>
        public DelegateCommand<SystemStructure> SystemArguments_Remove_Line { get; set; }

        private ObservableCollection<SystemStructure> _systemarguments = new ObservableCollection<SystemStructure>();
        /// <summary>
        /// 系统参数表
        /// </summary>
        public ObservableCollection<SystemStructure> SystemArguments
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
    }

    public struct ComboxList
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public class SystemStructure
    {
        public SystemStructure()
        {

        }
        public SystemStructure(SystemStructure system)
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

    public class PointLocation
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool Enable { get; set; }
    }

    /// <summary>
    /// 系统参数添加行通知
    /// </summary>
    public class SystemParameterAddEvent : PubSubEvent<Add_Ins> { }

    public class Add_Ins
    {
        public SystemStructure NewParameter { get; set; }

        public SystemStructure InsertionParameter { get; set; }
    }
}
