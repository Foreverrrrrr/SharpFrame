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
        private static Dictionary<string, FlowNode> pathNode = new Dictionary<string, FlowNode>();

        public static Dictionary<string, FlowNode> PathNode
        {
            get { return pathNode; }
            set { pathNode = value; }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        public static void AddNode(FlowNode node)
        {
            PathNode[node.Name] = node;
        }

        public static void SetNodeObject(string nodename, object value)
        {
            if (pathNode.ContainsKey(nodename))
            {
                pathNode[nodename].Parameter = value;
            }
        }

        /// <summary>
        /// 添加节点连接逻辑
        /// </summary>
        /// <param name="from">连接起始节点</param>
        /// <param name="to">连接结束节点</param>
        public static void AddEdge(string from, string to)
        {
            if (from != to)
            {
                if (PathNode.ContainsKey(from) && PathNode.ContainsKey(to))
                {
                    if (!PathNode[from].NextNodes.Contains(PathNode[to]))
                    {
                        PathNode[from].IsConnect = true;
                        PathNode[to].IsConnect = true;
                        PathNode[from].NextNodes.Add(PathNode[to]);
                        PathNode[to].PreNodes.Add(PathNode[from]);
                        PathNode[to].Level = PathNode[from].Level + 1;
                    }
                }
            }
        }

        /// <summary>
        /// 广度优先路径规划
        /// </summary>
        public static bool Route_Planning(object[] objects)
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
                        BreadthFirstExecute(item, objects);
                    }
                }
                else
                {
                    throw new Exception("检测到多个初始节点流程");
                }
            }
            else
            {
                throw new Exception("检测到有未连接的节点流程");
            }
            return connect;
        }

        private static void BreadthFirstExecute(FlowNode startNode, object[] objects)
        {
            Queue<FlowNode> queue = new Queue<FlowNode>();
            queue.Enqueue(startNode);
            while (queue.Count > 0)
            {
                FlowNode currentNode = queue.Dequeue();
                currentNode.Method?.Invoke(currentNode, objects);
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
        private static void RecursiveExecute(FlowNode flowNode, object[] objects)
        {
            if (flowNode.NextNodes.Count == 0)
            {
                flowNode.Method?.Invoke(flowNode, objects);
                return;
            }
            flowNode.Method?.Invoke(flowNode, objects);
            foreach (var nextNode in flowNode.NextNodes)
                RecursiveExecute(nextNode, objects);
        }

        /// <summary>
        /// 保存连接路径
        /// </summary>
        public static FlowGraphParameter ConnectortoFlowGraphParameter(ObservableCollection<RoutingNodeViewModel> nodes, ObservableCollection<ConnectorViewModel> connectors)
        {
            FlowGraphParameter Save_List = new FlowGraphParameter();
            foreach (var item in nodes)
            {
                Structure.Parameter.NodesStructure nodesoffset = new Structure.Parameter.NodesStructure();
                nodesoffset.ID = item.ID.ToString();
                nodesoffset.OffsetX = item.OffsetX;
                nodesoffset.OffsetY = item.OffsetY;
                if (item is ComboBoxNodeViewModel comboBoxViewModel)
                    nodesoffset.Value = comboBoxViewModel.SelectedItem;
                Save_List.NodesStructure.Add(nodesoffset);
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
        public static void RemoveEdge(string from, string to)
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
        public static void RemoveNode(string nodeName)
        {
        }
    }
}
