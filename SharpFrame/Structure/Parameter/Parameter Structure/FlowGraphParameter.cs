using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Structure.Parameter
{
    public class FlowGraphParameter
    {
        public FlowGraphParameter() { }
        public FlowGraphParameter(FlowGraphParameter parameter)
        {
            // 深拷贝 Connectors 集合
            if (parameter.Connectors != null)
            {
                Connectors = new List<Connector>();
                foreach (var connector in parameter.Connectors)
                {
                    Connectors.Add(new Connector(connector));
                }
            }

            // 深拷贝 NodesOffset 集合
            if (parameter.NodesStructure != null)
            {
                NodesStructure = new List<NodesStructure>();
                foreach (var offset in parameter.NodesStructure)
                {
                    NodesStructure.Add(new NodesStructure(offset));
                }
            }
        }
        /// <summary>
        /// 连接器状态
        /// </summary>
        public List<Connector> Connectors { get; set; } = new List<Connector>();
        /// <summary>
        /// 节点结构
        /// </summary>
        public List<NodesStructure> NodesStructure { get; set; } = new List<NodesStructure>();
    }

    public class Connector
    {
        public Connector(Connector connector)
        {
            this.SourceID = connector.SourceID;
            this.TargetID = connector.TargetID;
            this.SourcePortID = connector.SourcePortID;
            this.TargetPortID = connector.TargetPortID;
        }
        public Connector()
        {

        }
        public string SourceID { get; set; }
        public string TargetID { get; set; }
        public string SourcePortID { get; set; }
        public string TargetPortID { get; set; }
    }

    public class NodesStructure
    {
        public NodesStructure(NodesStructure nodes)
        {
            this.ID = nodes.ID;
            this.OffsetX = nodes.OffsetX;
            this.OffsetY = nodes.OffsetY;
            this.Value = nodes.Value;
        }
        public NodesStructure()
        {

        }
        public string ID { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public object Value { get; set; }
    }
}
