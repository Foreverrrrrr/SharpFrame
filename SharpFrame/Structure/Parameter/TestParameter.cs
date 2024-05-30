using System;

namespace SharpFrame.Structure.Parameter
{
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
                this.ValueType = system.ValueType;
                this.SelectedValue = system.SelectedValue;
            }
        }
        public TestParameter(int iD, string name, object value)
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
}
