using System.Windows;

namespace SharpFrame.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SystemLogIn systemLogIn = new SystemLogIn();
            systemLogIn.Show();
        }
    }
}
