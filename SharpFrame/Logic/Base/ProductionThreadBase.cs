using System;
using System.Threading;

namespace SharpFrame.Logic.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ProductionThreadBase : Attribute
    {
        /// <summary>
        /// 线程对象
        /// </summary>
        public Thread New_Thread { get; set; }

        /// <summary>
        /// 线程名称
        /// </summary>
        public string Thread_Name { get; set; }

        /// <summary>
        /// 流程类名称
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 线程状态
        /// </summary>
        public bool Is_Running { get; set; }
    }
}
