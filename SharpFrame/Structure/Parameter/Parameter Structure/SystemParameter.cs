using Newtonsoft.Json;
using Prism.Commands;
using System;

namespace SharpFrame.Structure.Parameter
{
    /// <summary>
    /// 系统参数结构
    /// </summary>
    public class SystemParameter : ParameterTypeBase
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

        public override object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public override Type ValueType
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

        public override int SelectedValue { get; set; }

        /// <summary>
        /// 参数类型变更
        /// </summary>
        [JsonIgnore]//跳过序列化、反序列化
        public DelegateCommand<object> ComboBoxChanged { get; set; }
    }
}
