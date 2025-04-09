using System;
using System.Collections.Generic;
using System.Threading;

namespace MotionClass
{
    public class LingHua : MotionBase
    {
        public override bool[] IO_Input { get; set; }
        public override bool[] IO_Output { get; set; }
        public override ushort[] Card_Number { get; set; }
        public override bool CAN_IsOpen { get; set; }
        public override double[] ADC_RealTime_DA { get; set; }
        public override double[] ADC_RealTime_AD { get; set; }
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

        public LingHua()
        {
            Read_ThreadPool = new Thread[2];
            AutoReadEvent = new ManualResetEvent(true);
            Read_ThreadPool[0] = new Thread(ReadState);
            Read_ThreadPool[1] = new Thread(ReadIO);
            IMoveStateQueue = new List<MoveState>();
            MotionBase.Thismotion = this;
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
                Thread.Sleep(20);
                ///Console.WriteLine("State=>" + stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        private void ReadIO()
        {
            while (true)
            {
                for (ushort i = 0; i < Card_Number.Length; i++)
                {

                    IO_Input = Getall_IOinput(i);
                    IO_Output = Getall_IOoutput(i);
                }
            }
        }

        public override void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitIOinput_Enum(ushort card, InPuts indexes, bool waitvalue, int timeout = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveLines(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveRel(ushort axis, double position, double speed, int time = 0)
        {
            throw new NotImplementedException();
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
            if (IsOpenCard)
            {
                for (ushort i = 0; i < Card_Number.Length; i++)
                {
                    for (ushort j = 0; j < Axis.Length; j++)
                    {
                        //ThisStateMac[j] = 0;
                        APS168.APS_set_servo_on(j, 0);
                    }

                }
                AxisStop(Axis[0], 0, true);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"所有轴下使能");
            }
            else
            {

                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先初始化板卡再下使能轴");
                else
                    throw new Exception("请先初始化板卡再下使能轴");
            }
        }

        public override void AxisOn(ushort card, ushort axis)
        {
            throw new NotImplementedException();
        }

        public override void AxisOn()
        {
            if (IsOpenCard)
            {
                for (int i = 0; i < Card_Number.Length; i++)
                {
                    for (ushort j = 0; j < Axis.Length; j++)
                    {
                        //ThisStateMac[j] = 4;
                        APS168.APS_set_servo_on(j, 1);
                        Stop_sign[j] = false;
                        AxisReset(j);
                    }
                }
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"所有轴上使能");
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先初始化板卡再使能轴");
                else
                    throw new Exception("请先初始化板卡再使能轴！");
            }
        }

        public override void AxisReset(ushort axis)
        {
            Stop_sign[axis] = false;
            APS168.APS_reset_sscnet_servo_alarm(axis);
        }

        public override void AxisStop(ushort axis, int stop_mode, bool all)
        {
            if (IsOpenCard)
            {
                if (axis < Axis.Length)
                {
                    if (!all)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{axis}轴停止！");
                        if (stop_mode == 0)
                        {
                            APS168.APS_stop_move(axis);
                            Stop_sign[axis] = false;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴减速停止！");
                        }
                        else
                        {
                            APS168.APS_emg_stop(axis);
                            Stop_sign[axis] = true;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴紧急停止！");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Axis.Length; i++)
                        {
                            APS168.APS_emg_stop(axis);
                            Stop_sign[i] = true;
                            //if (AxisStates[axis][6] == 9)
                            //{
                            //    LTDMC.dmc_stop_multicoor(Card_Number[0], 0, 0);
                            //    LTDMC.dmc_stop_multicoor(Card_Number[0], 1, 0);
                            //}
                            //else
                            //{
                            //    CardErrorMessage(LTDMC.dmc_emg_stop(Card_Number[0]));
                            //}
                        }
                        if (stop_mode == 0)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"全部轴减速停止！");
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"全部轴紧急停止！");
                        }
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"轴号输入错误！");
                    else
                        throw new Exception($"轴号输入错误！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
        }

        public override void CloseCard()
        {
            throw new NotImplementedException();
        }

        public override void Deploy_CAN(ushort card, ushort can_num, bool can_state, ushort can_baud = 0)
        {
            throw new NotImplementedException();
        }

        public override bool[] Getall_IOinput(ushort card)
        {
            if (IsOpenCard)
            {
                if (IO_Input != null)
                {
                    int ref_data = 0;
                    APS168.APS_read_d_input(card, 0, ref ref_data);
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
                    int ref_data = 0;
                    APS168.APS_read_d_output(card, 0, ref ref_data);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Output[i] = (ref_data & (1 << i)) == 0 ? LevelSignal : !LevelSignal;
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
            int[] bools = new int[7];
            if (IsOpenCard)
            {
                var state = APS168.APS_motion_io_status(axis);
                bools[0] = (state & 1) == 1 ? 1 : 0;// 伺服报警 True=ON 
                bools[1] = (state & 2) == 2 ? 1 : 0;// 正限位 True=ON 
                bools[2] = (state & 4) == 4 ? 1 : 0;// 负限位 True=ON 
                bools[4] = (state & 8) == 8 ? 1 : 0;// 原点 True=ON 
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
            ushort[] state = new ushort[2];
            int[] doubles = new int[8];
            if (IsOpenCard)
            {
                APS168.APS_get_position(axis, ref doubles[0]);
                APS168.APS_get_encoder(axis, ref doubles[1]);
                APS168.APS_get_feedback_velocity(axis, ref doubles[2]);
                APS168.APS_get_command(axis, ref doubles[3]);
                var statemobe = APS168.APS_motion_status(axis);
                int mask = 1 << 5;
                doubles[4] = (statemobe & mask) != 0 ? 1 : 0;
                mask = 1 << 30; //doubles[5] = 4;
                doubles[5] = (statemobe & mask) != 0 ? 1 : 0;
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
            double[] doubleArray = Array.ConvertAll(doubles, x => (double)x);
            return doubleArray;
        }

        public override int[] GetEtherCATState(ushort card_number)
        {
            throw new NotImplementedException();
        }

        public override void MoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveCircle_Center(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveJog(ushort axis, double speed, int posi_mode, double acc = 0.5, double dec = 0.5)
        {
            if (IsOpenCard)
            {
                if (axis < Axis.Length)
                {
                    lock (Motion_Lok[axis])
                    {
                        if (AxisStates[axis][4] == 1 && !Stop_sign[axis] && AxisStates[axis][5] == 4)
                        {
                            var t = APS168.APS_jog_mode_switch(axis, 1);
                            JOG_DATA jOG = new JOG_DATA();
                            jOG.i16_jogMode = 1;
                            jOG.i16_dir = (short)posi_mode;
                            jOG.i32_acc = (int)acc;
                            jOG.i32_dec = (int)dec;
                            jOG.i32_maxSpeed = (int)speed;
                            APS168.APS_set_jog_param(axis, ref jOG, 0);
                            APS168.APS_jog_start(axis, 1);
                            //CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], axis, 0, speed, acc, dec, 0));//设置速度参数
                            //LTDMC.dmc_vmove(Card_Number[0], axis, (ushort)posi_mode);
                        }
                        else if (Stop_sign[axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"轴{axis}停止中，请复位轴状态！");
                            else
                                throw new Exception($"轴{axis}停止中，请复位轴状态！");
                        }
                        else if (AxisStates[axis][5] != 4)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"轴{axis}未上使能");
                            else
                                throw new Exception($"轴{axis}未上使能");
                        }
                    }
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"轴{axis}进行JOG运动");
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"轴号输入错误！");
                    else
                        throw new Exception($"轴号输入错误！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
        }

        public override void MoveLines(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveRel(ushort axis, double position, double speed, int time = 0)
        {
            throw new NotImplementedException();
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
            int cardQuantity = 0;
            APS168.APS_initial(ref cardQuantity, 0);//获取设备信息
            Card_Number = new ushort[cardQuantity];
            Stop_sign = new bool[cardQuantity];
            Axisquantity = 4;
            if (Card_Number.Length > 0)
            {
                IO_Input = new bool[80];
                IO_Output = new bool[80];
                Motion_Lok = new object[Axisquantity];
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

        public override double Read_AD(ushort card, ushort channel_number)
        {
            throw new NotImplementedException();
        }

        public override double Read_DA(ushort card, ushort channel_number)
        {
            throw new NotImplementedException();
        }

        public override void ResetCard(ushort card, ushort reset)
        {
            throw new NotImplementedException();
        }

        public override void SetAxis_iniFile(string path = "AXIS.ini")
        {
            APS168.APS_load_param_from_file(path);
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

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster, ushort entrance)
        {
            throw new NotImplementedException();
        }

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster1, ushort raster2, ushort entrance1, ushort entrance2)
        {
            throw new NotImplementedException();
        }

        public override void Set_DA(ushort card, ushort channel_number, double voltage_values)
        {
            throw new NotImplementedException();
        }

        public override void Set_IOoutput(ushort card, ushort indexes, bool value)
        {
            throw new NotImplementedException();
        }

        public override void Set_IOoutput_Enum(ushort card, OutPut indexes, bool value)
        {
            throw new NotImplementedException();
        }

        public override void WaitAxis(int[] axis)
        {
            throw new NotImplementedException();
        }
    }
}
