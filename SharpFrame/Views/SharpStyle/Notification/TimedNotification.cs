using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SharpFrame.Views.SharpStyle
{
    public abstract class TimedNotification : Control
    {
        public static readonly DependencyProperty IDProperty =
           DependencyProperty.Register(
               "ID",
               typeof(int),
               typeof(TimedNotification),
               new PropertyMetadata(int.MaxValue)
           );

        public int ID
        {
            get => (int)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(TimedNotification),
                new PropertyMetadata(string.Empty));

        /// <summary>通知消息内容</summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageTimeProperty =
            DependencyProperty.Register(
                "MessageTime",
                typeof(string),
                typeof(TimedNotification),
                new PropertyMetadata(DateTime.Now.ToString("HH:mm:ss")));

        /// <summary>消息发生时间（默认当前时间）</summary>
        public string MessageTime
        {
            get => (string)GetValue(MessageTimeProperty);
            set => SetValue(MessageTimeProperty, value);
        }

        public static readonly DependencyProperty CloseCommandProperty =
           DependencyProperty.Register(
               "CloseCommand",
               typeof(ICommand),
               typeof(TimedNotification));

        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }
    }
}
