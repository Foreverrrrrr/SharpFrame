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

    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
                typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata("666888", OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
                typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, AttachMethod));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
                typeof(PasswordBoxHelper));

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
                if (!(bool)GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue;
                }
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void AttachMethod(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
                // 在附加属性时设置默认密码
                if (!string.IsNullOrEmpty((string)passwordBox.GetValue(PasswordProperty)))
                {
                    passwordBox.Password = (string)passwordBox.GetValue(PasswordProperty);
                }
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
