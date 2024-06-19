using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SharpFrame.Logic.Base
{
    public abstract class Thread_Auto_Base
    {
        /// <summary>
        /// 流程线程创建事件
        /// </summary>
        public static event Action<DateTime, string> NewClass_RunEvent;

        /// <summary>
        /// 全局状态机日志事件
        /// </summary>
        public static event Action<DateTime, string> DataConfigurationEvent;

        private static object _lock = new object();

        /// <summary>
        /// 全局状态机机枚举队列
        /// </summary>
        private static List<DataConfigurationBase> DataEnum = new List<DataConfigurationBase>();

        /// <summary>
        /// 全局状态机数据池
        /// </summary>
        private static bool[] DataPool = new bool[65535];

        public static System.Collections.Generic.List<ProductionThreadBase> Auto_Th { get; private set; } = new System.Collections.Generic.List<ProductionThreadBase>();

        /// <summary>
        /// 暂停
        /// </summary>
        public abstract ManualResetEvent Interrupt { get; set; }

        /// <summary>
        /// 日志接口
        /// </summary>
        public abstract event Action<DateTime, string> LogEvent;

        public enum Send_Variable
        {
            Start, AwaitStarted, Suspend, Stop, Reset, E_Stop
        }

        public Thread_Auto_Base() { }

        /// <summary>
        /// 流程初始化
        /// </summary>
        /// <param name="spintime">线程循环休眠时间</param>
        public static void NewClass(int spintime = 50)
        {
            DataStructureConfiguration(typeof(Send_Variable));
            Type baseType = typeof(Thread_Auto_Base);
            Assembly assembly = Assembly.GetEntryAssembly();
            Type[] derivedTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToArray();
            foreach (Type derivedType in derivedTypes)
            {
                object instance = Activator.CreateInstance(derivedType);
                MethodInfo[] methods = derivedType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (MethodInfo method in methods)
                {
                    var t = method.GetCustomAttributes(typeof(ProductionThreadBase), inherit: true);
                    if (t.Length > 0)
                        Thread_Configuration(derivedType.Name, method, instance, spintime);
                }
                NewClass_RunEvent?.Invoke(DateTime.Now, derivedType.FullName + "中自动运行线程启动");
            }
        }

        private static void Thread_Configuration(string class_na, MethodInfo method, object class_new, int spintime)
        {
            Thread_Auto_Base instance = class_new as Thread_Auto_Base ?? throw new Exception("Automatic thread conversion exception");
            if (instance.Interrupt == null)
                instance.Interrupt = new ManualResetEvent(true);
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)method.GetCustomAttribute(typeof(DescriptionAttribute));
            ProductionThreadBase threadBase = new ProductionThreadBase()
            {
                Target = method.Name,
                Thread_Name = descriptionAttribute?.Description ?? method.Name,
                Is_Running = true,
                New_Thread = new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            instance.Interrupt?.WaitOne();
                            Thread.Sleep(spintime);
                            method.Invoke(instance, new Thread_Auto_Base[] { instance });
                        }
                        catch (ThreadAbortException ex)
                        {
                            instance.ThreadRestartEvent(class_na, instance, ex);
                            Thread_Configuration(class_na, method, class_new, spintime);
                            return;
                        }
                        catch (TargetInvocationException ex)
                        {
                            Exception innerException = ex.InnerException;
                            instance.ThreadError(class_na, instance, innerException);
                            Thread_Configuration(class_na, method, class_new, spintime);
                            return;
                        }
                    }
                }),
            };
            threadBase.New_Thread.Name = class_na + "." + threadBase.Thread_Name;
            threadBase.New_Thread.IsBackground = true;
            threadBase.New_Thread.Start();
            Auto_Th.Add(threadBase);
        }

        /// <summary>
        /// 全部流程线程销毁
        /// </summary>
        public static void Thraead_Dispose()
        {
            if (Auto_Th != null)
                if (Auto_Th.Count > 0)
                {
                    var t = Thread_Auto_Base.Auto_Th.ToList();
                    foreach (var item in t)
                    {
                        item.Is_Running = false;
                        Thread_Auto_Base.Auto_Th.Remove(item);
                        item.New_Thread.Abort();
                        item.New_Thread.Join();
                    }
                    GC.Collect();
                }
        }

        /// <summary>
        /// 指定流程线程销毁
        /// </summary>
        /// <param name="Thread_Name">线程名称</param>
        public static void Thraead_Dispose(string Thread_Name)
        {
            if (Auto_Th != null)
                if (Auto_Th.Count > 0)
                {
                    var t = Thread_Auto_Base.Auto_Th.FirstOrDefault(x => x.Thread_Name == Thread_Name);
                    t.Is_Running = false;
                    Thread_Auto_Base.Auto_Th.Remove(t);
                    t.New_Thread.Abort();
                    t.New_Thread.Join();
                    GC.Collect();
                }
        }

        /// <summary>
        /// 信号机枚举配置
        /// </summary>
        /// <param name="enum">枚举类型</param>
        /// <exception cref="Exception"></exception>
        protected static void DataStructureConfiguration(Type @enum)
        {
            if (@enum.IsEnum)
            {
                string enumname = @enum.FullName;
                DataConfigurationEvent?.Invoke(DateTime.Now, $"配置“{enumname}”加入数据池");
                FieldInfo[] fields = @enum.GetFields(BindingFlags.Public | BindingFlags.Static);
                if (fields.Length > 0)
                {
                    foreach (FieldInfo field in fields)
                    {
                        DataConfigurationBase configurationBase = new DataConfigurationBase()
                        {
                            EnumTypeName = enumname,
                            EnumName = field.Name,
                            EnumKey = DataEnum.Count,
                        };
                        DataPool[configurationBase.EnumKey] = false;
                        DataEnum.Add(configurationBase);
                    }
                }
                else
                {
                    throw new Exception("@enum中没有枚举项，无法创建全局数据池");
                }
            }
            else
            {
                throw new Exception("@enum不是枚举类型，请检查@enum的类型");
            }
        }

        /// <summary>
        /// 等待全局状态机
        /// </summary>
        /// <typeparam name="TEnum">状态机枚举</typeparam>
        /// <param name="input">枚举项</param>
        /// <param name="state">等待状态值</param>
        /// <param name="time">超时时间</param>
        public static void AwaitEnum<TEnum>(TEnum input, bool state, int time = 0) where TEnum : Enum
        {
            Type enumType = typeof(TEnum);
            string enumTypeName = enumType.FullName;
            var t = DataEnum.FirstOrDefault(x => x.EnumTypeName == enumTypeName && x.EnumName == input.ToString());
            if (t != null)
            {
                DataConfigurationEvent?.Invoke(DateTime.Now, $"等待“{enumTypeName}”中“{input.ToString()}”信号状态为“{state}”");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    Thread.Sleep(50);
                    if (time > 0 && stopwatch.ElapsedMilliseconds > time)
                        break;
                } while (DataPool[t.EnumKey] != state);
                DataConfigurationEvent?.Invoke(DateTime.Now, $"等待“{enumTypeName}”中“{input.ToString()}”信号状态为“{state}”完成（{stopwatch.ElapsedMilliseconds}）");
            }
        }

        /// <summary>
        /// 等待全局状态机
        /// </summary>
        /// <typeparam name="TEnum">状态机枚举</typeparam>
        /// <param name="input">枚举项</param>
        /// <param name="state">等待状态值</param>
        /// <param name="manual">外部状态标志</param>
        /// <param name="time">超时时间</param>
        public static void AwaitEnum<TEnum>(TEnum input, bool state, ManualResetEvent manual, int time = 0) where TEnum : Enum
        {
            Type enumType = typeof(TEnum);
            string enumTypeName = enumType.FullName;
            var t = DataEnum.FirstOrDefault(x => x.EnumTypeName == enumTypeName && x.EnumName == input.ToString());
            if (t != null)
            {
                DataConfigurationEvent?.Invoke(DateTime.Now, $"等待“{enumTypeName}”中“{input.ToString()}”信号状态为“{state}”");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    manual.WaitOne();
                    Thread.Sleep(50);
                    if (time > 0 && stopwatch.ElapsedMilliseconds > time)
                        break;
                } while (DataPool[t.EnumKey] != state);
                DataConfigurationEvent?.Invoke(DateTime.Now, $"等待“{enumTypeName}”中“{input.ToString()}”信号状态为“{state}”完成（{stopwatch.ElapsedMilliseconds}）");
            }
        }

        /// <summary>
        /// 设置全局状态机
        /// </summary>
        /// <typeparam name="TEnum">状态机枚举</typeparam>
        /// <param name="input">枚举项</param>
        /// <param name="state">设置状态</param>
        public static void SetEnum<TEnum>(TEnum input, bool state) where TEnum : Enum
        {
            Type enumType = typeof(TEnum);
            string enumTypeName = enumType.FullName;
            var t = DataEnum.FirstOrDefault(x => x.EnumTypeName == enumTypeName && x.EnumName == input.ToString());
            if (t != null)
            {
                lock (_lock)
                {
                    DataConfigurationEvent?.Invoke(DateTime.Now, $"设置“{enumTypeName}”中“{input.ToString()}”信号状态为“{state}”");
                    DataPool[t.EnumKey] = state;
                }
            }
        }

        /// <summary>
        /// 获取全局状态机
        /// </summary>
        /// <typeparam name="TEnum">状态机枚举</typeparam>
        /// <param name="input">枚举项</param>
        /// <returns>枚举标签状态</returns>
        public static bool GetEnumValue<TEnum>(TEnum input) where TEnum : Enum
        {
            bool result = false;
            Type enumType = typeof(TEnum);
            string enumTypeName = enumType.FullName;
            var t = DataEnum.FirstOrDefault(x => x.EnumTypeName == enumTypeName && x.EnumName == input.ToString());
            if (t != null)
            {
                lock (_lock)
                {
                    result = DataPool[t.EnumKey];
                }
            }
            return result;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="thread"></param>
        public abstract void Initialize(object thread);

        /// <summary>
        /// 自动运行
        /// </summary>
        /// <param name="thread">线程对象</param>
        [ProductionThreadBase]
        protected abstract void Main(Thread_Auto_Base thread);

        /// <summary>
        /// 线程中断重置回调
        /// </summary>
        /// <param name="class_na">线程类名</param>
        /// <param name="thread">线程对象</param>
        /// <param name="ex">异常</param>
        protected abstract void ThreadRestartEvent(string class_na, Thread_Auto_Base thread, ThreadAbortException ex);

        /// <summary>
        /// 线程异常
        /// </summary>
        /// <param name="class_na">流程类名称</param>
        /// <param name="thread">线程对象</param>
        /// <param name="exception">异常</param>
        protected abstract void ThreadError(string class_na, Thread_Auto_Base thread, Exception exception);
    }
}
