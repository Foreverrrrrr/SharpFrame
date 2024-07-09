using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

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

        /// <summary>
        /// 参数查询
        /// </summary>
        /// <typeparam name="T">参数泛类</typeparam>
        /// <param name="collection">参数集合</param>
        /// <param name="propertyValue">参数项名称</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static T ParameterQuery<T>(ObservableCollection<T> collection, string propertyValue, string propertyName = "Name")
        {
            if (collection != null && collection.Count > 0)
            {
                PropertyInfo property = typeof(T).GetProperty(propertyName);
                if (property != null)
                {
                    //查找集合中第一个具有指定属性值的对象
                    var result = collection.FirstOrDefault(item =>
                    {
                        var value = property.GetValue(item);
                        return value != null && value.ToString() == propertyValue;
                    });
                    if (result != null)
                        return result;
                }
            }
            throw new System.ArgumentException($"不存在名称为“{propertyValue}”的项");
        }

    }
}
