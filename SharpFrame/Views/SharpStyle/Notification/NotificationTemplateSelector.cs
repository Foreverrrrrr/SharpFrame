using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using static SharpFrame.Common.Geometry;
using Prism.Events;
using Prism.Commands;

namespace SharpFrame.Views.SharpStyle
{
    public class NotificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InfoTemplate { get; set; }
        public DataTemplate WarningTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }
        public DataTemplate FatalTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is NotificationInfoModel)
            {
                return InfoTemplate;
            }
            else if (item is NotificationWarningModel)
            {
                return WarningTemplate;
            }
            else if (item is NotificationErrorModel)
            {
                return ErrorTemplate;
            }
            else if (item is NotificationFatalModel)
            {
                return FatalTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }

    public class NotificationInfoModel : NotificationModel
    {
        public override int ID { get; set; }
        public override string Message { get; set; }
        public override string MessageTime { get; set; }
        public override DelegateCommand<object> Delete { get; set; }
        protected override bool ShouldStartTimer()
        {
            return true;
        }
    }

    public class NotificationWarningModel : NotificationModel
    {
        public override int ID { get; set; }
        public override string Message { get; set; }
        public override string MessageTime { get; set; }
        public override DelegateCommand<object> Delete { get; set; }
        protected override bool ShouldStartTimer()
        {
            return true;
        }
    }

    public class NotificationErrorModel : NotificationModel
    {
        public override int ID { get; set; }
        public override string Message { get; set; }
        public override string MessageTime { get; set; }
        public override DelegateCommand<object> Delete { get; set; }
        protected override bool ShouldStartTimer()
        {
            return false;
        }
    }

    public class NotificationFatalModel : NotificationModel
    {
        public override int ID { get; set; }
        public override string Message { get; set; }
        public override string MessageTime { get; set; }
        public override DelegateCommand<object> Delete { get; set; }

        protected override bool ShouldStartTimer()
        {
            return false;
        }
    }

    public abstract class NotificationModel
    {
        protected readonly DispatcherTimer AutoRemoveTimer = new DispatcherTimer();
        public event RoutedEventHandler AutoRemoveRequested;
        public NotificationModel()
        {
            AutoRemoveTimer.Interval = TimeSpan.FromSeconds(5);
            if (ShouldStartTimer())
            {
                AutoRemoveTimer.Tick += AutoRemoveTimer_Tick;
                AutoRemoveTimer.Start();
            }
        }

        private void AutoRemoveTimer_Tick(object sender, EventArgs e)
        {
            AutoRemoveTimer.Stop();
            AutoRemoveRequested?.Invoke(this, new RoutedEventArgs());
        }

        protected virtual bool ShouldStartTimer()
        {
            return true;
        }
        public abstract int ID { get; set; }

        public abstract string Message { get; set; }

        public abstract string MessageTime { get; set; }

        public abstract DelegateCommand<object> Delete { get; set; }
    }

    public class Notification : PubSubEvent<Notification>
    {
        public enum InfoType
        {
            Info,
            Warning,
            Error,
            Fatal
        }
        public InfoType Type { get; set; }

        public string Message { get; set; }

        public string MessageTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
