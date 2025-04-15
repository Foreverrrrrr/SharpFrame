using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpFrame.ViewModels
{
    public class DebuggingViewModel : BindableBase
    {
        private IEventAggregator eventAggregator;
        private IDialogService dialogService;
        public DebuggingViewModel(IEventAggregator aggregator, IDialogService dialog)
        {
            this.eventAggregator = aggregator;
            this.dialogService = dialog;
            aggregator.GetEvent<PageLoadEvent>().Subscribe((classobj) =>
            {

            });
            aggregator.GetEvent<Close_MessageEvent>().Subscribe(() =>
            {

            });
            OutButton = new DelegateCommand<string>((x) =>
            {

            });
        }

        public DelegateCommand<string> OutButton { set; get; }

        #region 轴数据
        //private MotionBase _thismotion;

        //public MotionBase ThisMotion
        //{
        //    get { return _thismotion; }
        //    set { _thismotion = value; }
        //}

        private System.Windows.Media.Color[] _inputcolor;
        /// <summary>
        /// 数字输入UI
        /// </summary>
        public System.Windows.Media.Color[] InputColor
        {
            get { return _inputcolor; }
            set { SetProperty(ref _inputcolor, value); }
        }

        private System.Windows.Media.Color[] _outputcolor;
        /// <summary>
        /// 数字输出UI
        /// </summary>
        public System.Windows.Media.Color[] OutputColor
        {
            get { return _outputcolor; }
            set { SetProperty(ref _outputcolor, value); }
        }

        private double[] _speed = new double[3];
        /// <summary>
        /// 轴速度
        /// </summary>
        public double[] Speed
        {
            get { return _speed; }
            set { SetProperty(ref _speed, value); }
        }

        private double[] _pos = new double[3];
        /// <summary>
        /// 轴位置
        /// </summary>
        public double[] Pos
        {
            get { return _pos; }
            set { SetProperty(ref _pos, value); }
        }

        private string[] _movemodel = new string[3];
        /// <summary>
        /// 运动模式
        /// </summary>
        public string[] MoveModel
        {
            get { return _movemodel; }
            set { SetProperty(ref _movemodel, value); }
        }

        private string[] _stopcause = new string[3];
        /// <summary>
        /// 停止原因
        /// </summary>
        public string[] StopCause
        {
            get { return _stopcause; }
            set { SetProperty(ref _stopcause, value); }
        }

        private double _set_speed_axis0;
        /// <summary>
        /// JogAXIS1速度
        /// </summary>
        public double Set_Speed_axis0
        {
            get { return _set_speed_axis0; }
            set { SetProperty(ref _set_speed_axis0, value); }
        }

        private double _set_speed_axis1;
        /// <summary>
        /// JogAXIS2速度
        /// </summary>
        public double Set_Speed_axis1
        {
            get { return _set_speed_axis1; }
            set { SetProperty(ref _set_speed_axis1, value); }
        }

        private double _set_speed_axis2;
        /// <summary>
        /// JogAXIS3速度
        /// </summary>
        public double Set_Speed_axis2
        {
            get { return _set_speed_axis2; }
            set { SetProperty(ref _set_speed_axis2, value); }
        }

        private double[] _import_location = new double[3];
        /// <summary>
        /// 定位地址
        /// </summary>
        public double[] Import_location
        {
            get { return _import_location; }
            set { SetProperty(ref _import_location, value); }
        }

        private string[] _servoon = new string[3];
        /// <summary>
        /// 伺服使能状态
        /// </summary>
        public string[] ServoOn
        {
            get { return _servoon; }
            set { SetProperty(ref _servoon, value); }
        }

        private string[] _servocpolice = new string[3];
        /// <summary>
        /// 伺服报警
        /// </summary>
        public string[] ServoCpolice
        {
            get { return _servocpolice; }
            set { SetProperty(ref _servocpolice, value); }
        }

        private string[] _operation_state = new string[3] { "#FFFF0000", "#FFFF0000", "#FFFF0000" };
        /// <summary>
        /// 伺服动态
        /// </summary>
        public string[] Operation_State
        {
            get { return _operation_state; }
            set { SetProperty(ref _operation_state, value); }
        }

        private string[] _estop_state = new string[3];
        /// <summary>
        /// 紧急停止
        /// </summary>
        public string[] EStop_State
        {
            get { return _estop_state; }
            set { SetProperty(ref _estop_state, value); }
        }
        #endregion   
    }

    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            else if (value == null)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string[] array && parameter is int index && index >= 0 && index < array.Length)
            {
                return array[index]; // 返回指定索引的数组元素
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ArrayToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double[] array && parameter is int index && index >= 0 && index < array.Length)
            {
                return array[index]; // 返回指定索引的数组元素
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (double)value;
        }
    }
}
