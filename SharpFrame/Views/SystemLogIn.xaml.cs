using SharpFrame.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SharpFrame.Views
{
    /// <summary>
    /// SystemLogIn.xaml 的交互逻辑
    /// </summary>
    public partial class SystemLogIn : Window
    {
        public static SystemLogInViewModel viewModel;
        public SystemLogIn()
        {
            InitializeComponent();
            viewModel = (SystemLogInViewModel)this.DataContext;
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

    public class PasswordBoxHelper : DependencyObject
    {
        public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));
        public static string GetPassword(DependencyObject d)
        {
            return (string)d.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject d, string value)
        {
            d.SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                passwordBox.Password = (string)e.NewValue;
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
