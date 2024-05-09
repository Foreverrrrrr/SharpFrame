using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpFrame.Views
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SharpFrame.Views"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SharpFrame.Views;assembly=SharpFrame.Views"
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
    ///     <MyNamespace:AxisManualOperation/>
    ///
    /// </summary>
    public class AxisManualOperation : System.Windows.Controls.Control
    {
        static AxisManualOperation()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisManualOperation), new FrameworkPropertyMetadata(typeof(AxisManualOperation)));
        }

        public static readonly DependencyProperty AxisNameProperty = DependencyProperty.Register("AxisName", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 轴名称
        /// </summary>
        public string AxisName
        {
            get { return (string)GetValue(AxisNameProperty); }
            set { SetValue(AxisNameProperty, value); }
        }

        public static readonly DependencyProperty PosProperty = DependencyProperty.Register("Pos", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 当前位置
        /// </summary>
        public string Pos
        {
            get { return (string)GetValue(PosProperty); }
            set { SetValue(PosProperty, value); }
        }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(AxisManualOperation));

        /// <summary>
        /// 当前速度
        /// </summary>
        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly DependencyProperty MoveModelProperty = DependencyProperty.Register("MoveModel", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 运动模式
        /// </summary>
        public string MoveModel
        {
            get { return (string)GetValue(MoveModelProperty); }
            set { SetValue(MoveModelProperty, value); }
        }

        public static readonly DependencyProperty StopCauseProperty = DependencyProperty.Register("StopCause", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 停止原因
        /// </summary>
        public string StopCause
        {
            get { return (string)GetValue(StopCauseProperty); }
            set { SetValue(StopCauseProperty, value); }
        }

        public static readonly DependencyProperty Servo_Run_StateProperty = DependencyProperty.Register("Servo_Run_State", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 伺服使能状态
        /// </summary>
        public string Servo_Run_State
        {
            get { return (string)GetValue(Servo_Run_StateProperty); }
            set { SetValue(Servo_Run_StateProperty, value); }
        }

        public static readonly DependencyProperty Error_StateProperty = DependencyProperty.Register("Error_State", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 伺服报警状态
        /// </summary>
        public string Error_State
        {
            get { return (string)GetValue(Error_StateProperty); }
            set { SetValue(Error_StateProperty, value); }
        }

        public static readonly DependencyProperty Operation_StateProperty = DependencyProperty.Register("Operation_State", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 伺服动态
        /// </summary>
        public string Operation_State
        {
            get { return (string)GetValue(Operation_StateProperty); }
            set { SetValue(Operation_StateProperty, value); }
        }

        public static readonly DependencyProperty EStop_StateProperty = DependencyProperty.Register("EStop_State", typeof(string), typeof(AxisManualOperation));

        /// <summary>
        /// 急停状态
        /// </summary>
        public string EStop_State
        {
            get { return (string)GetValue(EStop_StateProperty); }
            set { SetValue(EStop_StateProperty, value); }
        }

        public static readonly DependencyProperty Set_SpeedProperty = DependencyProperty.Register("Set_Speed", typeof(double), typeof(AxisManualOperation),
                new FrameworkPropertyMetadata((double)1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 手动运行速度
        /// </summary>
        public double Set_Speed
        {
            get { return (double)GetValue(Set_SpeedProperty); }
            set { SetValue(Set_SpeedProperty, value); }
        }

        public static readonly DependencyProperty Import_locationProperty = DependencyProperty.Register("Import_location", typeof(double), typeof(AxisManualOperation),
        new FrameworkPropertyMetadata((double)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 定位地址
        /// </summary>
        public double Import_location
        {
            get { return (double)GetValue(Import_locationProperty); }
            set { SetValue(Import_locationProperty, value); }
        }

        /// <summary>
        /// 正转Jog触发事件
        /// </summary>
        public event MouseButtonEventHandler Jogjust_PreviewMouseLeft;

        /// <summary>
        /// Jog停止事件
        /// </summary>
        public event MouseButtonEventHandler Jog_Stop;

        /// <summary>
        /// 反转Jog触发事件
        /// </summary>
        public event MouseButtonEventHandler Joglose_PreviewMouseLeft;

        /// <summary>
        /// 原点回归触发事件
        /// </summary>
        public event RoutedEventHandler Move_Home;

        /// <summary>
        /// 相对定位触发事件
        /// </summary>
        public event RoutedEventHandler Move_Rel;

        /// <summary>
        /// 伺服使能触发事件
        /// </summary>
        public event MouseButtonEventHandler Servo_Run;

        /// <summary>
        /// 报警触发事件
        /// </summary>
        public event RoutedEventHandler Error_Rest;

        /// <summary>
        /// 紧急停止触发事件
        /// </summary>
        public event RoutedEventHandler E_Stop;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button Jogjust_button = GetTemplateChild("Jogjust") as Button;
            if (Jogjust_button != null)
            {
                if (Jogjust_PreviewMouseLeft != null)
                    Jogjust_button.PreviewMouseLeftButtonDown += Jogjust_PreviewMouseLeft;
                if (Jog_Stop != null)
                    Jogjust_button.PreviewMouseLeftButtonUp += Jog_Stop;
            }
            Button Joglose_button = GetTemplateChild("Joglose") as Button;
            if (Joglose_button != null)
            {
                if (Jogjust_PreviewMouseLeft != null)
                    Joglose_button.PreviewMouseLeftButtonDown += Joglose_PreviewMouseLeft;
                if (Jog_Stop != null)
                    Joglose_button.PreviewMouseLeftButtonUp += Jog_Stop;
            }
            Button Homemove_button = GetTemplateChild("Homemove") as Button;
            if (Homemove_button != null)
            {
                if (Move_Home != null)
                    Homemove_button.Click += Move_Home;
            }
            Button Relmove_button = GetTemplateChild("Relmove") as Button;
            if (Relmove_button != null)
            {
                if (Move_Rel != null)
                    Relmove_button.Click += Move_Rel;
            }
            TextBlock Servo_Run_button = GetTemplateChild("Servo_Run") as TextBlock;
            if (Servo_Run_button != null)
            {
                if (Servo_Run != null)
                    Servo_Run_button.MouseLeftButtonUp += Servo_Run;
            }
            Button Rset_button = GetTemplateChild("Rset") as Button;
            if (Rset_button != null)
            {
                if (Error_Rest != null)
                    Rset_button.Click += Error_Rest;
            }
            Button Stop_button = GetTemplateChild("Stop") as Button;
            if (Stop_button != null)
            {
                if (E_Stop != null)
                    Stop_button.Click += E_Stop;
            }
        }
    }
}
