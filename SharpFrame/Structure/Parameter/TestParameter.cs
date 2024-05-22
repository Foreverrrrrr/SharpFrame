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
}
