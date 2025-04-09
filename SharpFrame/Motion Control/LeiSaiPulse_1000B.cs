using MotionClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MotionControl
{
    public class LeiSaiPulse_1000B : MotionBase
    {
        public override bool[] IO_Input { get; set; }
        public override bool[] IO_Output { get; set; }
        public override ushort[] Card_Number { get; set; }
        public override ushort[] Axis { get; set; }
        public override int[] EtherCATStates { get; set; }
        public override double[][] AxisStates { get; set; }
        public override Thread[] Read_ThreadPool { get; set; }
        public override ManualResetEvent AutoReadEvent { get; set; }
        public override CancellationTokenSource[] Task_Token { get; set; }
        public override CancellationToken[] cancellation_Token { get; set; }
        public override ushort FactorValue { get; set; }
        public override double Speed { get; set; }
        public override double Acc { get; set; }
        public override double Dec { get; set; }
        public override int[][] Axis_IO { get; set; }
        public override short[] CoordinateSystemStates { get; set; }
        public override bool IsOpenCard { get; set; }
        public override int Axisquantity { get; set; }

        public override event Action<DateTime, bool, string> CardLogEvent;

        /// <summary>
        /// 使能标志
        /// </summary>
        private ushort[] ThisStateMac { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        private double[] TargetLocation { get; set; }
        public override ushort CANNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ushort CANState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool CAN_IsOpen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double[] ADC_RealTime_DA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double[] ADC_RealTime_AD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public LeiSaiPulse_1000B()
        {
            Read_ThreadPool = new Thread[2];
            AutoReadEvent = new ManualResetEvent(true);
            Read_ThreadPool[0] = new Thread(ReadState);
            Read_ThreadPool[1] = new Thread(ReadIO);
            IMoveStateQueue = new List<MoveState>();
            MotionBase.Thismotion = this;
        }

        private void ReadIO()
        {
            AxisStates = new double[Axis.Length][];
            Axis_IO = new int[Axis.Length][];
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Restart();

                for (ushort i = 0; i < Axis.Length; i++)
                {
                    IO_Input = Getall_IOinput(i);
                    IO_Output = Getall_IOoutput(i);
                }
                stopwatch.Stop();
                Thread.Sleep(10);
                Console.WriteLine("IO=>" + stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        private void ReadState()
        {
            AxisStates = new double[Axis.Length][];
            Axis_IO = new int[Axis.Length][];
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            while (true)
            {
                stopwatch.Restart();
                //EtherCATStates = GetEtherCATState(0);
                for (ushort i = 0; i < Axis.Length; i++)
                {
                    AxisStates[i] = GetAxisState(i);
                    Axis_IO[i] = GetAxisExternalio(i);
                }
                //for (ushort i = 0; i < 2; i++)
                //{
                //    CoordinateSystemStates[i] = LTDMC.dmc_check_done_multicoor(Card_Number[0], i);
                //}
                stopwatch.Stop();
                Thread.Sleep(10);
                //Console.WriteLine("State=>" + stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        public override void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                var axis_stat = Dmc1000.d1000_check_done(axis);
                if (axis_stat == 1)
                {
                    Dmc1000.d1000_start_sa_move(axis, (int)position, 0, (int)speed, Acc);
                    Thread.Sleep(100);
                    do
                    {
                        Thread.Sleep(50);
                    } while (Dmc1000.d1000_check_done(axis) != 1);
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位完成！");

                }
                else if (axis_stat == 0)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                else if (axis_stat == 3)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴到限位！");
            }
        }

        public override void AwaitMoveHome(ushort axis, short home_model, double home_speed, int timeout = 0, double acc = 0.1, double dcc = 0.1, double offpos = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveLines(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                var axis_stat = Dmc1000.d1000_check_done(axis);
                if (axis_stat == 1)
                {
                    Dmc1000.d1000_start_s_move(axis, (int)position, 0, (int)speed, Acc);
                    Thread.Sleep(100);
                    do
                    {
                        Thread.Sleep(50);
                    } while (Dmc1000.d1000_check_done(axis) != 1);
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位完成！");
                }
                else if (axis_stat == 0)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                else if (axis_stat == 3)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴到限位！");
            }
        }

        public override void AxisBasicSet(ushort axis, double equiv, double startvel, double speed, double acc, double dec, double stopvel, double s_para, int posi_mode, int stop_mode)
        {
            throw new NotImplementedException();
        }

        public override void AxisErrorReset(ushort axis)
        {
            throw new NotImplementedException();
        }

        public override void AxisOff(ushort card, ushort axis)
        {
            throw new NotImplementedException();
        }

        public override void AxisOff()
        {
            throw new NotImplementedException();
        }

        public override void AxisOn(ushort card, ushort axis)
        {
            throw new NotImplementedException();
        }

        public override void AxisOn()
        {
            throw new NotImplementedException();
        }

        public override void AxisReset(ushort axis)
        {
            throw new NotImplementedException();
        }

        public override void AxisStop(ushort axis, int stop_mode, bool all)
        {
            throw new NotImplementedException();
        }

        public override void CloseCard()
        {
            throw new NotImplementedException();
        }

        public override bool[] Getall_IOinput(ushort card)
        {
            if (IsOpenCard)
            {
                if (IO_Input != null)
                {
                    for (int i = 0; i < IO_Input.Length; i++)
                    {
                        var input = Dmc1000.d1000_in_bit(i + 1);
                        IO_Input[i] = input == 0 ? !LevelSignal : LevelSignal;
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
            return IO_Input;
        }

        public override bool[] Getall_IOoutput(ushort card)
        {
            if (IsOpenCard)
            {
                if (IO_Output != null)
                {
                    for (int i = 0; i < IO_Output.Length; i++)
                    {
                        var output = Dmc1000.d1000_get_outbit(i + 1);
                        IO_Output[i] = output == 0 ? !LevelSignal : LevelSignal;
                    }
                }
                else
                {
                    if (Card_Number == null)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                        else
                            throw new Exception($"请先调用OpenCard方法！");
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
            return IO_Output;
        }

        public override int[] GetAxisExternalio(ushort axis)
        {
            int[] bools = new int[3];
            if (IsOpenCard)
            {
                var t = Dmc1000.d1000_get_axis_status(axis);
                for (var i = 0; i < 3; i++)
                {
                    var val = 1 << i;
                    bools[i] = (t & val) == val ? 1 : 0;
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
            return bools;
        }

        public override double[] GetAxisState(ushort axis)
        {
            double[] doubles = new double[8];
            doubles[0] = Dmc1000.d1000_get_command_pos(axis);
            doubles[2] = Dmc1000.d1000_get_speed(axis);
            return doubles;
        }

        public override int[] GetEtherCATState(ushort card_number)
        {
            throw new NotImplementedException();
        }

        public override void MoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                var axis_stat = Dmc1000.d1000_check_done(axis);
                if (axis_stat == 1)
                {
                    Dmc1000.d1000_start_sa_move(axis, (int)position, 0, (int)speed, Acc);
                    Task.Run(() =>
                    {
                        Thread.Sleep(100);
                        do
                        {
                            Thread.Sleep(50);
                        } while (Dmc1000.d1000_check_done(axis) != 1);
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位完成！");
                    });
                }
                else if (axis_stat == 0)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                else if (axis_stat == 3)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴到限位！");
            }
        }

        public override void MoveCircle_Center(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveHome(ushort axis, short home_model, double home_speed, int timeout = 0, double acc = 0.1, double dcc = 0.1, double offpos = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveJog(ushort axis, double speed, int posi_mode, double acc = 0.5, double dec = 0.5)
        {

        }

        public override void MoveLines(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                var axis_stat = Dmc1000.d1000_check_done(axis);
                if (axis_stat == 1)
                {
                    Dmc1000.d1000_start_s_move(axis, (int)position, 0, (int)speed, Acc);
                    Task.Run(() =>
                    {
                        Thread.Sleep(100);
                        do
                        {
                            Thread.Sleep(50);
                        } while (Dmc1000.d1000_check_done(axis) != 1);
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位完成！");
                    });
                }
                else if (axis_stat == 0)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                else if (axis_stat == 3)
                    CardLogEvent(DateTime.Now, true, $"{axis}单轴绝相对定位启动错误！ {axis}轴到限位！");
            }
        }

        public override void MoveReset(ushort axis)
        {
            throw new NotImplementedException();
        }

        public override bool OpenCard(ushort card_number)
        {
            throw new NotImplementedException();
        }

        public override bool OpenCard()
        {
            var ret = Dmc1000.d1000_board_init();
            if (ret > 0)
            {
                IO_Input = new bool[32];
                IO_Output = new bool[27];
                Axisquantity = 4;
                Motion_Lok = new object[Axisquantity];
                ThisStateMac = new ushort[Axisquantity];
                TargetLocation = new double[Axisquantity];
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"初始化{Card_Number.Length}张板卡，总轴数为{Axisquantity}");
                Axis = new ushort[Axisquantity];
                Stop_sign = new bool[Axisquantity];
                IsOpenCard = true;
                for (int i = 0; i < Axis.Length; i++)
                {
                    Stop_sign[i] = false;
                    Motion_Lok[i] = new object();
                }
                if (Read_ThreadPool[0].ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    Read_ThreadPool[0].IsBackground = true;
                    Read_ThreadPool[0].Name = "ReadAxis_State";
                    Read_ThreadPool[0].Start();
                }
                if (Read_ThreadPool[1].ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    Read_ThreadPool[1].IsBackground = true;
                    Read_ThreadPool[1].Name = "ReadCard_io";
                    Read_ThreadPool[1].Start();
                }
                Thread.Sleep(100);
                return true;
            }
            else
            {
                IsOpenCard = false;
                if (Card_Number.Length == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"未查找到{CardBrand}板卡!");
                    else
                        throw new Exception("未查找到板卡!");
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, "有 2 张或 2 张以上控制卡的硬件设置卡号相同!");
                    else
                        throw new Exception("有 2 张或 2 张以上控制卡的硬件设置卡号相同!");
                }
                return false;
            }
        }

        public override void ResetCard(ushort card, ushort reset)
        {
            throw new NotImplementedException();
        }

        public override void SetAxis_iniFile(string path = "AXIS.ini")
        {
            throw new NotImplementedException();
        }

        public override void SetbjectDictionary(ushort card, ushort etherCATLocation, ushort primeindex, ushort wordindexing, ushort bitlength, int value)
        {
            throw new NotImplementedException();
        }

        public override void SetEtherCAT_eniFiel()
        {
            throw new NotImplementedException();
        }

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop)
        {
            throw new NotImplementedException();
        }

        public override void SetExternalTrigger(ushort start1, ushort start2, ushort reset, ushort stop, ushort estop, ushort raster, ushort entrance)
        {
            throw new NotImplementedException();
        }

        public override void SetExternalTrigger(ushort start1, ushort start2, ushort reset, ushort stop, ushort estop)
        {
            throw new NotImplementedException();
        }

        public override void Set_IOoutput(ushort card, ushort indexes, bool value)
        {
            throw new NotImplementedException();
        }

        public override void WaitAxis(int[] axis)
        {
            throw new NotImplementedException();
        }

        public override void Set_CAN_State(ushort card, ushort can_num, bool state)
        {
            throw new NotImplementedException();
        }

        public override void Set_IOoutput_Enum(ushort card, OutPut indexes, bool value)
        {
            throw new NotImplementedException();
        }

        public override void AwaitIOinput_Enum(ushort card, InPuts indexes, bool waitvalue, int timeout = 0)
        {
            throw new NotImplementedException();
        }

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster, ushort entrance)
        {
            throw new NotImplementedException();
        }

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster1, ushort raster2, ushort entrance1, ushort entrance2)
        {
            throw new NotImplementedException();
        }

        public override void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            throw new NotImplementedException();
        }

        public override void Set_DA(ushort card, ushort channel_number, double voltage_values)
        {
            throw new NotImplementedException();
        }

        public override double Read_DA(ushort card, ushort channel_number)
        {
            throw new NotImplementedException();
        }

        public override double Read_AD(ushort card, ushort channel_number)
        {
            throw new NotImplementedException();
        }

        public override void Deploy_CAN(ushort card, ushort can_num, bool can_state, ushort can_baud = 0)
        {
            throw new NotImplementedException();
        }
    }
}
