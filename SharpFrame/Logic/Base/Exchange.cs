using static SharpFrame.Logic.Base.Thread_Auto_Base;

namespace SharpFrame.Logic.Base
{
    public class Exchange
    {
        public static bool External_IO(Send_Variable io)
        {
            switch (io)
            {
                case Send_Variable.Start:
                    if (Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.ResetOver))
                    {
                        if (Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Suspend))//恢复
                        {
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Suspend, false);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Start, true);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.AwaitStarted, true);
                            return true;
                        }
                        else if (Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Reset))//复位后启动
                        {

                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Reset, false);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Start, true);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.AwaitStarted, true);
                            return true;
                        }
                        else if (Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.AwaitStarted)
                            && !Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Start))//连续启动
                        {
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Start, true);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.AwaitStarted, true);
                            return true;
                        }
                    }
                    break;
                case Send_Variable.Suspend:
                    if (Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.ResetOver))
                    {
                        if (Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.AwaitStarted) &&
                            Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Start))//暂停
                        {
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Suspend, true);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Start, false);
                            return true;
                        }
                    }
                    break;
                case Send_Variable.Stop:
                    Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.ResetOver, false);
                    Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Suspend, false);
                    Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Stop, true);
                    Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Reset, false);
                    Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Start, false);
                    Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.AwaitStarted, false);
                    return true;
                case Send_Variable.Reset:
                    if (!Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.ResetOver))
                    {
                        if ((Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Stop)
                            || Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Suspend))
                            && !Thread_Auto_Base.GetEnumValue(Thread_Auto_Base.Send_Variable.Reset))
                        {
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.ResetOver, false);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Suspend, false);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Stop, false);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Reset, true);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.AwaitStarted, false);
                            Thread_Auto_Base.SetEnum(Thread_Auto_Base.Send_Variable.Suspend, false);
                            Thread_Auto_Base.Thraead_Dispose();
                            return true;
                        }
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
