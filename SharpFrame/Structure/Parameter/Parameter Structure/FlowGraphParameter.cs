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
            if (parameter.NodesOffset != null)
            {
                NodesOffset = new List<NodesOffset>();
                foreach (var offset in parameter.NodesOffset)
                {
                    NodesOffset.Add(new NodesOffset(offset));
                }
            }
        }

        public List<Connector> Connectors { get; set; } = new List<Connector>();
        public List<NodesOffset> NodesOffset { get; set; } = new List<NodesOffset>();
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

    public class NodesOffset
    {
        public NodesOffset(NodesOffset nodes)
        {
            this.ID = nodes.ID;
            this.OffsetX = nodes.OffsetX;
            this.OffsetY = nodes.OffsetY;
        }
        public NodesOffset()
        {

        }
        public string ID { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }
}
