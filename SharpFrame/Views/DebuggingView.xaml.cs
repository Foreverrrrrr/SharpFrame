using MotionClass;
using SharpFrame.Structure.Debugging;
using SharpFrame.ViewModels;
using SharpFrame.Views.SharpStyle;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpFrame.Views
{
    /// <summary>
    /// DebuggingVision.xaml 的交互逻辑
    /// </summary>
    public partial class DebuggingView : UserControl
    {
        DebuggingViewModel model;
        MotionBase motion;
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
                TextBlock textBlock1 = new TextBlock() { Text = deploy.Input[i].IO, FontSize = 14, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap, };
                stackPanel1.Children.Add(textBlock1);
                TextBlock textBlock2 = new TextBlock() { Text = deploy.Input[i].Name, Width = 60, FontSize = 14, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };
                stackPanel1.Children.Add(textBlock2);
                Input_Wp.Children.Add(stackPanel1);
            }
            for (int i = 0; i < deploy.Output.Count; i++)
            {
                StackPanel stackPanel2 = new StackPanel();
                stackPanel2.Margin = new Thickness(10, 5, 0, 0);
                Button button = new Button() { Style = (Style)FindResource("MaterialDesignFloatingActionLightButton"), BorderBrush = null, Name = $"Output{i}" };
                Binding binding = new Binding($"OutputColor[{i}]");
                binding.Converter = new ColorToSolidColorBrushConverter();
                binding.Source = model;
                button.SetBinding(Button.BackgroundProperty, binding);
                button.IsTabStop = false;
                button.Click += Set_OutPut;
                button.CommandParameter = i.ToString();
                button.SetBinding(Button.CommandProperty, new System.Windows.Data.Binding("OutButton"));
                stackPanel2.Children.Add(button);
                TextBlock textBlock1 = new TextBlock() { Text = deploy.Output[i].IO, FontSize = 14, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap, };
                stackPanel2.Children.Add(textBlock1);
                TextBlock textBlock2 = new TextBlock() { Text = deploy.Output[i].Name, Width = 60, FontSize = 14, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };
                stackPanel2.Children.Add(textBlock2);
                Output_Wp.Children.Add(stackPanel2);
            }
            for (int i = 0; i < deploy.Axis.Count; i++)
            {
                AxisManualOperation axisManual = new AxisManualOperation();
                axisManual.AxisName = deploy.Axis[i].Axis_Name;
                Binding binding_location = new Binding($"Pos");
                binding_location.Source = model;
                binding_location.Converter = new ArrayToDoubleConverter();
                binding_location.ConverterParameter = i;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.PosProperty, binding_location);

                Binding binding_Speed = new Binding($"Speed");
                binding_Speed.Converter = new ArrayToDoubleConverter();
                binding_Speed.Source = model;
                binding_Speed.ConverterParameter = i;

                BindingOperations.SetBinding(axisManual, AxisManualOperation.SpeedProperty, binding_Speed);

                Binding binding_MoveModel = new Binding($"MoveModel");
                binding_MoveModel.Converter = new ArrayToStringConverter();
                binding_MoveModel.Source = model;
                binding_MoveModel.ConverterParameter = i;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.MoveModelProperty, binding_MoveModel);

                Binding binding_StopCause = new Binding($"StopCause");
                binding_StopCause.Converter = new ArrayToStringConverter();
                binding_StopCause.Source = model;
                binding_StopCause.ConverterParameter = i;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.StopCauseProperty, binding_StopCause);

                Binding binding_Set_Speed = new Binding($"Set_Speed_axis" + i);
                binding_Set_Speed.Source = model;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.Set_SpeedProperty, binding_Set_Speed);

                Binding binding_Import_location = new Binding($"Import_location[{i}]");
                binding_Import_location.Source = model;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.Import_locationProperty, binding_Import_location);

                Binding binding_Servo_Run_State = new Binding($"ServoOn[{i}]");
                binding_Servo_Run_State.Source = model;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.Servo_Run_StateProperty, binding_Servo_Run_State);

                Binding binding_Error_State = new Binding($"ServoCpolice[{i}]");
                binding_Error_State.Source = model;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.Error_StateProperty, binding_Error_State);

                Binding binding_Operation_State = new Binding($"Operation_State[{i}]");
                binding_Operation_State.Source = model;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.Operation_StateProperty, binding_Operation_State);

                Binding binding_EStop_State = new Binding($"EStop_State[{i}]");
                binding_EStop_State.Source = model;
                BindingOperations.SetBinding(axisManual, AxisManualOperation.EStop_StateProperty, binding_EStop_State);
                axisManual.Jogjust_PreviewMouseLeft += (o, e) =>
                {
                    Button button = o as Button;
                    var tk = button.TemplatedParent as AxisManualOperation;
                    if (motion != null && motion.IsOpenCard)
                    {
                        switch (tk.AxisName)
                        {
                            case "X":
                                motion.MoveJog(0, model.Set_Speed_axis0, 1, 0.2, 0.2); break;
                            case "Y":
                                motion.MoveJog(1, model.Set_Speed_axis1, 1, 0.2, 0.2); break;
                            case "Z":
                                motion.MoveJog(2, model.Set_Speed_axis2, 1, 0.2, 0.2); break;
                            case "Width1":
                                motion.MoveJog(3, model.Set_Speed_axis3, 1, 0.2, 0.2); break;
                            case "Width2":
                                motion.MoveJog(4, model.Set_Speed_axis4, 1, 0.2, 0.2); break;
                        }
                    }
                    //else
                    // model.MessageLog(DateTime.Now.ToString() + "运动控制卡未打开", model.Abnormal, 5);
                };
                axisManual.Jog_Stop += (o, e) =>
                {
                    Button button = o as Button;
                    var tk = button.TemplatedParent as AxisManualOperation;
                    if (motion != null && motion.IsOpenCard)
                    {
                        switch (tk.AxisName)
                        {
                            case "X":
                                motion.AxisStop(0, 0, false); break;
                            case "Y":
                                motion.AxisStop(1, 0, false); break;
                            case "Z":
                                motion.AxisStop(2, 0, false); break;
                            case "Width1":
                                motion.AxisStop(3, 0, false); break;
                            case "Width2":
                                motion.AxisStop(4, 0, false); break;
                        }
                    }
                    //else
                    //    model.MessageLog(DateTime.Now.ToString() + "运动控制卡未打开", model.Abnormal, 5);
                };
                axisManual.Joglose_PreviewMouseLeft += (o, e) =>
                {
                    Button button = o as Button;
                    var tk = button.TemplatedParent as AxisManualOperation;
                    if (motion != null && motion.IsOpenCard)
                    {
                        switch (tk.AxisName)
                        {
                            case "X":
                                motion.MoveJog(0, model.Set_Speed_axis0, 0, 0.2, 0.2); break;
                            case "Y":
                                motion.MoveJog(1, model.Set_Speed_axis1, 0, 0.2, 0.2); break;
                            case "Z":
                                motion.MoveJog(2, model.Set_Speed_axis2, 0, 0.2, 0.2); break;
                            case "Width1":
                                motion.MoveJog(3, model.Set_Speed_axis3, 0, 0.2, 0.2); break;
                            case "Width2":
                                motion.MoveJog(4, model.Set_Speed_axis4, 0, 0.2, 0.2); break;
                        }
                    }
                    //else
                    //    model.MessageLog(DateTime.Now.ToString() + "运动控制卡未打开", model.Abnormal, 5);
                };
                axisManual.Move_Home += (o, e) =>
                {
                    Button button = o as Button;
                    var tk = button.TemplatedParent as AxisManualOperation;
                    if (motion != null && motion.IsOpenCard)
                    {
                        switch (tk.AxisName)
                        {
                            case "X":
                                motion.MoveHome(0, 3, -model.Set_Speed_axis0, 0, 0.2, 0.2); break;
                            case "Y":
                                motion.MoveHome(1, 3, -model.Set_Speed_axis1, 0, 0.2, 0.2); break;
                            case "Z":
                                motion.MoveHome(2, 2, -model.Set_Speed_axis2, 0, 0.2, 0.2); break;
                            case "Width1":
                                motion.MoveHome(3, 2, -model.Set_Speed_axis3, 0, 0.2, 0.2); break;
                            case "Width2":
                                motion.MoveHome(4, 2, -model.Set_Speed_axis4, 0, 0.2, 0.2); break;
                        }
                    }
                    //else
                    //    model.MessageLog(DateTime.Now.ToString() + "运动控制卡未打开", model.Abnormal, 5);
                };
                axisManual.Move_Rel += (o, e) =>
                {
                    Button button = o as Button;
                    var tk = button.TemplatedParent as AxisManualOperation;
                    if (motion != null && motion.IsOpenCard)
                    {
                        switch (tk.AxisName)
                        {
                            case "X":
                                motion.MoveRel(0, model.Pos[0], model.Set_Speed_axis0); break;
                            case "Y":
                                motion.MoveRel(1, model.Pos[1], model.Set_Speed_axis1); break;
                            case "Z":
                                motion.MoveRel(2, model.Pos[2], model.Set_Speed_axis2); break;
                            case "Width1":
                                motion.MoveRel(3, model.Pos[3], model.Set_Speed_axis3); break;
                            case "Width2":
                                motion.MoveRel(4, model.Pos[4], model.Set_Speed_axis4); break;
                        }
                    }
                };
                axisManual.Servo_Run += (o, e) =>
                {

                };
                axisManual.Error_Rest += (o, e) =>
                {

                };
                axisManual.E_Stop += (o, e) =>
                {
                    if (motion != null && motion.IsOpenCard)
                    {
                        for (ushort t = 0; t < motion.Axis.Length; t++)
                        {
                            motion.AxisStop(t, 0, false);
                        }
                    }
                    //else
                    //    model.MessageLog(DateTime.Now.ToString() + "运动控制卡未打开", model.Abnormal, 5);
                };
                Servo_Wp.Children.Add(axisManual);
            }
        }

        private void Set_OutPut(object sender, RoutedEventArgs e)
        {
            if (model.ThisMotion != null && model.ThisMotion.IsOpenCard)
            {
                Button button = sender as Button;
                int output = Convert.ToInt32(button.Name.Replace("Output", ""));
                if (model.ThisMotion.IO_Output[output])
                    model.ThisMotion.Set_IOoutput(0, (ushort)output, false);
                else
                    model.ThisMotion.Set_IOoutput(0, (ushort)output, true);
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
