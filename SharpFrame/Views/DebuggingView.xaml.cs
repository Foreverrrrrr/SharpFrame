using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SharpFrame.Structure.Debugging;
using SharpFrame.ViewModels;

namespace SharpFrame.Views
{
    /// <summary>
    /// DebuggingVision.xaml 的交互逻辑
    /// </summary>
    public partial class DebuggingView : UserControl
    {
        DebuggingViewModel model;
        double[] pos;
        double[] speed;
        string[] stopreason;
        string[] movemodel;
        string[] servoon;
        string[] servocpolice;
        string[] servobusy;
        string[] servostop;
        Color[] input;
        Color[] output;

        public DebuggingView()
        {
            InitializeComponent();
        }

        public void ConfigurationFileReading(string filename)
        {
            model = DataContext as DebuggingViewModel;
            Json_Deploy deploy = new Json_Deploy();
            AxisConfigurationFile.Get_Local_Profile(filename, ref deploy);
            model.InputColor = new Color[deploy.Input.Count];
            model.OutputColor = new Color[deploy.Output.Count];
            pos = new double[deploy.Axis.Count];
            speed = new double[deploy.Axis.Count];
            stopreason = new string[deploy.Axis.Count];
            movemodel = new string[deploy.Axis.Count];
            servoon = new string[deploy.Axis.Count];
            servocpolice = new string[deploy.Axis.Count];
            servobusy = new string[deploy.Axis.Count];
            servostop = new string[deploy.Axis.Count];
            input = new Color[deploy.Input.Count];
            output = new Color[deploy.Output.Count];
            for (int i = 0; i < deploy.Input.Count; i++)
            {
                StackPanel stackPanel1 = new StackPanel();
                stackPanel1.Margin = new Thickness(10, 5, 0, 0);
                Button button = new Button() { Style = (Style)FindResource("MaterialDesignFloatingActionLightButton"), BorderBrush = null, };
                Binding binding = new Binding($"InputColor[{i}]");
                binding.Converter = new ColorToSolidColorBrushConverter();
                binding.Source = model;
                button.SetBinding(Button.BackgroundProperty, binding);
                button.IsTabStop = false;
                button.IsHitTestVisible = false;
                stackPanel1.Children.Add(button);
                TextBlock textBlock1 = new TextBlock() { Text = deploy.Input[i].IO, FontSize = 12, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap, };
                stackPanel1.Children.Add(textBlock1);
                TextBlock textBlock2 = new TextBlock() { Text = deploy.Input[i].Name, Width = 60, FontSize = 12, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };
                stackPanel1.Children.Add(textBlock2);
                Input_Wp.Children.Add(stackPanel1);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            ConfigurationFileReading("2024-03--04");
        }
    }
}
