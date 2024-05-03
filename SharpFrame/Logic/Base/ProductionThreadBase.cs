using System;
using System.Threading;

namespace SharpFrame.Logic.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ProductionThreadBase : Attribute
    {
        private Thread new_Thread;

        private bool is_Running;

        public System.Threading.Thread New_Thread { get; set; }

        public string Thread_Name { get; set; }

        public string Target { get; set; }

        public bool Is_Running { get; set; }
    }
}
