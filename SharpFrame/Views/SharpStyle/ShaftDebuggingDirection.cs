using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace SharpFrame.Views.SharpStyle
{
    public class ShaftDebuggingDirection : Control
    {
        static ShaftDebuggingDirection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShaftDebuggingDirection), new FrameworkPropertyMetadata(typeof(ShaftDebuggingDirection)));
        }

        public static readonly DependencyProperty StyleNameProperty = DependencyProperty.Register("StyleName", typeof(string), typeof(ShaftDebuggingDirection));
        /// <summary>
        /// 控件标题
        /// </summary>
        public string StyleName
        {
            get { return (string)GetValue(StyleNameProperty); }
            set { SetValue(StyleNameProperty, value); }
        }

        public static readonly DependencyProperty X_PositiveDirectionProperty =
          DependencyProperty.Register(
              "X_PositiveDirection",
              typeof(ICommand),
              typeof(ShaftDebuggingDirection),
              new PropertyMetadata(null));

        /// <summary>
        /// X轴正方向
        /// </summary>
        public ICommand X_PositiveDirection
        {
            get { return (ICommand)GetValue(X_PositiveDirectionProperty); }
            set { SetValue(X_PositiveDirectionProperty, value); }
        }

        public static readonly DependencyProperty X_NegativeDirectionProperty =
         DependencyProperty.Register(
             "X_NegativeDirection",
             typeof(ICommand),
             typeof(ShaftDebuggingDirection),
              new PropertyMetadata(null));

        /// <summary>
        /// X轴负方向
        /// </summary>
        public ICommand X_NegativeDirection
        {
            get { return (ICommand)GetValue(X_NegativeDirectionProperty); }
            set { SetValue(X_NegativeDirectionProperty, value); }
        }

        public static readonly DependencyProperty Y_PositiveDirectionProperty =
          DependencyProperty.Register(
              "Y_PositiveDirection",
              typeof(ICommand),
              typeof(ShaftDebuggingDirection),
              new PropertyMetadata(null));

        /// <summary>
        /// Y轴正方向
        /// </summary>
        public ICommand Y_PositiveDirection
        {
            get { return (ICommand)GetValue(Y_PositiveDirectionProperty); }
            set { SetValue(Y_PositiveDirectionProperty, value); }
        }

        public static readonly DependencyProperty Y_NegativeDirectionProperty =
         DependencyProperty.Register(
             "Y_NegativeDirection",
             typeof(ICommand),
             typeof(ShaftDebuggingDirection),
             new PropertyMetadata(null));

        /// <summary>
        /// Y轴负方向
        /// </summary>
        public ICommand Y_NegativeDirection
        {
            get { return (ICommand)GetValue(Y_NegativeDirectionProperty); }
            set { SetValue(Y_NegativeDirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionStopCommandProperty =
        DependencyProperty.Register(
            "DirectionStopCommand",
            typeof(ICommand),
            typeof(ShaftDebuggingDirection),
            new PropertyMetadata(null));

        /// <summary>
        /// 停止
        /// </summary>
        public ICommand DirectionStopCommand
        {
            get { return (ICommand)GetValue(DirectionStopCommandProperty); }
            set { SetValue(DirectionStopCommandProperty, value); }
        }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(ShaftDebuggingDirection),
               new FrameworkPropertyMetadata((double)1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 手动速度
        /// </summary>
        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }
    }
}
