using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using SharpFrame.Flow_of_Execution;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Serializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SharpFrame.Views
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CustomFlowchart"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CustomFlowchart;assembly=CustomFlowchart"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:FlowChart/>
    ///
    /// </summary>
    public class FlowChart : Control
    {
        /// <summary>
        /// 端口位置
        /// </summary>
        public enum PortDirection
        {
            /// <summary>
            /// 上
            /// </summary>
            up,
            /// <summary>
            /// 下
            /// </summary>
            down,
            /// <summary>
            /// 左
            /// </summary>
            left,
            /// <summary>
            /// 右
            /// </summary>
            right
        }

        public static readonly DependencyProperty ConnectorsProperty = DependencyProperty.Register(
       "Connectors",
       typeof(object),
       typeof(FlowChart),
       new PropertyMetadata(ConnectorConstraints.Default));

        public static readonly DependencyProperty ConstraintsProperty = DependencyProperty.Register(
            "Constraints",
            typeof(GraphConstraints),
            typeof(FlowChart));

        public static readonly DependencyProperty FlowChartNodesProperty =
            DependencyProperty.Register(
                "FlowChartNodes",
                typeof(ObservableCollection<RoutingNodeViewModel>),
                typeof(FlowChart));

        public static readonly DependencyProperty ItemDoubleTappedCommandProperty =
            DependencyProperty.Register(
                "ItemDoubleTappedCommand",
                typeof(ICommand),
                typeof(FlowChart));

        public static readonly DependencyProperty ViewPortChangedCommandProperty =
            DependencyProperty.Register(
                "ViewPortChangedCommand",
                typeof(ICommand),
                typeof(FlowChart));

        public static readonly DependencyProperty ConnectorEditingCommandProperty =
            DependencyProperty.Register(
                "ConnectorEditingCommand",
                typeof(ICommand),
                typeof(FlowChart));

        public static readonly DependencyProperty ItemDeletedCommandProperty =
          DependencyProperty.Register(
              "ItemDeletedCommand",
              typeof(ICommand),
              typeof(FlowChart));

        public static readonly DependencyProperty InitializationCompleteCommandProperty =
        DependencyProperty.Register(
            "InitializationCompleteCommand",
            typeof(ICommand),
            typeof(FlowChart));

        private ResourceDictionary _resourceDictionary;

        private Rect currentViewPort = Rect.Empty;

        private object minfo;

        static FlowChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlowChart),
                new FrameworkPropertyMetadata(typeof(FlowChart))
            );
        }

        public static Control ControlElement { get; set; }

        public static SfDiagram SfDiagram { get; set; }

        /// <summary>
        /// 连接器集合
        /// </summary>
        public object Connectors
        {
            get { return (object)GetValue(ConnectorsProperty); }
            set { SetValue(ConnectorsProperty, value); }
        }

        public GraphConstraints Constraints
        {
            get { return (GraphConstraints)GetValue(ConstraintsProperty); }
            set { SetValue(ConstraintsProperty, value); }
        }

        /// <summary>
        /// 流程图Nodes属性集合
        /// </summary>
        public ObservableCollection<RoutingNodeViewModel> FlowChartNodes
        {
            get { return (ObservableCollection<RoutingNodeViewModel>)GetValue(FlowChartNodesProperty); }
            set { SetValue(FlowChartNodesProperty, value); }
        }

        public object Info
        {
            get { return minfo; }
            set { minfo = value; }
        }

        /// <summary>
        /// 节点双击命令
        /// </summary>
        public ICommand ItemDoubleTappedCommand
        {
            get { return (ICommand)GetValue(ItemDoubleTappedCommandProperty); }
            set { SetValue(ItemDoubleTappedCommandProperty, value); }
        }

        public ResourceDictionary ResourceDictionary
        {
            get { return _resourceDictionary; }
            set { _resourceDictionary = value; }
        }

        public ICommand ViewPortChangedCommand
        {
            get { return (ICommand)GetValue(ViewPortChangedCommandProperty); }
            set { SetValue(ViewPortChangedCommandProperty, value); }
        }

        /// <summary>
        /// 连接器连接触发命令
        /// </summary>
        public ICommand ConnectorEditingCommand
        {
            get { return (ICommand)GetValue(ConnectorEditingCommandProperty); }
            set { SetValue(ConnectorEditingCommandProperty, value); }
        }

        /// <summary>
        /// 删除命令
        /// </summary>
        public ICommand ItemDeletedCommand
        {
            get { return (ICommand)GetValue(ItemDeletedCommandProperty); }
            set { SetValue(ItemDeletedCommandProperty, value); }
        }

        /// <summary>
        /// 流程图初始化完成命令
        /// </summary>
        public ICommand InitializationCompleteCommand
        {
            get { return (ICommand)GetValue(InitializationCompleteCommandProperty); }
            set { SetValue(InitializationCompleteCommandProperty, value); }
        }

        public DataTemplate GetConnectorAnnotationFlowChartTemplate(string DataTemplate)
        {
            return _resourceDictionary[DataTemplate] as DataTemplate;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ControlElement = this;
            ViewPortChangedCommand = new DelegateCommand<object>((parameter) =>
            {
                var args = parameter as ChangeEventArgs<object, ScrollChanged>;
                if (Info != null && (args.Item as SfDiagram).IsLoaded == true && args.NewValue.ContentBounds != currentViewPort)
                {
                    (Info as IGraphInfo).BringIntoCenter(args.NewValue.ContentBounds);
                }
                currentViewPort = args.NewValue.ContentBounds;
            }
            );
            SfDiagram = GetTemplateChild("diagram") as SfDiagram;
            var t2 = SfDiagram.SelectedItems as SelectorViewModel;
            t2.SelectorConstraints = SelectorConstraints.Tooltip;
            SfDiagram.SnapSettings = new SnapSettings()
            {
                SnapConstraints = SnapConstraints.ShowLines,
            };
            ICommand command = InitializationCompleteCommand;
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }

        public static Brush ConvertHexToBrush(string hexColor)
        {
            BrushConverter converter = new BrushConverter();
            return (Brush)converter.ConvertFromString(hexColor);
        }

        /// <summary>
        /// 节点连接器声明
        /// </summary>
        /// <param name="node1">节点</param>
        /// <param name="portDirection">连接器类型</param>
        /// <exception cref="Exception"></exception>
        public static void CreateNodePort(NodeViewModel node1, PortDirection portDirection)
        {
            RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(0, 0, 5, 5));
            NodePortViewModel nodePort = new NodePortViewModel()
            {
                HitPadding = 20,
                Shape = rectangleGeometry,
                ShapeStyle = new Style()
                {
                    Setters =
                    {
                // 设置填充颜色
                new Setter(System.Windows.Shapes.Path.FillProperty, Brushes.Black),
                // 设置边框颜色
                new Setter(System.Windows.Shapes.Path.StrokeProperty, Brushes.Black),
                // 设置边框粗细
                new Setter(System.Windows.Shapes.Path.StrokeThicknessProperty, 1.5),
                    }
                },
                Constraints = PortConstraints.Connectable & ~PortConstraints.InheritConnectable
            };
            nodePort.ID = node1.ID + portDirection.ToString();
            switch (portDirection)
            {
                case PortDirection.up:
                    nodePort.NodeOffsetX = 0.5;
                    nodePort.NodeOffsetY = 0;
                    break;
                case PortDirection.down:
                    nodePort.NodeOffsetX = 0.5;
                    nodePort.NodeOffsetY = 1;
                    break;
                case PortDirection.left:
                    nodePort.NodeOffsetX = 0;
                    nodePort.NodeOffsetY = 0.5;
                    break;
                case PortDirection.right:
                    nodePort.NodeOffsetX = 1;
                    nodePort.NodeOffsetY = 0.5;
                    break;
            }
            var t = (node1.Ports as PortCollection);
            foreach (var item in t)
            {
                if (item.ID == nodePort.ID)
                {
                    throw new Exception($"节点:{node1.ID}存在:{item.ID}端口，请勿重复添加端口");
                }
            }
            t.Add(nodePort);
        }

        public static RoutingNodeViewModel CreateComboBoxNodes(ref FlowGraphPath flowgraph, ObservableCollection<RoutingNodeViewModel> nodeViewModels, Action<FlowNode> method, string id, double offsetx, double offsety, string text, List<string> strings, string fillColor)
        {
            var borderFactory = CreateComboBoxDataTemplate(strings, 100, 22, 1, fillColor);
            RoutingNodeViewModel node = new RoutingNodeViewModel()
            {
                ID = text,
                UnitHeight = 50,
                UnitWidth = 130,
                OffsetX = offsetx,
                OffsetY = offsety,
                ContentTemplate = borderFactory,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColor)),
                Constraints = NodeConstraints.None | ~NodeConstraints.Connectable & ~NodeConstraints.Delete,
                Annotations = new ObservableCollection<IAnnotation>()
                {
                    new TextAnnotationViewModel()
                    {
                        FontWeight = FontWeights.Bold,
                        Text = text,
                        FontSize = 15,
                        WrapText = TextWrapping.NoWrap,
                        Offset = new Point(0.5, 1.2),
                        Foreground = new SolidColorBrush(Colors.Black),
                        Constraints = AnnotationConstraints.None,
                    },
                },
            };
            nodeViewModels.Add(node);
            flowgraph.AddNode(new FlowNode(text, method));
            return node;
        }

        public static RoutingNodeViewModel CreateNodes(string datatemplateKey, string id, double offsetx, double offsety, string text, string fillColor)
        {
            var nodeTemplate = ControlElement.TryFindResource(datatemplateKey) as DataTemplate;
            if (nodeTemplate != null)
            {
                RoutingNodeViewModel node = new RoutingNodeViewModel()
                {
                    ID = text,
                    UnitHeight = 50,
                    UnitWidth = 130,
                    OffsetX = offsetx,
                    OffsetY = offsety,
                    ContentTemplate = nodeTemplate,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColor)),
                    Constraints = NodeConstraints.None | ~NodeConstraints.Connectable & ~NodeConstraints.Delete,
                    Annotations = new ObservableCollection<IAnnotation>()
                    {
                        new TextAnnotationViewModel()
                        {
                            FontWeight = FontWeights.Bold,
                            Text = text,
                            FontSize = 15,
                            WrapText = TextWrapping.NoWrap,
                            Offset = new Point(0.5, 1.2),
                            Foreground = new SolidColorBrush(Colors.Black),
                            Constraints = AnnotationConstraints.None,
                        },
                    },
                };
                return node;
            }
            else
            {
                return null;
            }
        }

        public static RoutingNodeViewModel CreateTextBoxNodes(ref FlowGraphPath flowgraph, ObservableCollection<RoutingNodeViewModel> nodeViewModels, Action<FlowNode> method, string id, double offsetx, double offsety, string text, string fillColor)
        {
            var borderFactory = CreateTextBlockDataTemplate(text, 100, 20, fillColor);
            RoutingNodeViewModel node = new RoutingNodeViewModel()
            {
                ID = text,
                UnitHeight = 50,
                UnitWidth = 130,
                OffsetX = offsetx,
                OffsetY = offsety,
                ContentTemplate = borderFactory,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColor)),
                Constraints = NodeConstraints.None | ~NodeConstraints.Connectable & ~NodeConstraints.Delete,
                Annotations = new ObservableCollection<IAnnotation>()
                {
                    new TextAnnotationViewModel()
                    {
                        FontWeight = FontWeights.Bold,
                        Text = text,
                        WrapText = TextWrapping.NoWrap,
                        FontSize = 15,
                        Offset = new Point(0.5, 1.2),
                        Foreground = new SolidColorBrush(Colors.Black),
                        Constraints = AnnotationConstraints.None,
                    },
                },
            };
            nodeViewModels.Add(node);
            flowgraph.AddNode(new FlowNode(text, method));
            return node;
        }

        public static ConnectorViewModel CreateConnectors(ObservableCollection<ConnectorViewModel> connectors, NodeViewModel sourceNode, PortDirection sourcePortID, NodeViewModel targetNode, PortDirection targetPortID)
        {
            ConnectorViewModel connector = new ConnectorViewModel()
            {
                SourceNode = sourceNode,
                TargetNode = targetNode,
                SourcePortID = sourceNode.ID + sourcePortID.ToString(),
                TargetPortID = targetNode.ID + targetPortID.ToString(),
                CornerRadius = 0.5,
                Constraints = (ConnectorConstraints.Default | ConnectorConstraints.Bridging) & ~ConnectorConstraints.SourceDraggable & ~ConnectorConstraints.TargetDraggable,
            };
            connector.TargetDecoratorStyle = new System.Windows.Style(typeof(System.Windows.Shapes.Path))
            {
                Setters =
                {
                    new Setter(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M0,0 L10,5 L0,10 z")), //自定义箭头形状
                    new Setter(System.Windows.Shapes.Path.FillProperty, Brushes.Black),
                    new Setter(System.Windows.Shapes.Path.StrokeProperty, Brushes.Black),
                    new Setter(System.Windows.Shapes.Path.StrokeThicknessProperty, 1.0)
                }
            };
            connectors.Add(connector);
            return connector;
        }

        /// <summary>
        /// 生成连接路径
        /// </summary>
        /// <param name="connectors">连接器集合</param>
        /// <param name="models">节点集合</param>
        /// <param name="sourceNode">起始节点</param>
        /// <param name="targetNode">结束节点</param>
        /// <param name="sourcePortID">起始连接端口名称</param>
        /// <param name="targetPortID">结束连接端口名称</param>
        /// <returns></returns>
        public static ConnectorViewModel CreateConnectors(ObservableCollection<ConnectorViewModel> connectors, ObservableCollection<RoutingNodeViewModel> models, string sourceNode, string targetNode, string sourcePortID, string targetPortID)
        {
            RoutingNodeViewModel source = models.FirstOrDefault(x => (x.ID as string) == sourceNode);
            RoutingNodeViewModel target = models.FirstOrDefault(x => (x.ID as string) == targetNode);
            ConnectorViewModel connector = new ConnectorViewModel()
            {
                SourceNode = source,
                TargetNode = target,
                SourcePortID = sourcePortID,
                TargetPortID = targetPortID,
                CornerRadius = 0.5,
                Constraints = (ConnectorConstraints.Default | ConnectorConstraints.Bridging) & ~ConnectorConstraints.SourceDraggable & ~ConnectorConstraints.TargetDraggable,
            };
            connector.TargetDecoratorStyle = new System.Windows.Style(typeof(System.Windows.Shapes.Path))
            {
                Setters =
                {
                    new Setter(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M0,0 L10,5 L0,10 z")), //自定义箭头形状
                    new Setter(System.Windows.Shapes.Path.FillProperty, Brushes.Black),
                    new Setter(System.Windows.Shapes.Path.StrokeProperty, Brushes.Black),
                    new Setter(System.Windows.Shapes.Path.StrokeThicknessProperty, 1.0)
                }
            };
            connectors.Add(connector);
            return connector;
        }


        /// <summary>
        /// 设置ComboBox类型节点
        /// </summary>
        /// <param name="items">ComboBox集合</param>
        /// <param name="width">ComboBox控件宽度</param>
        /// <param name="height">ComboBox控件高度</param>
        /// <param name="defaultSelectedIndex">ComboBox默认选项</param>
        /// <param name="hexColor">节点背景色<param>
        /// <returns></returns>
        private static DataTemplate CreateComboBoxDataTemplate(List<string> items, double width, double height, int defaultSelectedIndex, string hexColor)
        {
            DataTemplate nodeTemplate = new DataTemplate();
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, ConvertHexToBrush(hexColor));
            FrameworkElementFactory comboBoxFactory = new FrameworkElementFactory(typeof(ComboBox));
            comboBoxFactory.SetValue(ComboBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            comboBoxFactory.SetValue(ComboBox.WidthProperty, width);
            comboBoxFactory.SetValue(ComboBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            comboBoxFactory.SetValue(ComboBox.HeightProperty, height);
            comboBoxFactory.SetValue(ComboBox.BackgroundProperty, ConvertHexToBrush("#FFECECEC"));
            comboBoxFactory.SetValue(ComboBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            comboBoxFactory.SetValue(ComboBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            if (defaultSelectedIndex >= 0 && defaultSelectedIndex < items.Count)
            {
                comboBoxFactory.SetValue(ComboBox.SelectedIndexProperty, defaultSelectedIndex);
            }
            foreach (var item in items)
            {
                FrameworkElementFactory itemFactory = new FrameworkElementFactory(typeof(ComboBoxItem));
                itemFactory.SetValue(ComboBoxItem.ContentProperty, item);
                comboBoxFactory.AppendChild(itemFactory);
            }
            borderFactory.AppendChild(comboBoxFactory);
            nodeTemplate.VisualTree = borderFactory;
            return nodeTemplate;
        }

        /// <summary>
        /// 设置TextBlock类型节点
        /// </summary>
        /// <param name="items">TextBlock Text属性</param>
        /// <param name="width">TextBlock宽度</param>
        /// <param name="height">TextBlock高度</param>
        /// <param name="hexColor">节点背景色</param>
        /// <returns></returns>
        private static DataTemplate CreateTextBlockDataTemplate(string items, double width, double height, string hexColor)
        {
            DataTemplate nodeTemplate = new DataTemplate();
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, ConvertHexToBrush(hexColor));
            FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetValue(TextBlock.WidthProperty, width);
            textBlockFactory.SetValue(TextBlock.HeightProperty, height);
            textBlockFactory.SetValue(TextBlock.TextProperty, items);
            borderFactory.AppendChild(textBlockFactory);
            nodeTemplate.VisualTree = borderFactory;
            return nodeTemplate;
        }

        /// <summary>
        /// 移除流程图内容
        /// </summary>
        /// <param name="deleteableObjects"></param>
        public static void Delete(List<IGroupable> deleteableObjects)
        {
            var parameter = new DeleteParameter() { Items = deleteableObjects };
            (SfDiagram.Info as IGraphInfo).Commands.Delete.Execute(parameter);
        }
    }
}