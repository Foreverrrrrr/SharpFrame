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
using System.Windows.Shapes;

namespace SharpFrame.Views
{
    /// <summary>
    /// SystemLogIn.xaml 的交互逻辑
    /// </summary>
    public partial class SystemLogIn : Window
    {
        public SystemLogIn()
        {
            InitializeComponent();
        }
    }

    public abstract class Increaser<T> : DependencyObject
    {
        public abstract T Next { get; }
        public virtual T Start { get; set; }
        public T Step { get; set; }
    }

    public class TimeSpanIncreaser : Increaser<TimeSpan>
    {
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(TimeSpan), typeof(TimeSpanIncreaser),
                new PropertyMetadata(default(TimeSpan)));

        private TimeSpan _current;

        public override TimeSpan Next
        {
            get
            {
                var result = Start + _current;
                _current += Step;
                return result;
            }
        }

        public override TimeSpan Start
        {
            get => (TimeSpan)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }
    }

    public class DurationIncreaser : Increaser<Duration>
    {
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(Duration), typeof(DurationIncreaser),
                new PropertyMetadata(new Duration(TimeSpan.Zero)));

        private Duration _current = new Duration(TimeSpan.Zero);

        public override Duration Next
        {
            get
            {
                var result = Start + _current;
                _current += Step;
                return result;
            }
        }

        public override Duration Start
        {
            get => (Duration)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }
    }

}
