using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpFrame.Flow_of_Execution
{
    public class FlowNode
    {
        public string Name { get; set; }

        public Action<FlowNode, object[]> Method { get; set; }

        /// <summary>
        /// 后节点集合
        /// </summary>
        public List<FlowNode> NextNodes { get; set; }

        /// <summary>
        /// 前节点集合
        /// </summary>
        public List<FlowNode> PreNodes { get; set; }

        /// <summary>
        /// 执行状态
        /// </summary>
        public bool IsExecuted { get; set; }

        /// <summary>
        /// 连接层级
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// 是否存在路径连接
        /// </summary>
        public bool IsConnect { get; set; }

        /// <summary>
        /// 节点自定义参数
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="method">节点回调方法</param>
        public FlowNode(string name, DataTemplate template, Action<FlowNode, object[]> method) : this(name)
        {
            Method = method;
        }

        public FlowNode(string name)
        {
            Name = name;
            NextNodes = new List<FlowNode>();
            PreNodes = new List<FlowNode>();
            NextNodes = new List<FlowNode>();
            IsExecuted = false;
            IsConnect = false;
        }
    }
}
