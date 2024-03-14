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
                New_Thread = new Thread(() => { while (true) { try { Thread.Sleep(spintime); method.Invoke(instance, new object[] { instance }); } catch (ThreadAbortException ex) { instance.ThreadRestartEvent(ex, class_na); Thread_Configuration(class_na, method, class_new, spintime); break; } } }),
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
                    foreach (var item in Thread_Auto_Base.Auto_Th)
                        item.New_Thread.Abort();
                }
        }

        public static void Thraead_Dispose(string Thread_Name)
        {
            if (Auto_Th != null)
                if (Auto_Th.Count > 0)
                {
                    var t = Thread_Auto_Base.Auto_Th.FirstOrDefault(x => x.Thread_Name == "diyds");
                    t.New_Thread.Abort();
                }
        }

        public abstract void Initialize(object thread);

        [ProductionThreadBase]
        public abstract void Main(object thread);

        public abstract void Error(object thread);

        public abstract void ThreadRestartEvent(ThreadAbortException ex, string class_na);
    }
}
