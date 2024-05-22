using System.Collections.ObjectModel;

namespace SharpFrame.Structure.Parameter
{
    /// <summary>
    /// 程序参数
    /// </summary>
    public class Parameter
    {
        public Parameter()
        {
            SystemParameters_Obse = new ObservableCollection<SystemParameter>();
            PointLocationParameter_Obse = new ObservableCollection<PointLocationParameter>();
            TestParameter_Obse = new ObservableCollection<TestParameter>();
        }

        private ObservableCollection<SystemParameter> _systemparameters_obse;

        public ObservableCollection<SystemParameter> SystemParameters_Obse
        {
            get { return _systemparameters_obse; }
            set { _systemparameters_obse = value; }
        }

        private ObservableCollection<PointLocationParameter> _pointLocationparameter_obse;

        public ObservableCollection<PointLocationParameter> PointLocationParameter_Obse
        {
            get { return _pointLocationparameter_obse; }
            set { _pointLocationparameter_obse = value; }
        }

        private ObservableCollection<TestParameter> _testParameter_obse;

        public ObservableCollection<TestParameter> TestParameter_Obse
        {
            get { return _testParameter_obse; }
            set { _testParameter_obse = value; }
        }
    }
}
