using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SharpFrame.Logic.Base
{
    public class Exchange
    {
        private static bool Initialization_State { get; set; }

        public static bool[] Variable { get; set; }

        public static bool[] Error { get; set; }

        public enum Send_Variable : ushort
        {
            启动, 停止, 复位, 紧急停止
        }

        public enum Send_Error : ushort
        {

        }

        public static void Cache_Initialization()
        {
            Type classType = typeof(Exchange);
            var enumTypes = classType.GetNestedTypes().Where(t => t.IsEnum);
            foreach (Type enumType in enumTypes)
            {
                var t = Enum.GetNames(enumType);
                if (enumType.Name == "Send_Variable")
                {
                    Variable = new bool[t.Length];
                }
                else if (enumType.Name == "Send_Error")
                {
                    Error = new bool[t.Length];
                }
            }
            Initialization_State = true;
        }

        public static void AwaitInput(Send_Variable input, bool state, int time = 0)
        {
            if (Initialization_State)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    Thread.Sleep(50);
                    if (time > 0 && stopwatch.ElapsedMilliseconds > time)
                        break;
                } while (Variable[(ushort)input] != state);
            }
        }

        public static void AwaitInput(Send_Variable input1, Send_Variable input2, bool state, int time = 0)
        {
            if (Initialization_State)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    Thread.Sleep(50);
                    if (time > 0 && stopwatch.ElapsedMilliseconds > time)
                        break;
                } while (Variable[(ushort)input1] != state || Variable[(ushort)input2] != state);
            }
        }

        public static void Set_Output(Send_Variable input, bool state)
        {
            if (Initialization_State)
                Variable[(ushort)input] = state;
        }
    }
}
