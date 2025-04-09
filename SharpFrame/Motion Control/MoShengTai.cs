using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MotionClass
{
    public sealed class MoShengTai : MotionBase
    {
        /// <summary>
        /// 启动按钮上升沿触发事件
        /// </summary>
        public override event Action<DateTime> StartPEvent;

        /// <summary>
        /// 启动按钮下降沿触发事件
        /// </summary>
        public override event Action<DateTime> StartNEvent;

        /// <summary>
        /// 复位按钮上升沿触发事件
        /// </summary>
        public override event Action<DateTime> ResetPEvent;

        /// <summary>
        /// 复位按钮下降沿触发事件
        /// </summary>
        public override event Action<DateTime> ResetNEvent;

        /// <summary>
        /// 停止按钮上升沿触发事件
        /// </summary>
        public override event Action<DateTime> StopPEvent;

        /// <summary>
        /// 停止按钮下降沿触发事件
        /// </summary>
        public override event Action<DateTime> StopNEvent;

        /// <summary>
        /// 紧急停止按钮上升沿触发事件
        /// </summary>
        public override event Action<DateTime> EStopPEvent;

        /// <summary>
        /// 紧急停止按钮下降沿触发事件
        /// </summary>
        public override event Action<DateTime> EStopNEvent;

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 连接站号类型
        /// </summary>
        public enum ModelType : ushort
        {
            /// <summary>
            /// 40/32/16点IO模块
            /// </summary>
            NIO2416_161R_0808R,
            /// <summary>
            /// 80/64点IO模块
            /// </summary>
            NIO4832_3232,
            /// <summary>
            /// 4/4/4/2/1轴网络总线运动控制卡
            /// </summary>
            NMC3400_3401_1400_1200_1100R,
            /// <summary>
            /// 6/8轴网络总线控制卡
            /// </summary>
            NMC5800_5600_1800_1600R,
            /// <summary>
            /// 12/16轴网络总线运动控制卡
            /// </summary>
            NMC5160_5120,
            /// <summary>
            /// 网络运动控制激光卡
            /// </summary>
            LMC3400R_LMC3100R,
            /// <summary>
            /// 暂未开放
            /// </summary>
            EIO0840
        }
        /// <summary>
        /// 输入输出高低电平反转
        /// </summary>
        public override bool LevelSignal { get; set; }
        /// <summary>
        /// 总线轴总数
        /// </summary>
        public override int Axisquantity { get; set; } = -1;
        public bool[] Special_io { get; set; }
        public override bool[] IO_Input { get; set; }
        public override bool[] IO_Output { get; set; }
        public override ushort[] Card_Number { get; set; }
        public override ushort[] Axis { get; set; }
        public override int[] EtherCATStates { get; set; }
        public override double[][] AxisStates { get; set; }
        public override ManualResetEvent AutoReadEvent { get; set; }
        public override ushort FactorValue { get; set; }
        public override double Speed { get; set; }
        public override double Acc { get; set; } = 0.5;
        public override double Dec { get; set; } = 0.5;
        public override int[][] Axis_IO { get; set; }
        public override short[] CoordinateSystemStates { get; set; } = new short[2];
        /// <summary>
        /// 板卡是否打开
        /// </summary>
        public override bool IsOpenCard { get; set; }
        public override Thread[] Read_ThreadPool { get; set; } = new Thread[4];

        public override event Action<DateTime, bool, string> CardLogEvent;

        /// <summary>
        /// 目标位置
        /// </summary>
        private double[] TargetLocation { get; set; }

        /// <summary>
        /// 轴状态机
        /// </summary>
        private int[] AxisMachine { get; set; }
        /// <summary>
        /// 轴运动模式
        /// </summary>
        private int[] AxisMoveModel { get; set; }
        /// <summary>
        /// 运动控制类方法内部Taks线程令牌
        /// </summary>
        public override CancellationTokenSource[] Task_Token { get; set; }
        public override CancellationToken[] cancellation_Token { get; set; }
        public override bool CAN_IsOpen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double[] ADC_RealTime_DA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double[] ADC_RealTime_AD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MoShengTai()
        {
            AutoReadEvent = new ManualResetEvent(true);
            Read_ThreadPool[0] = new Thread(Read);
            IMoveStateQueue = new List<MoveState>();
            MotionBase.Thismotion = this;
        }

        public override void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，单轴阻塞绝对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，单轴阻塞绝对定位启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，单轴阻塞绝对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，单轴阻塞绝对定位启动失败！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞绝对定位启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴阻塞绝对定位启动错误！ {axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                MoveState state;
                TargetLocation[axis] = position;
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 1;
                state = new MoveState()
                {
                    Axis = axis,
                    CurrentPosition = AxisStates[axis][0],
                    Speed = speed,
                    Position = position,
                    Movetype = 3,
                    OutTime = time,
                    Handle = DateTime.Now,
                };
                lock (Motion_Lok[axis])
                {
                    if (IMoveStateQueue.Exists(e => e.Axis == axis))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                        IMoveStateQueue.Remove(colose);
                    }
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴绝对阻塞定位启动，速度{speed}pulse/S，目标位置{position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(axis, 0, speed, speed / Acc, speed / Acc, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(axis, (int)position, 0);
                    IMoveStateQueue.Add(state);
                }
                Thread.Sleep(50);
                do
                {
                    Thread.Sleep(20);
                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                        goto Timeout;
                    if (Stop_sign[axis])
                        goto Stop;
                } while (AxisStates[state.Axis][4] != 1);
                AxisMachine[axis] = 4;
                AxisMoveModel[axis] = 0;
                lock (this)
                {
                    IMoveStateQueue.Remove(state);
                }
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{axis}轴绝对阻塞定位完成，用时：{stopwatch.Elapsed}");
                return;
            Timeout:
                stopwatch.Stop();
                AxisStop(axis);
                Console.WriteLine(stopwatch.Elapsed);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                return;
            Stop:
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                return;
            }
        }

        public override void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，原点回归启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，原点回归启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，原点回归启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，原点回归启动失败！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}轴原点回归启动错误！ {axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                MoveState state;
                TargetLocation[axis] = 0;
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 3;
                state = new MoveState()
                {
                    Axis = axis,
                    CurrentPosition = AxisStates[axis][0],
                    Speed = home_speed,
                    ACC = acc,
                    Dcc = dcc,
                    Position = 0,
                    Movetype = 6,
                    HomeModel = home_model,
                    Home_off = offpos,
                    OutTime = timeout,
                    Handle = DateTime.Now,
                };
                lock (Motion_Lok[axis])
                {
                    if (IMoveStateQueue.Exists(e => e.Axis == axis))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                        IMoveStateQueue.Remove(colose);
                    }
                    IMoveStateQueue.Add(state);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴阻塞原点回归启动，速度{home_speed}pulse/S，原点回归模式{home_model}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Search_Home_Stop_Time_Net(axis, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Set_Net(axis, home_model, 0, 0, 0, home_model / acc, home_model / acc, 0, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Start_Net(axis, 0);
                }
                Thread.Sleep(50);
                ushort Home_State = 0;
                do
                {
                    if (timeout != 0 && stopwatch.Elapsed.TotalMilliseconds > timeout)
                        goto Timeout;
                    if (Stop_sign[axis])
                        goto Stop;
                    Thread.Sleep(50);
                    CMCDLL_NET.MCF_Search_Home_Get_State_Net(axis, ref Home_State, 0);
                } while (Home_State == 32);
                stopwatch.Stop();
                if (Home_State == 0)
                {
                    lock (this)
                    {
                        IMoveStateQueue.Remove(state);
                    }
                    AxisMachine[axis] = 4;
                    AxisMoveModel[axis] = 0;
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴阻塞原点回归完成，用时：{stopwatch.Elapsed}");
                }
                else if (Home_State == 31)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴阻塞原点回归错误，用时：{stopwatch.Elapsed}");
                }
                return;
            Timeout:
                stopwatch.Stop();
                AxisStop(axis);
                Console.WriteLine(stopwatch.Elapsed);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{axis}轴阻塞原点回归等待到位超时 （{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{axis}轴阻塞原点回归等待到位超时（{stopwatch.Elapsed}）");
                return;
            Stop:
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{axis}轴阻塞原点回归外部异常停止！（{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{axis}轴阻塞原点回归外部异常停止！（{stopwatch.Elapsed}）");
                return;
            }
        }

        public override void AwaitMoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，单轴阻塞相对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，单轴阻塞相对定位启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，单轴阻塞相对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，单轴阻塞相对定位启动失败！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞相对定位启动错误， {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴阻塞相对定位启动错误， {axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                MoveState state;
                TargetLocation[axis] = AxisStates[axis][0] + position;
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 1;
                state = new MoveState()
                {
                    Axis = axis,
                    CurrentPosition = AxisStates[axis][0],
                    Speed = speed,
                    Position = position,
                    Movetype = 4,
                    OutTime = time,
                    Handle = DateTime.Now,
                };
                lock (Motion_Lok[axis])
                {
                    if (IMoveStateQueue.Exists(e => e.Axis == axis))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                        IMoveStateQueue.Remove(colose);
                    }
                    IMoveStateQueue.Add(state);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴相对阻塞定位启动，速度{speed}pulse/S，目标位置{state.Position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(axis, 0, speed, speed / Acc, speed / Acc, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(axis, (int)position, 1);
                }

                Thread.Sleep(50);
                do
                {
                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                        goto Timeout;
                    if (Stop_sign[axis])
                        goto Stop;
                } while (AxisStates[state.Axis][4] != 1);
                AxisMachine[axis] = 4;
                AxisMoveModel[axis] = 0;
                lock (this)
                {
                    IMoveStateQueue.Remove(state);
                }
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{axis}轴相对阻塞定位完成，用时：{stopwatch.Elapsed}");
                return;
            Timeout:
                stopwatch.Stop();
                AxisStop(axis);
                Console.WriteLine(stopwatch.Elapsed);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞相对定位等待到位超时 （{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{axis}轴定位地址{position}，单轴阻塞相对定位等待到位超时 （{stopwatch.Elapsed}）");
                return;
            Stop:
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞相对定位外部异常停止！（{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{axis}轴定位地址{position}，单轴阻塞相对定位外部异常停止！（{stopwatch.Elapsed}）");
                return;

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
            if (IsOpenCard)
            {
                if (CardErrorMessage(CMCDLL_NET.MCF_Set_Servo_Enable_Net(axis, 0, card)))
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴上使能！");
                }
            }
        }

        public override void AxisOff()
        {
            if (IsOpenCard)
            {
                for (int i = 0; i < Axis.Length; i++)
                {
                    AxisMachine[i] = 0;
                    AxisMachine[i] = 0;
                    if (CardErrorMessage(CMCDLL_NET.MCF_Set_Servo_Enable_Net((ushort)i, 1, 0)))
                    {
                        CMCDLL_NET.MCF_Clear_Axis_State_Net((ushort)i);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{i}轴下使能！");
                    }
                }
            }
        }

        public override void AxisOn(ushort card, ushort axis)
        {
            if (IsOpenCard)
            {
                if (CardErrorMessage(CMCDLL_NET.MCF_Set_Servo_Enable_Net(axis, 1, card)))
                {
                    CMCDLL_NET.MCF_Clear_Axis_State_Net(axis);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴上使能！");
                }
            }
        }

        public override void AxisOn()
        {
            if (IsOpenCard)
            {
                for (int i = 0; i < 2; i++)
                {
                    CoordinateSystemStates[i] = 4;
                }
                for (int i = 0; i < Axis.Length; i++)
                {
                    if (CardErrorMessage(CMCDLL_NET.MCF_Set_Servo_Enable_Net((ushort)i, 0, 0)))
                    {
                        AxisMachine[i] = 4;
                        AxisMoveModel[i] = 0;
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{i}轴上使能！");
                    }
                }
            }
        }

        public override void AxisStop(ushort axis, int stop_mode = 0, bool all = false)
        {
            if (IsOpenCard)
            {
                if (!all)
                {

                    ushort a = (ushort)(stop_mode == 0 ? 1 : 0);
                    CMCDLL_NET.MCF_Axis_Stop_Net(axis, a);
                    CMCDLL_NET.MCF_Clear_Axis_State_Net(axis);
                    Stop_sign[axis] = true;
                }
                else
                {
                    for (ushort i = 0; i < 2; i++)
                    {
                        CoordinateSystemStates[i] = 4;
                        CMCDLL_NET.MCF_Coordinate_Stop_Net(i, 0);
                    }
                    for (ushort i = 0; i < Axis.Length; i++)
                    {
                        ushort a = (ushort)(stop_mode == 0 ? 1 : 0);
                        CMCDLL_NET.MCF_Axis_Stop_Net(i, a);
                        CMCDLL_NET.MCF_Clear_Axis_State_Net(i);
                        Stop_sign[i] = true;
                    }
                }
            }
        }

        public override void CloseCard()
        {
            AutoReadEvent.Reset();
        }

        /// <summary>
        /// 获取板卡全部数字输入
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        public override bool[] Getall_IOinput(ushort card)
        {
            uint a = 0;
            uint b = 0;
            CardErrorMessage(CMCDLL_NET.MCF_Get_Input_Net(ref a, ref b, card));
            int aLength = a != 0 ? Convert.ToString(a, 2).ToCharArray().Length : 0;
            int bLength = b != 0 ? Convert.ToString(b, 2).ToCharArray().Length : 0;
            bool[] ret = new bool[aLength + bLength];
            for (int i = 0; i < aLength + bLength; i++)
            {
                if (i < aLength)
                    ret[i] = (a & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                else
                    ret[i] = (b & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
            }
            return ret;
        }

        /// <summary>
        /// 获取板卡全部数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        /// 
        public override bool[] Getall_IOoutput(ushort card)
        {
            uint a = 0;
            CMCDLL_NET.MCF_Get_Output_Net(ref a, card);
            int aLength = a != 0 ? Convert.ToString(a, 2).ToCharArray().Length : 0;
            bool[] ret = new bool[aLength];
            for (int i = 0; i < aLength; i++)
            {
                ret[i] = (a & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
            }
            return ret;
        }

        /// <summary>
        /// 获取轴专用IO
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>
        /// <param> bool[0]==伺服报警(True=ON)</param>
        /// <param> bool[1]==正限位(True=ON)</param>
        /// <param> bool[2]==负限位(True=ON)</param>
        /// <param> bool[3]==急停(True=ON)</param>
        /// <param> bool[4]==原点(True=ON)</param>
        /// <param> bool[5]==正软限位(True=ON)</param>
        /// <param> bool[6]==负软限位(True=ON)</param>
        /// </returns>
        public override int[] GetAxisExternalio(ushort axis)
        {
            ushort[] ushorts = new ushort[7];
            int[] outint = new int[7];
            CardErrorMessage(CMCDLL_NET.MCF_Get_Servo_Alarm_Net(axis, ref ushorts[0]));
            CardErrorMessage(CMCDLL_NET.MCF_Get_Positive_Limit_Net(axis, ref ushorts[1]));
            CardErrorMessage(CMCDLL_NET.MCF_Get_Negative_Limit_Net(axis, ref ushorts[2]));
            CardErrorMessage(CMCDLL_NET.MCF_Get_Home_Net(axis, ref ushorts[4]));
            for (int i = 0; i < ushorts.Length; i++)
            {
                outint[i] = ushorts[i] == 1 ? 0 : 1;
            }
            return outint;
        }

        /// <summary>
        /// 获取轴状态信息
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns> 返回值double[6] 数组
        ///double[0]= 脉冲位置
        ///double[1]= 伺服编码器位置
        ///double[2]= 速度
        ///double[3]= 目标位置
        ///double[4]= 轴运动到位 0=运动中 1=轴停止
        ///double[5]= 轴状态机0：轴处于未启动状态 1：轴处于启动禁止状态 2：轴处于准备启动状态 3：轴处于启动状态 4：轴处于操作使能状态 5：轴处于停止状态 6：轴处于错误触发状态 7：轴处于错误状态
        ///double[6]= 轴运行模式0：空闲 1：Pmove 2：Vmove 3：Hmove 4：Handwheel 5：Ptt / Pts 6：Pvt / Pvts 10：Continue
        ///double[7]= 轴停止原因获取0：正常停止  3：LTC 外部触发立即停止  4：EMG 立即停止  5：正硬限位立即停止  6：负硬限位立即停止  7：正硬限位减速停止  8：负硬限位减速停止  9：正软限位立即停止  
        ///10：负软限位立即停止11：正软限位减速停止  12：负软限位减速停止  13：命令立即停止  14：命令减速停止  15：其它原因立即停止  16：其它原因减速停止  17：未知原因立即停止  18：未知原因减速停止
        /// </returns>
        public override double[] GetAxisState(ushort axis)
        {
            //CMCDLL_NET.MCF_Clear_Axis_State_Net(axis);
            double[] doubles = new double[8];
            double[] vel = new double[2];
            int Encoderpos = 0;
            int pos = 0;
            ushort indog = 0;
            short stop = 0;
            CMCDLL_NET.MCF_Get_Position_Net(axis, ref pos);//脉冲位置
            CMCDLL_NET.MCF_Get_Encoder_Net(axis, ref Encoderpos);//编码器位置
            var t = CMCDLL_NET.MCF_Get_Vel_Net(axis, ref vel[0], ref vel[1]);//速度读取
            CMCDLL_NET.MCF_Get_Servo_INP_Net(axis, ref indog);//运动到位
            CMCDLL_NET.MCF_Get_Axis_State_Net(axis, ref stop);//停止原因
            //doubles[4] = indog == 1 ? 0 : 1;
            switch (stop)
            {
                case 0: doubles[4] = 1; doubles[7] = 0; break;
                case 1: doubles[4] = 0; break;
                case 2: doubles[7] = 4; break;
                case 14: doubles[7] = 5; break;
                case 16: doubles[7] = 6; break;
                case 15: doubles[7] = 7; break;
                case 17: doubles[7] = 8; break;
                case 18: doubles[7] = 9; break;
                case 20: doubles[7] = 10; break;
                case 19: doubles[7] = 11; break;
                case 21: doubles[7] = 12; break;
                case 22: doubles[7] = 13; break;
                case 23: doubles[7] = 14; break;
                default:
                    doubles[7] = 15;
                    break;
            }
            Console.WriteLine(pos);
            doubles[0] = Convert.ToDouble(pos);
            doubles[1] = Convert.ToDouble(Encoderpos);
            doubles[2] = TargetLocation[axis];
            doubles[3] = 0;
            doubles[5] = AxisMachine[axis];
            doubles[6] = AxisMoveModel[axis];
            //doubles[4] = indog == 1 ? 0 : 1;
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
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，单轴绝对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，单轴绝对定位启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，单轴绝对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，单轴绝对定位启动失败！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴绝对定位启动错误！ {axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                MoveState state;
                TargetLocation[axis] = position;
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 1;
                state = new MoveState()
                {
                    Axis = axis,
                    CurrentPosition = AxisStates[axis][0],
                    Speed = speed,
                    Position = position,
                    Movetype = 1,
                    OutTime = time,
                    ACC = Acc,
                    Dcc = Dec,
                    Handle = DateTime.Now,
                };
                lock (Motion_Lok[axis])
                {
                    if (IMoveStateQueue.Exists(e => e.Axis == axis))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                        IMoveStateQueue.Remove(colose);
                    }
                    IMoveStateQueue.Add(state);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴绝对定位启动，速度{speed}pulse/S，目标位置{position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(axis, 0, speed, speed / Acc, speed / Acc, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(axis, (int)position, 0);
                }
                Thread.Sleep(50);
                Task.Run(() =>
                {
                    do
                    {
                        if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                            goto Timeout;
                        if (Stop_sign[axis])
                            goto Stop;
                    } while (AxisStates[state.Axis][4] != 1);
                    AxisMachine[axis] = 4;
                    AxisMoveModel[axis] = 0;
                    lock (this)
                    {
                        IMoveStateQueue.Remove(state);
                    }
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴绝对定位完成，用时：{stopwatch.Elapsed}");
                    return;
                Timeout:
                    stopwatch.Stop();
                    AxisStop(axis);
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                    return;
                Stop:
                    AxisStop(state.Axis, 1);
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                    return;
                });
            }
        }

        public override void MoveCircle_Center(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        public override void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，原点回归启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，原点回归启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，原点回归启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，原点回归启动失败！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}轴原点回归启动错误！ {axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                MoveState state;
                TargetLocation[axis] = 0;
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 3;
                state = new MoveState()
                {
                    Axis = axis,
                    CurrentPosition = AxisStates[axis][0],
                    Speed = home_speed,
                    ACC = acc,
                    Dcc = dcc,
                    Position = 0,
                    Movetype = 5,
                    HomeModel = home_model,
                    Home_off = offpos,
                    OutTime = timeout,
                    Handle = DateTime.Now,
                };
                lock (Motion_Lok[axis])
                {
                    if (IMoveStateQueue.Exists(e => e.Axis == axis))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                        IMoveStateQueue.Remove(colose);
                    }
                    IMoveStateQueue.Add(state);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴原点回归启动，速度{home_speed}pulse/S，原点回归模式{home_model}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Search_Home_Stop_Time_Net(axis, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Set_Net(axis, home_model, 0, 0, 0, home_speed, home_speed, 0, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Start_Net(axis, 0);
                }
                Thread.Sleep(50);
                Task.Run(() =>
                {
                    ushort Home_State = 0;
                    do
                    {
                        if (timeout != 0 && stopwatch.Elapsed.TotalMilliseconds > timeout)
                            goto Timeout;
                        if (Stop_sign[axis])
                            goto Stop;
                        Thread.Sleep(50);
                        CMCDLL_NET.MCF_Search_Home_Get_State_Net(axis, ref Home_State, 0);
                    } while (Home_State == 32);
                    stopwatch.Stop();
                    if (Home_State == 0)
                    {
                        lock (this)
                        {
                            IMoveStateQueue.Remove(state);
                        }
                        AxisMachine[axis] = 4;
                        AxisMoveModel[axis] = 0;
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{axis}轴原点回归完成，用时：{stopwatch.Elapsed}");
                    }
                    else if (Home_State == 31)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{axis}轴原点回归错误，用时：{stopwatch.Elapsed}");
                    }
                    return;
                Timeout:
                    stopwatch.Stop();
                    AxisStop(axis);
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归等待到位超时 （{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{axis}轴原点回归等待到位超时（{stopwatch.Elapsed}）");
                    return;
                Stop:
                    AxisStop(state.Axis, 1);
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                    return;
                });
            }
        }

        public override void MoveJog(ushort axis, double speed, int posi_mode, double acc = 0.5, double dec = 0.5)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，JOG启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，JOG启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，JOG启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，JOG启动失败！");
                    return;
                }
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{axis}轴JOG运动启动，速度{speed}pulse/S");
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 2;
                if (posi_mode == 0)
                    CMCDLL_NET.MCF_JOG_Net(axis, -speed, Math.Abs(speed / acc));
                else
                    CMCDLL_NET.MCF_JOG_Net(axis, speed, Math.Abs(speed / acc));
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"未初始化板卡！");
                else
                    throw new Exception($"未初始化板卡！");
            }
        }

        public override void MoveLines(ushort card, ControlState t, int time = 0)
        {
            if (IsOpenCard)
            {
                ControlState control = t;
                foreach (var item in t.Axis)
                {
                    if (Axis_IO[item][0] == 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴驱动器报错，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴驱动器报错，单轴相对定位启动失败！");
                        return;
                    }
                    if (AxisStates[item][4] == 0 && AxisStates[item][7] == 0)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴运动中，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴运动中，单轴相对定位启动失败！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        else
                            throw new Exception($"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        return;
                    }
                    if (IMoveStateQueue.Exists(e => e.Axis == item))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == item);
                        lock (this)
                        { IMoveStateQueue.Remove(colose); }
                    }
                    if (IMoveStateQueue.Exists(e => e.Axises.Contains(item)))
                    {
                        var colose = IMoveStateQueue.FirstOrDefault(x => x.Axises.Contains(item));
                        lock (this)
                        { IMoveStateQueue.Remove(colose); }
                    }
                }
                MoveState state = new MoveState()
                {
                    CardID = card,
                    ACC = control.Acc,
                    Dcc = control.Dcc,
                    Speed = control.Speed,
                    UsingAxisNumber = control.UsingAxisNumber,
                    Axises = control.Axis,
                    OutTime = time,
                    Positions = control.Position
                };
                state.CurrentPositions = new double[state.UsingAxisNumber];
                int type = 0;
                ushort[] axis = new ushort[control.UsingAxisNumber];
                int[] pos = new int[control.UsingAxisNumber];
                axis = control.Axis;
                for (int i = 0; i < state.Axises.Length; i++)
                {
                    state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                }
                if (control.locationModel == 0)//相对
                {
                    for (int i = 0; i < control.Position.Length; i++)
                    {
                        TargetLocation[axis[i]] = state.Positions[i] + state.CurrentPositions[i];
                        AxisMachine[axis[i]] = 3;
                        AxisMoveModel[axis[i]] = 10;
                        pos[i] = Convert.ToInt32(control.Position[i]);
                    }

                    type = 1;
                    state.Movetype = 7;//相对
                }
                else if (control.locationModel == 1)//绝对
                {
                    for (int i = 0; i < control.Position.Length; i++)
                    {
                        TargetLocation[axis[i]] = state.Positions[i];
                        AxisMachine[axis[i]] = 3;
                        AxisMoveModel[axis[i]] = 10;
                        pos[i] = Convert.ToInt32(control.Position[i]);
                    }
                    state.Movetype = 8;
                }
                lock (this)
                {
                    IMoveStateQueue.Add(state);
                }
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
                CoordinateSystemStates[coordinate] = 0;
                if (coordinate != -1)
                {
                    if (control.UsingAxisNumber == control.Axis.Length && control.UsingAxisNumber == control.Position.Length)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            lock (Motion_Lok[control.Axis[0]])
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴直线插补启动！");

                                stopwatch.Restart();
                                CMCDLL_NET.MCF_Set_Coordinate_Profile_Net((ushort)coordinate, 0, control.Speed, control.Speed / control.Acc, (control.Speed / control.Acc) * 2, 0, 0);
                                switch (control.UsingAxisNumber)
                                {
                                    case 2:
                                        CMCDLL_NET.MCF_Line2_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 3:
                                        CMCDLL_NET.MCF_Line3_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 4:
                                        CMCDLL_NET.MCF_Line4_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    default:
                                        break;
                                }
                                Thread.Sleep(50);
                                Task.Run(() =>
                                {
                                    do
                                    {
                                        if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                            goto Timeout;
                                        if (Stop_sign[state.Axises[0]])
                                            goto Stop;
                                    } while (AxisStates[state.Axises[0]][4] != 1);
                                    lock (this)
                                    {
                                        IMoveStateQueue.Remove(state);
                                    }
                                    for (int i = 0; i < control.Position.Length; i++)
                                    {
                                        AxisMachine[axis[i]] = 3;
                                        AxisMoveModel[axis[i]] = 0;
                                    }
                                    CoordinateSystemStates[coordinate] = 4;
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴直线插补完成，用时：{stopwatch.Elapsed}");
                                    return;
                                Timeout:
                                    AxisStop(state.Axis, 1);
                                    stopwatch.Stop();
                                    Console.WriteLine(stopwatch.Elapsed);
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                    else
                                        throw new Exception($"{control.UsingAxisNumber} 轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                    return;
                                Stop:
                                    AxisStop(state.Axis, 1);
                                    stopwatch.Stop();
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                    else
                                        throw new Exception($"{control.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                    return;
                                });

                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();

                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        return;
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"插补坐标系无空闲！");
                    else
                        throw new Exception($"插补坐标系无空闲！");
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

        public override void MoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴驱动器报错，单轴相对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴驱动器报错，单轴相对定位启动失败！");
                    return;
                }
                if (AxisStates[axis][4] == 0 && AxisStates[axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴运动中，单轴相对定位启动失败！");
                    else
                        throw new Exception($"{axis}轴运动中，单轴相对定位启动失败！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误， {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误， {axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                MoveState state;
                TargetLocation[axis] = AxisStates[axis][0] + position;
                AxisMachine[axis] = 3;
                AxisMoveModel[axis] = 1;
                state = new MoveState()
                {
                    Axis = axis,
                    CurrentPosition = AxisStates[axis][0],
                    Speed = speed,
                    Position = position,
                    Movetype = 2,
                    ACC = Acc,
                    Dcc = Dec,
                    OutTime = time,
                    Handle = DateTime.Now,
                };
                lock (Motion_Lok[axis])
                {
                    if (IMoveStateQueue.Exists(e => e.Axis == axis))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                        IMoveStateQueue.Remove(colose);
                    }
                    IMoveStateQueue.Add(state);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴相对定位启动，速度{speed}pulse/S，目标位置{state.Position}！");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(axis, 0, speed, speed / Acc, speed / Acc, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(axis, (int)position, 1);
                }
                Thread.Sleep(50);
                Task.Run(() =>
                {
                    do
                    {
                        if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                            goto Timeout;
                        if (Stop_sign[axis])
                            goto Stop;
                    } while (AxisStates[state.Axis][4] != 1);
                    AxisMachine[axis] = 4;
                    AxisMoveModel[axis] = 0;
                    lock (this)
                    {
                        IMoveStateQueue.Remove(state);
                    }
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴相对定位完成，用时{stopwatch.Elapsed}");
                    return;
                Timeout:
                    stopwatch.Stop();
                    AxisStop(axis);
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{axis}轴定位地址{position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                    return;
                Stop:
                    AxisStop(state.Axis, 1);
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴相对定位外部异常停止！（{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{axis}轴定位地址{position}，单轴相对定位外部异常停止！（{stopwatch.Elapsed}）");
                    return;
                });
            }
        }

        public override void MoveReset(ushort axis)
        {
            if (IsOpenCard)
            {
                if (!Stop_sign[axis])
                {
                    var list = IMoveStateQueue.ToArray();
                    foreach (var item in list)
                    {
                        if (item.Axis == axis)
                        {
                            switch (item.Movetype)
                            {
                                case 1: MoveAbs(item); break;
                                case 2: MoveRel(item); break;
                                case 3: AwaitMoveAbs(item); break;
                                case 4: AwaitMoveRel(item); break;
                                case 5: MoveHome(item); break;
                                case 6: AwaitMoveHome(item); break;
                                case 7: MoveLines(item); break;
                                case 8: MoveLines(item); break;
                                case 9: AwaitMoveLines(item); break;
                                case 10: AwaitMoveLines(item); break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}轴处于停止状态中,请复位轴状态！");
                    else
                        throw new Exception($"{axis}轴处于停止状态中,请复位轴状态！");
                    return;
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        private void MoveLines(MoveState linesitem)
        {
            if (IsOpenCard)
            {
                MoveState state = linesitem;
                foreach (var item in state.Axises)
                {
                    if (Axis_IO[item][0] == 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴驱动器报错，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴驱动器报错，单轴相对定位启动失败！");
                        return;
                    }
                    if (AxisStates[item][4] == 0 && AxisStates[item][7] == 0)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴运动中，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴运动中，单轴相对定位启动失败！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        else
                            throw new Exception($"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        return;
                    }
                }

                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
                CoordinateSystemStates[coordinate] = 0;
                if (coordinate != -1)
                {
                    if (state.UsingAxisNumber == state.Axises.Length && state.UsingAxisNumber == state.Positions.Length)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            ushort[] axis = new ushort[state.UsingAxisNumber];
                            int[] pos = new int[state.UsingAxisNumber];
                            axis = state.Axises;
                            int type = 0;
                            if (state.Movetype == 7)//相对
                            {
                                type = 1;
                                for (int i = 0; i < state.Positions.Length; i++)
                                {
                                    TargetLocation[axis[i]] = state.Positions[i] + state.CurrentPositions[i];
                                    AxisMachine[axis[i]] = 3;
                                    AxisMoveModel[axis[i]] = 10;
                                    int site = Convert.ToInt32((state.Positions[i] + state.CurrentPositions[i]) - AxisStates[state.Axises[i]][0]);
                                    pos[i] = Convert.ToInt32(site);
                                }
                            }

                            else if (state.Movetype == 8)//绝对
                            {
                                type = 0;
                                for (int i = 0; i < state.Positions.Length; i++)
                                {
                                    TargetLocation[axis[i]] = state.Positions[i];
                                    AxisMachine[axis[i]] = 3;
                                    AxisMoveModel[axis[i]] = 10;
                                    int site = Convert.ToInt32((state.Positions[i]));
                                    pos[i] = Convert.ToInt32(site);
                                }
                            }

                            lock (Motion_Lok[state.Axises[0]])
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补启动！");
                                stopwatch.Restart();
                                CMCDLL_NET.MCF_Set_Coordinate_Profile_Net((ushort)coordinate, state.Speed / state.ACC, state.Speed, state.Speed / state.ACC, state.Speed / state.ACC, 1, 0);
                                switch (state.UsingAxisNumber)
                                {
                                    case 2:
                                        CMCDLL_NET.MCF_Line2_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 3:
                                        CMCDLL_NET.MCF_Line3_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 4:
                                        CMCDLL_NET.MCF_Line4_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    default:
                                        break;
                                }
                                Thread.Sleep(50);
                                Task.Run(() =>
                                {
                                    do
                                    {
                                        if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                            goto Timeout;
                                        if (Stop_sign[state.Axises[0]])
                                            goto Stop;
                                    } while (AxisStates[state.Axises[0]][4] != 1);

                                    lock (this)
                                    {
                                        IMoveStateQueue.Remove(state);
                                    }
                                    for (int i = 0; i < state.Positions.Length; i++)
                                    {
                                        AxisMachine[axis[i]] = 3;
                                        AxisMoveModel[axis[i]] = 0;
                                    }
                                    CoordinateSystemStates[coordinate] = 4;
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补完成，用时：{stopwatch.Elapsed}");
                                    return;
                                Timeout:
                                    stopwatch.Stop();
                                    for (int i = 0; i < state.Axises.Length; i++)
                                    {
                                        AxisStop(state.Axises[i]);
                                    }
                                    CoordinateSystemStates[coordinate] = 4;
                                    Console.WriteLine(stopwatch.Elapsed);
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                    else
                                        throw new Exception($"{state.UsingAxisNumber} 轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                    CoordinateSystemStates[coordinate] = 4;
                                    return;
                                Stop:
                                    AxisStop(state.Axis, 1);
                                    stopwatch.Stop();
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                    else
                                        throw new Exception($"{state.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                    CoordinateSystemStates[coordinate] = 4;
                                    return;
                                });
                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        return;
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"插补坐标系无空闲！");
                    else
                        throw new Exception($"插补坐标系无空闲！");
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

        private void AwaitMoveHome(MoveState item)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[item.Axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴驱动器报错，原点回归启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴驱动器报错，原点回归启动失败！");
                    return;
                }
                if (AxisStates[item.Axis][4] == 0 && AxisStates[item.Axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴运动中，原点回归启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴运动中，原点回归启动失败！");
                    return;
                }
                if (Stop_sign[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴阻塞原点回归启动错误！ {item.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{item.Axis}轴阻塞原点回归启动错误！ {item.Axis}轴处于停止中！");
                    return;
                }
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴阻塞原点回归启动，速度{item.Speed}pulse/S，原点回归模式{item.HomeModel}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Search_Home_Stop_Time_Net(item.Axis, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Set_Net(item.Axis, item.HomeModel, 0, 0, 0, item.Speed, item.Speed / 2, 0, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Start_Net(item.Axis, 0);
                    Thread.Sleep(50);

                    ushort Home_State = 0;
                    do
                    {
                        if (item.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > item.OutTime)
                            goto Timeout;
                        if (Stop_sign[item.Axis])
                            goto Stop;
                        Thread.Sleep(50);
                        CMCDLL_NET.MCF_Search_Home_Get_State_Net(item.Axis, ref Home_State, 0);
                    } while (Home_State == 32);
                    stopwatch.Stop();
                    if (Home_State == 0)
                    {
                        AxisMachine[item.Axis] = 4;
                        AxisMoveModel[item.Axis] = 0;
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{item.Axis}轴阻塞原点回归完成，用时：{stopwatch.Elapsed}");
                        lock (this)
                        {
                            IMoveStateQueue.Remove(item);
                        }
                    }
                    else if (Home_State == 31)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{item.Axis}轴阻塞原点回归错误，用时：{stopwatch.Elapsed}");
                    }
                    return;
                Timeout:
                    stopwatch.Stop();
                    AxisStop(item.Axis);
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴阻塞原点回归等待到位超时 （{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{item.Axis}轴阻塞原点回归等待到位超时（{stopwatch.Elapsed}）");
                    return;
                Stop:
                    AxisStop(item.Axis, 1);
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴阻塞原点回归外部异常停止！（{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{item.Axis}轴阻塞原点回归外部异常停止！（{stopwatch.Elapsed}）");
                    return;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void MoveHome(MoveState item)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[item.Axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴驱动器报错，原点回归启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴驱动器报错，原点回归启动失败！");
                    return;
                }
                if (AxisStates[item.Axis][4] == 0 && AxisStates[item.Axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴运动中，原点回归启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴运动中，原点回归启动失败！");
                    return;
                }
                if (Stop_sign[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴原点回归启动错误！ {item.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{item.Axis}轴原点回归启动错误！ {item.Axis}轴处于停止中！");
                    return;
                }
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴原点回归启动，速度{item.Speed}pulse/S，原点回归模式{item.HomeModel}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Search_Home_Stop_Time_Net(item.Axis, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Set_Net(item.Axis, item.HomeModel, 0, 0, 0, item.Speed, item.Speed / 2, 0, 0, 0);
                    CMCDLL_NET.MCF_Search_Home_Start_Net(item.Axis, 0);
                    Thread.Sleep(50);
                    Task.Run(() =>
                    {
                        ushort Home_State = 0;
                        do
                        {
                            if (item.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > item.OutTime)
                                goto Timeout;
                            if (Stop_sign[item.Axis])
                                goto Stop;
                            Thread.Sleep(50);
                            CMCDLL_NET.MCF_Search_Home_Get_State_Net(item.Axis, ref Home_State, 0);
                        } while (Home_State == 32);
                        stopwatch.Stop();
                        if (Home_State == 0)
                        {
                            AxisMachine[item.Axis] = 4;
                            AxisMoveModel[item.Axis] = 0;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{item.Axis}轴原点回归完成，用时：{stopwatch.Elapsed}");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(item);
                            }
                        }
                        else if (Home_State == 31)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{item.Axis}轴原点回归错误，用时：{stopwatch.Elapsed}");
                        }
                        return;
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(item.Axis);
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item.Axis}轴原点回归等待到位超时 （{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{item.Axis}轴原点回归等待到位超时（{stopwatch.Elapsed}）");
                        return;
                    Stop:
                        AxisStop(item.Axis, 1);
                        stopwatch.Stop();
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item.Axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{item.Axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                        return;
                    });
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void AwaitMoveRel(MoveState item)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[item.Axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴驱动器报错，单轴阻塞相对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴驱动器报错，单轴阻塞相对定位启动失败！");
                    return;
                }
                if (AxisStates[item.Axis][4] == 0 && AxisStates[item.Axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴运动中，单轴阻塞相对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴运动中，单轴阻塞相对定位启动失败！");
                    return;
                }
                if (Stop_sign[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}单轴阻塞相对定位启动错误， {item.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{item.Axis}单轴阻塞相对定位启动错误， {item.Axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                int site = Convert.ToInt32((item.CurrentPosition + item.Position) - AxisStates[item.Axis][0]);
                TargetLocation[item.Axis] = item.CurrentPosition + item.Position;
                AxisMachine[item.Axis] = 3;
                AxisMoveModel[item.Axis] = 1;
                lock (Motion_Lok[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴相对阻塞定位启动，速度{item.Speed}pulse/S，目标位置{item.CurrentPosition + item.Position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(item.Axis, 0, item.Speed, item.Speed / Acc, dJerk: item.Speed / Acc, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(item.Axis, site, 1);
                }

                Thread.Sleep(50);
                do
                {
                    if (item.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > item.OutTime)
                        goto Timeout;
                    if (Stop_sign[item.Axis])
                        goto Stop;
                } while (AxisStates[item.Axis][4] != 1);
                AxisMachine[item.Axis] = 4;
                AxisMoveModel[item.Axis] = 0;
                lock (this)
                {
                    IMoveStateQueue.Remove(item);
                }
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{item.Axis}轴相对阻塞定位完成，用时：{stopwatch.Elapsed}");
                return;
            Timeout:
                stopwatch.Stop();
                AxisStop(item.Axis);
                Console.WriteLine(stopwatch.Elapsed);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴阻塞相对定位等待到位超时 （{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴阻塞相对定位等待到位超时 （{stopwatch.Elapsed}）");
                return;
            Stop:
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴阻塞相对定位外部异常停止！（{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴阻塞相对定位外部异常停止！（{stopwatch.Elapsed}）");
                return;

            }
        }

        private void AwaitMoveAbs(MoveState item)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[item.Axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴驱动器报错，单轴阻塞绝对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴驱动器报错，单轴阻塞绝对定位启动失败！");
                    return;
                }
                if (AxisStates[item.Axis][4] == 0 && AxisStates[item.Axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴运动中，单轴阻塞绝对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴运动中，单轴阻塞绝对定位启动失败！");
                    return;
                }
                if (Stop_sign[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}单轴阻塞绝对定位启动错误！ {item.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{item.Axis}单轴阻塞绝对定位启动错误！ {item.Axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                TargetLocation[item.Axis] = item.Position;
                AxisMachine[item.Axis] = 3;
                AxisMoveModel[item.Axis] = 1;
                lock (Motion_Lok[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴阻塞绝对定位启动，速度{item.Speed}pulse/S，目标位置{item.Position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(item.Axis, 0, item.Speed, item.Speed / item.ACC, item.Speed / item.ACC, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(item.Axis, (int)item.Position, 0);
                }
                Thread.Sleep(50);
                do
                {
                    if (item.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > item.OutTime)
                        goto Timeout;
                    if (Stop_sign[item.Axis])
                        goto Stop;
                } while (AxisStates[item.Axis][4] != 1);
                AxisMachine[item.Axis] = 4;
                AxisMoveModel[item.Axis] = 0;
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{item.Axis}轴阻塞绝对定位完成，用时：{stopwatch.Elapsed}");
                lock (this)
                {
                    IMoveStateQueue.Remove(item);
                }
                return;
            Timeout:
                stopwatch.Stop();
                AxisStop(item.Axis);
                Console.WriteLine(stopwatch.Elapsed);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴阻塞绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴阻塞绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                return;
            Stop:
                AxisStop(item.Axis, 1);
                stopwatch.Stop();
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                else
                    throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                return;
            }
        }

        private void MoveRel(MoveState item)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[item.Axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴驱动器报错，单轴相对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴驱动器报错，单轴相对定位启动失败！");
                    return;
                }
                if (AxisStates[item.Axis][4] == 0 && AxisStates[item.Axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴运动中，单轴相对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴运动中，单轴相对定位启动失败！");
                    return;
                }
                if (Stop_sign[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}单轴相对定位启动错误， {item.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{item.Axis}单轴相对定位启动错误， {item.Axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                int site = Convert.ToInt32((item.CurrentPosition + item.Position) - AxisStates[item.Axis][0]);
                TargetLocation[item.Axis] = item.CurrentPosition + item.Position;
                AxisMachine[item.Axis] = 3;
                AxisMoveModel[item.Axis] = 1;
                lock (Motion_Lok[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴相对定位启动，速度{item.Speed}pulse/S，目标位置{item.Position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(item.Axis, 0, item.Speed, item.Speed / Acc, dJerk: item.Speed / Acc, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(item.Axis, site, 1);
                }
                Thread.Sleep(50);
                Task.Run(() =>
                {
                    do
                    {
                        if (item.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > item.OutTime)
                            goto Timeout;
                        if (Stop_sign[item.Axis])
                            goto Stop;
                    } while (AxisStates[item.Axis][4] != 1);
                    AxisMachine[item.Axis] = 4;
                    AxisMoveModel[item.Axis] = 0;
                    lock (this)
                    {
                        IMoveStateQueue.Remove(item);
                    }
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴相对定位完成，用时：{stopwatch.Elapsed}");
                    return;
                Timeout:
                    stopwatch.Stop();
                    AxisStop(item.Axis);
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                    return;
                Stop:
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴相对定位外部异常停止！（{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴相对定位外部异常停止！（{stopwatch.Elapsed}）");
                    return;
                });
            }
        }

        private void MoveAbs(MoveState item)
        {
            if (IsOpenCard)
            {
                if (Axis_IO[item.Axis][0] == 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴驱动器报错，单轴绝对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴驱动器报错，单轴绝对定位启动失败！");
                    return;
                }
                if (AxisStates[item.Axis][4] == 0 && AxisStates[item.Axis][7] == 0)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴运动中，单轴绝对定位启动失败！");
                    else
                        throw new Exception($"{item.Axis}轴运动中，单轴绝对定位启动失败！");
                    return;
                }
                if (Stop_sign[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}单轴绝对定位启动错误！ {item.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{item.Axis}单轴绝对定位启动错误！ {item.Axis}轴处于停止中！");
                    return;
                }
                Stopwatch stopwatch = new Stopwatch();
                TargetLocation[item.Axis] = item.Position;
                AxisMachine[item.Axis] = 3;
                AxisMoveModel[item.Axis] = 1;
                lock (Motion_Lok[item.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴绝对定位启动，速度{item.Speed}pulse/S，目标位置{item.Position}");
                    stopwatch.Restart();
                    CMCDLL_NET.MCF_Set_Axis_Profile_Net(item.Axis, 0, item.Speed, item.Speed / item.ACC, item.Speed / item.ACC, 0, 0);
                    CMCDLL_NET.MCF_Uniaxial_Net(item.Axis, (int)item.Position, 0);
                }
                Thread.Sleep(50);
                Task.Run(() =>
                {
                    do
                    {
                        if (item.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > item.OutTime)
                            goto Timeout;
                        if (Stop_sign[item.Axis])
                            goto Stop;
                    } while (AxisStates[item.Axis][4] != 1);
                    AxisMachine[item.Axis] = 4;
                    AxisMoveModel[item.Axis] = 0;
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{item.Axis}轴绝对定位完成，用时：{stopwatch.Elapsed}");
                    lock (this)
                    {
                        IMoveStateQueue.Remove(item);
                    }
                    return;
                Timeout:
                    stopwatch.Stop();
                    AxisStop(item.Axis);
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                    return;
                Stop:
                    AxisStop(item.Axis, 1);
                    stopwatch.Stop();
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{item.Axis}轴定位地址{item.Position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                    else
                        throw new Exception($"{item.Axis}轴定位地址{item.Position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                    return;
                });
            }
        }

        private void AwaitMoveLines(MoveState linesitem)
        {
            if (IsOpenCard)
            {
                MoveState state = linesitem;
                foreach (var item in state.Axises)
                {
                    if (Axis_IO[item][0] == 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴驱动器报错，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴驱动器报错，单轴相对定位启动失败！");
                        return;
                    }
                    if (AxisStates[item][4] == 0 && AxisStates[item][7] == 0)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴运动中，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴运动中，单轴相对定位启动失败！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        else
                            throw new Exception($"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        return;
                    }
                }

                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
                CoordinateSystemStates[coordinate] = 0;
                if (coordinate != -1)
                {
                    if (state.UsingAxisNumber == state.Axises.Length && state.UsingAxisNumber == state.Positions.Length)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            ushort[] axis = new ushort[state.UsingAxisNumber];
                            int[] pos = new int[state.UsingAxisNumber];
                            axis = state.Axises;
                            int type = 0;
                            if (state.Movetype == 9)//相对
                            {
                                type = 1;
                                for (int i = 0; i < state.Positions.Length; i++)
                                {
                                    TargetLocation[axis[i]] = state.Positions[i] + state.CurrentPositions[i];
                                    AxisMachine[axis[i]] = 3;
                                    AxisMoveModel[axis[i]] = 10;
                                    int site = Convert.ToInt32((state.Positions[i] + state.CurrentPositions[i]) - AxisStates[state.Axises[i]][0]);
                                    pos[i] = Convert.ToInt32(site);
                                }
                            }

                            else if (state.Movetype == 10)//绝对
                            {
                                type = 0;
                                for (int i = 0; i < state.Positions.Length; i++)
                                {
                                    TargetLocation[axis[i]] = state.Positions[i];
                                    AxisMachine[axis[i]] = 3;
                                    AxisMoveModel[axis[i]] = 10;
                                    int site = Convert.ToInt32((state.Positions[i]));
                                    pos[i] = Convert.ToInt32(site);
                                }
                            }
                            lock (Motion_Lok[state.Axises[0]])
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补启动！");
                                stopwatch.Restart();
                                CMCDLL_NET.MCF_Set_Coordinate_Profile_Net((ushort)coordinate, state.Speed / state.ACC, state.Speed, state.Speed / state.ACC, state.Speed / state.ACC, 1, 0);
                                switch (state.UsingAxisNumber)
                                {
                                    case 2:
                                        CMCDLL_NET.MCF_Line2_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 3:
                                        CMCDLL_NET.MCF_Line3_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 4:
                                        CMCDLL_NET.MCF_Line4_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    default:
                                        break;
                                }
                                Thread.Sleep(50);
                                do
                                {
                                    if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                        goto Timeout;
                                    if (Stop_sign[state.Axises[0]])
                                        goto Stop;
                                } while (AxisStates[state.Axises[0]][4] != 1);

                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                for (int i = 0; i < state.Positions.Length; i++)
                                {
                                    AxisMachine[axis[i]] = 3;
                                    AxisMoveModel[axis[i]] = 0;
                                }
                                CoordinateSystemStates[coordinate] = 4;
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补完成，用时：{stopwatch.Elapsed}");
                                return;
                            Timeout:
                                stopwatch.Stop();
                                for (int i = 0; i < state.Axises.Length; i++)
                                {
                                    AxisStop(state.Axises[i]);
                                }
                                Console.WriteLine(stopwatch.Elapsed);
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{state.UsingAxisNumber} 轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                CoordinateSystemStates[coordinate] = 4;
                                return;
                            Stop:
                                AxisStop(state.Axis, 1);
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{state.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                CoordinateSystemStates[coordinate] = 4;
                                return;
                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();

                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        return;
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"插补坐标系无空闲！");
                    else
                        throw new Exception($"插补坐标系无空闲！");
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

        public override bool OpenCard(ushort card_number)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打开板卡
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">板卡型号数组参数异常</exception>
        public override bool OpenCard()
        {
            bool isopen = false;
            if (Card_Number != null && Axisquantity != -1)
            {

                Axis = new ushort[Axisquantity];
                TargetLocation = new double[Axisquantity];
                AxisMachine = new int[Axisquantity];
                AxisMoveModel = new int[Axisquantity];
                ushort[] Station_Number = { 0, 1, 2, 3, 4, 5, 6, 7 };
                var refs = CMCDLL_NET.MCF_Open_Net((ushort)Card_Number.Length, ref Station_Number[0], ref Card_Number[0]);
                if (refs != 0)
                {
                    IsOpenCard = false;
                    isopen = false;
                    CardErrorMessage(refs);
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        CoordinateSystemStates[i] = 4;
                    }
                    Task_Token = new CancellationTokenSource[Axisquantity];
                    cancellation_Token = new CancellationToken[Axisquantity];
                    Stop_sign = new bool[Axisquantity];
                    Motion_Lok = new object[Axisquantity];
                    for (int i = 0; i < Axis.Length; i++)
                    {
                        Task_Token[i] = new CancellationTokenSource();
                        cancellation_Token[i] = Task_Token[i].Token;
                        Stop_sign[i] = false;
                        Motion_Lok[i] = new object();
                    }
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, "打开板卡成功!");
                    isopen = true;
                    if (Read_ThreadPool[0].ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        Read_ThreadPool[0].IsBackground = true;
                        Read_ThreadPool[0].Start();
                    }
                    Thread.Sleep(100);
                    IsOpenCard = true;
                }
            }
            else
            {
                if (Card_Number == null)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, "未设置板卡型号！在OpenCard前请先在Card_Number数组中设置板卡型号");
                    throw new Exception("未设置板卡型号！在OpenCard前请先在Card_Number数组中设置板卡型号");
                }
                else if (Axis == null)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, "未设置轴数量！在OpenCard前请先在Axis数组中设置轴数量");
                    throw new Exception("未设置轴数量！在OpenCard前请先在Axis数组中设置轴数量");
                }
            }
            return isopen;
        }

        private void Read()
        {
            AxisStates = new double[Axis.Length][];
            Axis_IO = new int[Axis.Length][];
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                Thread.Sleep(30);
                List<bool> inputList = new List<bool>();
                List<bool> outputList = new List<bool>();
                stopwatch.Restart();
                //EtherCATStates = GetEtherCATState(0);
                for (ushort i = 0; i < Axis.Length; i++)
                {
                    AxisStates[i] = GetAxisState(i);
                    Axis_IO[i] = GetAxisExternalio(i);
                }
                for (ushort i = 0; i < Card_Number.Length; i++)
                {
                    inputList.AddRange(Getall_IOinput(i));
                    outputList.AddRange(Getall_IOoutput(i));
                }
                IO_Input = inputList.ToArray();
                IO_Output = outputList.ToArray();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        public override void ResetCard(ushort card, ushort reset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置板卡轴配置文件
        /// </summary>
        public override void SetAxis_iniFile(string path)
        {
            Span<string> lines = System.IO.File.ReadAllLines(path);
            string pattern = "=(.*)";
            string io = null;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("["))
                {
                    for (int j = 1; j < 7; j++)
                    {
                        Match match = Regex.Match(lines[i + j], pattern);
                        if (match.Success && Convert.ToInt32(match.Groups[1].Value) < 5 && Convert.ToInt32(match.Groups[1].Value) >= 0)
                        {
                            switch (j)
                            {
                                case 1: CMCDLL_NET.MCF_Set_ELP_Trigger_Net((ushort)(i / 7), Convert.ToUInt16(match.Groups[1].Value)); break;
                                case 2: CMCDLL_NET.MCF_Set_ELN_Trigger_Net((ushort)(i / 7), Convert.ToUInt16(match.Groups[1].Value)); break;
                                case 3: CMCDLL_NET.MCF_Set_Alarm_Trigger_Net((ushort)(i / 7), Convert.ToUInt16(match.Groups[1].Value)); break;
                                case 4: CMCDLL_NET.MCF_Set_Home_Trigger_Net((ushort)(i / 7), Convert.ToUInt16(match.Groups[1].Value)); break;
                                case 5: io = match.Groups[1].Value; break;
                                case 6: CardErrorMessage(CMCDLL_NET.MCF_Set_Input_Trigger_Net(0, (ushort)(i / 7), Convert.ToUInt16(io), Convert.ToUInt16(match.Groups[1].Value))); break;
                                default:
                                    break;
                            }
                        }
                    }
                    i = i + 4;
                }
            }
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
            Special_io = new bool[32];
            Task.Run(() =>
            {
                while (true)
                {
                    lock (this)
                    {
                        Thread.Sleep(30);
                        if (IO_Input != null)
                        {
                            if (!Special_io[estop])//紧急停止
                            {
                                if (IO_Input[estop])
                                {
                                    EStopPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[estop])
                            {
                                if (!IO_Input[estop])
                                {
                                    EStopNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[estop] = IO_Input[estop];
                            AutoReadEvent.WaitOne();
                            if (!Special_io[stop])//停止
                            {
                                if (IO_Input[stop])
                                {
                                    StopPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[stop])
                            {
                                if (!IO_Input[stop])
                                {
                                    StopNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[stop] = IO_Input[stop];
                            AutoReadEvent.WaitOne();

                            if (!Special_io[reset])//复位
                            {
                                if (IO_Input[reset])
                                {
                                    ResetPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[reset])
                            {
                                if (!IO_Input[reset])
                                {
                                    ResetNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[reset] = IO_Input[reset];
                            AutoReadEvent.WaitOne();

                            if (!Special_io[start])//启动
                            {
                                if (IO_Input[start])
                                {
                                    StartPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[start])
                            {
                                if (!IO_Input[start])
                                {
                                    StartNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[start] = IO_Input[start];
                            AutoReadEvent.WaitOne();
                        }
                    }
                }
            });
        }

        public override void Set_IOoutput(ushort card, ushort indexes, bool value)
        {
            throw new NotImplementedException();
        }

        public override void AwaitMoveLines(ushort card, ControlState t, int time = 0)
        {
            if (IsOpenCard)
            {
                ControlState control = t;
                foreach (var item in t.Axis)
                {
                    if (Axis_IO[item][0] == 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴驱动器报错，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴驱动器报错，单轴相对定位启动失败！");
                        return;
                    }
                    if (AxisStates[item][4] == 0 && AxisStates[item][7] == 0)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}轴运动中，单轴相对定位启动失败！");
                        else
                            throw new Exception($"{item}轴运动中，单轴相对定位启动失败！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        else
                            throw new Exception($"{item}单轴相对定位启动错误， {item}轴处于停止中！");
                        return;
                    }
                    if (IMoveStateQueue.Exists(e => e.Axis == item))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == item);
                        lock (this)
                        { IMoveStateQueue.Remove(colose); }
                    }
                    if (IMoveStateQueue.Exists(e => e.Axises.Contains(item)))
                    {
                        var colose = IMoveStateQueue.FirstOrDefault(x => x.Axises.Contains(item));
                        lock (this)
                        { IMoveStateQueue.Remove(colose); }
                    }
                }
                MoveState state = new MoveState()
                {
                    Axis = control.Axis[0],
                    CardID = card,
                    ACC = control.Acc,
                    Dcc = control.Dcc,
                    Speed = control.Speed,
                    UsingAxisNumber = control.UsingAxisNumber,
                    Axises = control.Axis,
                    OutTime = time,
                    Positions = control.Position
                };
                state.CurrentPositions = new double[state.UsingAxisNumber];
                ushort[] axis = new ushort[control.UsingAxisNumber];
                int[] pos = new int[control.UsingAxisNumber];
                axis = control.Axis;
                for (int i = 0; i < state.Axises.Length; i++)
                {
                    state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                }
                int type = 0;
                if (control.locationModel == 0)//相对
                {
                    for (int i = 0; i < control.Position.Length; i++)
                    {
                        TargetLocation[axis[i]] = state.Positions[i] + state.CurrentPositions[i];
                        AxisMachine[axis[i]] = 3;
                        AxisMoveModel[axis[i]] = 10;
                        pos[i] = Convert.ToInt32(control.Position[i]);
                    }
                    type = 1;
                    state.Movetype = 9;//相对
                }
                else if (control.locationModel == 1)//绝对
                {
                    for (int i = 0; i < control.Position.Length; i++)
                    {
                        TargetLocation[axis[i]] = control.Position[i];
                        AxisMachine[axis[i]] = 3;
                        AxisMoveModel[axis[i]] = 10;
                        pos[i] = Convert.ToInt32(control.Position[i]);
                    }
                    type = 0;
                    state.Movetype = 10;
                }
                lock (this)
                {
                    IMoveStateQueue.Add(state);
                }
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
                CoordinateSystemStates[coordinate] = 0;
                if (coordinate != -1)
                {
                    if (control.UsingAxisNumber == control.Axis.Length && control.UsingAxisNumber == control.Position.Length)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            lock (Motion_Lok[control.Axis[0]])
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴直线插补启动！");

                                stopwatch.Restart();
                                CMCDLL_NET.MCF_Set_Coordinate_Profile_Net((ushort)coordinate, 0, control.Speed, control.Speed / control.Acc, control.Speed / control.Acc, 0, 0);
                                switch (control.UsingAxisNumber)
                                {
                                    case 2:
                                        CMCDLL_NET.MCF_Line2_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 3:
                                        CMCDLL_NET.MCF_Line3_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    case 4:
                                        CMCDLL_NET.MCF_Line4_Net((ushort)coordinate, ref axis[0], ref pos[0], (ushort)type, 0);
                                        break;
                                    default:
                                        break;
                                }
                                Thread.Sleep(50);
                                do
                                {
                                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                        goto Timeout;
                                    if (Stop_sign[state.Axises[0]])
                                        goto Stop;
                                } while (AxisStates[state.Axises[0]][4] != 1);

                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                for (int i = 0; i < control.Position.Length; i++)
                                {
                                    AxisMachine[axis[i]] = 3;
                                    AxisMoveModel[axis[i]] = 0;
                                }
                                CoordinateSystemStates[coordinate] = 4;
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴直线插补完成，用时：{stopwatch.Elapsed}");
                                return;
                            Timeout:
                                stopwatch.Stop();
                                for (int i = 0; i < state.Axises.Length; i++)
                                {
                                    AxisStop(state.Axises[i]);
                                }
                                Console.WriteLine(stopwatch.Elapsed);
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{control.UsingAxisNumber} 轴直线插补等待到位超时 （{stopwatch.Elapsed}）");
                                CoordinateSystemStates[coordinate] = 4;
                                return;
                            Stop:
                                AxisStop(state.Axis, 1);
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{control.UsingAxisNumber}轴直线插补外部异常停止！（{stopwatch.Elapsed}）");
                                CoordinateSystemStates[coordinate] = 4;
                                return;
                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        return;
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"插补坐标系无空闲！");
                    else
                        throw new Exception($"插补坐标系无空闲！");
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

        public override void AxisReset(ushort axis)
        {
            Stop_sign[axis] = false;
            AxisMachine[axis] = 4;
            AxisMoveModel[axis] = 0;
        }

        public override void WaitAxis(int[] axis)
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

        ///// <summary>
        ///// 设置外部紧急停止输入（摩升泰专用方法）
        ///// </summary>
        ///// <param name="card">卡号</param>
        ///// <param name="in_put">输入点</param>
        ///// <param name="stop_model">停止模式 0：关闭触发 1：低电平触发紧急停止 2：低电平触发减速停止 3：高电平触发紧急停止 4：高电平触发减速停止</param>
        //public override void Set_ExigencyIO(ushort card, ushort in_put, uint stop_model)
        //{
        //    if (IsOpenCard)
        //    {
        //        for (ushort i = 0; i < Axis.Length; i++)
        //        {
        //            CardErrorMessage(CMCDLL_NET.MCF_Set_Input_Trigger_Net(card, i, in_put, stop_model));
        //        }
        //    }
        //}
    }
}
