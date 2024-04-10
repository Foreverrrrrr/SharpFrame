using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SharpFrame.Logic.Base
{
    public abstract class Thread_Auto_Base
    {
        public abstract event Action<DateTime, string> Run_LogEvent;

        public static event Action<DateTime> NewClass_Run;

        public static System.Collections.Generic.List<ProductionThreadBase> Auto_Th { get; private set; } = new System.Collections.Generic.List<ProductionThreadBase>();

        public Thread_Auto_Base()
        {

        }

        public static void NewClass(int spintime = 50)
        {
            Exchange.Cache_Initialization();
            Type baseType = typeof(Thread_Auto_Base);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] derivedTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToArray();
            foreach (Type derivedType in derivedTypes)
            {
                object instance = Activator.CreateInstance(derivedType);
                MethodInfo[] methods = derivedType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo method in methods)
                {
                    var t = method.GetCustomAttributes(typeof(ProductionThreadBase), inherit: true);
                    if (t.Length > 0)
                        Thread_Configuration(derivedType.Name, method, instance, spintime);
                }
            }
            NewClass_Run?.Invoke(DateTime.Now);
        }

        private static void Thread_Configuration(string class_na, MethodInfo method, object class_new, int spintime)
        {
            Thread_Auto_Base instance = class_new as Thread_Auto_Base ?? throw new Exception("Automatic thread conversion exception");
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)method.GetCustomAttribute(typeof(DescriptionAttribute));
            ProductionThreadBase threadBase = new ProductionThreadBase()
            {
                Target = method.Name,
                Thread_Name = descriptionAttribute?.Description ?? method.Name,
                New_Thread = new Thread(() =>
                {
                    while (true) { try { Thread.Sleep(spintime); method.Invoke(instance, new Thread_Auto_Base[] { instance }); } catch (ThreadAbortException ex) { instance.ThreadRestartEvent(class_na, instance, ex); Thread_Configuration(class_na, method, class_new, spintime); GC.Collect(); break; } }
                }),
            };
            threadBase.New_Thread.Name = class_na + "." + threadBase.Thread_Name;
            threadBase.New_Thread.IsBackground = true;
            threadBase.New_Thread.Start();
            Auto_Th.Add(threadBase);
        }

        public static void Thraead_Dispose()
        {
            if (Auto_Th != null)
                if (Auto_Th.Count > 0)
                {
                    var t = Thread_Auto_Base.Auto_Th.ToList();
                    foreach (var item in t)
                    {
                        item.New_Thread.Abort();
                        item.New_Thread.Join();
                    }
                }
        }

        public static void Thraead_Dispose(string Thread_Name)
        {
            if (Auto_Th != null)
                if (Auto_Th.Count > 0)
                {
                    var t = Thread_Auto_Base.Auto_Th.FirstOrDefault(x => x.Thread_Name == Thread_Name);
                    t.New_Thread.Abort();
                    t.New_Thread.Join();
                }
        }

        public abstract void Initialize(object thread);

        public abstract void Error(object thread);

        [ProductionThreadBase]
        public abstract void Main(Thread_Auto_Base thread);

        public abstract void ThreadRestartEvent(string class_na, Thread_Auto_Base thread, ThreadAbortException ex);
    }
}
