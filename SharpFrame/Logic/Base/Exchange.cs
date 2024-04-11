using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace SharpFrame.Logic.Base
{
    public class Exchange
    {
        private static bool Initialization_State { get; set; }

        public static bool[] Variable { get; set; }

        public static bool[] Error { get; set; }

        public enum Send_Variable : ushort
        {
            Start, AwaitStarted, Suspend, Stop, Reset, E_Stop
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

        public static bool External_IO(Send_Variable io)
        {
            if (Exchange.Variable == null)
                Cache_Initialization();
            switch (io)
            {
                case Send_Variable.Start:
                    if (Exchange.Variable[(int)Exchange.Send_Variable.Suspend])//恢复
                    {
                        Exchange.Set_Output(Exchange.Send_Variable.Suspend, false);
                        Exchange.Set_Output(Exchange.Send_Variable.Start, true);
                        Exchange.Set_Output(Exchange.Send_Variable.AwaitStarted, true);
                        return true;
                    }
                    else if (Exchange.Variable[(int)Exchange.Send_Variable.Reset])//复位后启动
                    {
                        Exchange.Set_Output(Exchange.Send_Variable.Reset, false);
                        Exchange.Set_Output(Exchange.Send_Variable.Start, true);
                        Exchange.Set_Output(Exchange.Send_Variable.AwaitStarted, true);
                        return true;
                    }
                    else if (Exchange.Variable[(int)Exchange.Send_Variable.AwaitStarted] && !Exchange.Variable[(int)Exchange.Send_Variable.Start])//连续启动
                    {
                        Exchange.Set_Output(Exchange.Send_Variable.Start, true);
                        Exchange.Set_Output(Exchange.Send_Variable.AwaitStarted, true);
                        return true;
                    }
                    break;
                case Send_Variable.Suspend:
                    if (Exchange.Variable[(int)Exchange.Send_Variable.AwaitStarted] && Exchange.Variable[(int)Exchange.Send_Variable.Start])//暂停
                    {
                        Exchange.Set_Output(Exchange.Send_Variable.Suspend, true);
                        Exchange.Set_Output(Exchange.Send_Variable.Start, false);
                        return true;
                    }
                    break;
                case Send_Variable.Stop:
                    Exchange.Set_Output(Exchange.Send_Variable.Suspend, false);
                    Exchange.Set_Output(Exchange.Send_Variable.Stop, true);
                    Exchange.Set_Output(Exchange.Send_Variable.Reset, false);
                    Exchange.Set_Output(Exchange.Send_Variable.Start, false);
                    Exchange.Set_Output(Exchange.Send_Variable.AwaitStarted, false);
                    return true;
                case Send_Variable.Reset:
                    if ((Exchange.Variable[(int)Exchange.Send_Variable.Stop] || Exchange.Variable[(int)Exchange.Send_Variable.Suspend])
                 && !Exchange.Variable[(int)Exchange.Send_Variable.Reset])
                    {
                        Thread_Auto_Base.Thraead_Dispose();
                        Exchange.Set_Output(Exchange.Send_Variable.Suspend, false);
                        Exchange.Set_Output(Exchange.Send_Variable.Reset, true);
                        Exchange.Set_Output(Exchange.Send_Variable.Stop, false);
                        Exchange.Set_Output(Exchange.Send_Variable.AwaitStarted, false);
                        Exchange.Set_Output(Exchange.Send_Variable.Suspend, false);
                        return true;
                    }
                    break;
                case Send_Variable.E_Stop:
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
