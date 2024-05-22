using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpFrame.Views
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
        }
    }

    public class ConVerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "Error")
            {
                return Brushes.Red;
            }
            else if (value.ToString() == "Normal")
            {
                return Brushes.LawnGreen;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
