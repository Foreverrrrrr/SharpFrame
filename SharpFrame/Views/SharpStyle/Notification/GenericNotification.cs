using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SharpFrame.Views.SharpStyle
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Notification_Popu"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Notification_Popu;assembly=Notification_Popu"
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
    ///     <MyNamespace:SuspensionNotice/>
    ///
    /// </summary>
    public class GenericNotification : Control
    {
        static GenericNotification()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GenericNotification), new FrameworkPropertyMetadata(typeof(GenericNotification)));
        }

        public static readonly DependencyProperty IsNoticeProperty =
            DependencyProperty.Register("IsNotice", typeof(ObservableCollection<NotificationModel>), typeof(GenericNotification), new PropertyMetadata(new ObservableCollection<NotificationModel>(), OnIsNoticeChanged));

        public ObservableCollection<NotificationModel> IsNotice
        {
            get { return (ObservableCollection<NotificationModel>)GetValue(IsNoticeProperty); }
            set { SetValue(IsNoticeProperty, value); }
        }

        private static void OnIsNoticeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var suspensionNotice = (GenericNotification)d;
            suspensionNotice.UpdateNotifications();
        }

        private void UpdateNotifications()
        {
            // 这里可以根据 IsNotice 集合的内容更新通知显示
            // 暂时省略具体实现，后续会在模板中处理
        }
    }
}
