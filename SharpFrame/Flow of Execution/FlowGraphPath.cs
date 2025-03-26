using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpFrame.Structure.Parameter;
using Syncfusion.UI.Xaml.Diagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Flow_of_Execution
{
    public class FlowGraphPath
    {
        private Dictionary<string, FlowNode> PathNode = new Dictionary<string, FlowNode>();

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(FlowNode node)
        {
            PathNode[node.Name] = node;
        }

        /// <summary>
        /// 添加节点连接逻辑
        /// </summary>
        /// <param name="from">连接起始节点</param>
        /// <param name="to">连接结束节点</param>
        public void AddEdge(string from, string to)
        {
            if (from != to)
            {
                if (PathNode.ContainsKey(from) && PathNode.ContainsKey(to))
                {
                    PathNode[from].IsConnect = true;
                    PathNode[to].IsConnect = true;
                    PathNode[from].NextNodes.Add(PathNode[to]);
                    PathNode[to].PreNodes.Add(PathNode[from]);
                    PathNode[to].Level = PathNode[from].Level + 1;
                }
            }
        }

        /// <summary>
        /// 广度优先路径规划
        /// </summary>
        public void Route_Planning()
        {
            var connect = PathNode.Values.All(node => node.IsConnect);
            if (connect)
            {
                foreach (var kvp in PathNode)
                    kvp.Value.IsExecuted = false;
                // 找出所有入度为0的节点
                var startNodes = PathNode.Values.Where(n => n.PreNodes.Count == 0 && n.IsConnect == true).ToList();
                if (startNodes.Count == 1)
                {
                    foreach (FlowNode item in startNodes)
                    {
                        BreadthFirstExecute(item);
                    }
                }
                else
                {
                    Console.WriteLine("检测到多个起始流程");
                }
            }
            else
            {
                Console.WriteLine("检测到有未连接的流程");
            }
        }

        public void BreadthFirstExecute(FlowNode startNode)
        {
            Queue<FlowNode> queue = new Queue<FlowNode>();
            queue.Enqueue(startNode);
            while (queue.Count > 0)
            {
                FlowNode currentNode = queue.Dequeue();
                currentNode.Method?.Invoke(currentNode);
                foreach (var nextNode in currentNode.NextNodes)
                {
                    if (!nextNode.IsExecuted)
                    {
                        nextNode.IsExecuted = true;
                        queue.Enqueue(nextNode);
                    }
                }
            }
        }

        /// <summary>
        /// 递归执行当前节点及其后续节点的方法
        /// </summary>
        public void RecursiveExecute(FlowNode flowNode)
        {
            if (flowNode.NextNodes.Count == 0)
            {
                flowNode.Method?.Invoke(flowNode);
                return;
            }
            flowNode.Method?.Invoke(flowNode);
            foreach (var nextNode in flowNode.NextNodes)
                RecursiveExecute(nextNode);
        }

        /// <summary>
        /// 保存连接路径
        /// </summary>
        public static FlowGraphParameter ConnectortoFlowGraphParameter(ObservableCollection<RoutingNodeViewModel> nodes, ObservableCollection<ConnectorViewModel> connectors)
        {
            FlowGraphParameter Save_List = new FlowGraphParameter();
            foreach (var item in nodes)
            {
                Structure.Parameter.NodesOffset nodesoffset = new Structure.Parameter.NodesOffset();
                nodesoffset.ID = item.ID.ToString();
                nodesoffset.OffsetX = item.OffsetX;
                nodesoffset.OffsetY = item.OffsetY;
                Save_List.NodesOffset.Add(nodesoffset);
            }
            foreach (var item in connectors)
            {
                Structure.Parameter.Connector connector = new Structure.Parameter.Connector();
                var snode = item.SourceNode as RoutingNodeViewModel;
                var tnode = item.TargetNode as RoutingNodeViewModel;
                var sport = item.SourcePort as NodePortViewModel;
                var tport = item.TargetPort as NodePortViewModel;
                connector.SourcePortID = sport.ID.ToString();
                connector.TargetPortID = tport.ID.ToString();
                connector.SourceID = snode.ID.ToString();
                connector.TargetID = tnode.ID.ToString();
                Save_List.Connectors.Add(connector);
            }
            return Save_List;
        }

        public static void Set_NullJson<T>(T t) where T : class
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string destinationFile = path + @"\Base.json";
            string serializedResult = JToken.Parse(JsonConvert.SerializeObject(t)).ToString();
            File.WriteAllText(destinationFile, serializedResult, Encoding.UTF8);
        }

        /// <summary>
        /// 读取指定Json格式参数
        /// </summary>
        /// <typeparam name="T">Json反序列化类型</typeparam>
        /// <param name="table">参数名称</param>
        /// <param name="t">反序列化传递变量</param>
        /// <returns>读取结果</returns>
        public static bool ReadJson<T>(string table, ref T t) where T : class
        {
            string path = "";
            path = System.Environment.CurrentDirectory + @"\Parameter";
            DirectoryInfo root = new DirectoryInfo(path + "\\");
            FileInfo[] files = root.GetFiles();
            if (Array.Exists(files, x => x.Name == table + ".json"))
            {
                string destinationFile = path + "\\" + table + ".json";
                string jsonStr = File.ReadAllText(destinationFile);
                T deserializeResult = JsonConvert.DeserializeObject<T>(jsonStr);
                t = deserializeResult;
                return true;
            }
            else
                return false;
        }

        private void ResetNodes()
        {

        }

        /// <summary>
        /// 移除连接器
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RemoveEdge(string from, string to)
        {
            if (from != to)
            {
                if (PathNode.ContainsKey(from) && PathNode.ContainsKey(to))
                {

                    PathNode[from].IsConnect = true;
                    PathNode[to].IsConnect = true;
                    PathNode[from].NextNodes.Remove(PathNode[to]);
                    PathNode[to].PreNodes.Remove(PathNode[from]);
                    if (PathNode[from].PreNodes.Count == 0)
                        PathNode[from].IsConnect = false;
                    if (PathNode[to].PreNodes.Count == 0)
                    {
                        PathNode[to].IsConnect = false;
                        PathNode[to].Level = 0;
                    }
                    else
                    {
                        int maxLevel = PathNode[to].PreNodes.Max(node => node.Level);
                        PathNode[to].Level = maxLevel++;
                    }
                }
            }
        }

        /// <summary>
        /// 节点移除
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        public void RemoveNode(string nodeName)
        {
        }
    }
}
