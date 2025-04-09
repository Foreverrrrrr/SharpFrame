using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotionClass
{
    public class LeiSaiPulse : MotionBase
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

        /// <summary>
        /// 光栅上升沿触发事件
        /// </summary>
        public override event Action<DateTime> RasterPEvent;

        /// <summary>
        /// 光栅按钮下降沿触发事件
        /// </summary>
        public override event Action<DateTime> RasterNEvent;

        /// <summary>
        /// 门禁上升沿触发事件
        /// </summary>
        public override event Action<DateTime> EntrancePEvent;

        /// <summary>
        /// 门禁按钮下降沿触发事件
        /// </summary>
        public override event Action<DateTime> EntranceNEvent;

        /// <summary>
        /// 特殊IO
        /// </summary>
        public bool[] Special_io { get; set; }
        private ushort[] ThisStateMac { get; set; }
        private double[] TargetLocation { get; set; }
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
        public override short[] CoordinateSystemStates { get; set; } = new short[2];
        public override bool IsOpenCard { get; set; }
        public override int Axisquantity { get; set; }
        public override bool CAN_IsOpen { get; set; }
        public override double[] ADC_RealTime_DA { get; set; } = new double[2];
        public override double[] ADC_RealTime_AD { get; set; } = new double[4];

        public override event Action<DateTime, bool, string> CardLogEvent;

        public LeiSaiPulse()
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
            ADC_RealTime_AD = new double[4];
            ADC_RealTime_DA = new double[4];
            AxisStates = new double[Axis.Length][];
            Axis_IO = new int[Axis.Length][];
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Restart();
                for (ushort i = 0; i < Card_Number.Length; i++)
                {
                    IO_Input = Getall_IOinput(i);
                    IO_Output = Getall_IOoutput(i);
                }
                for (int i = 0; i < 4; i++)
                {
                    ADC_RealTime_AD[i] = Read_AD(0, (ushort)i);
                    ADC_RealTime_DA[i] = Read_DA(0, (ushort)i);
                }
                stopwatch.Stop();
                Thread.Sleep(20);
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
                for (ushort i = 0; i < 2; i++)
                {
                    CoordinateSystemStates[i] = LTDMC.dmc_check_done_multicoor(Card_Number[0], i);
                }
                stopwatch.Stop();
                Thread.Sleep(20);
                ///Console.WriteLine("State=>" + stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        public override void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0)
        {
            if (IsOpenCard)
            {
                if (IO_Input != null)
                {
                    if (LevelSignal)
                        waitvalue = !waitvalue;
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        stopwatch.Restart();
                        do
                        {
                            Thread.Sleep(20);
                            if (timeout != 0 && stopwatch.Elapsed.TotalMilliseconds > timeout)
                                goto Timeout;

                        } while (IO_Input[indexes] != waitvalue);
                        stopwatch.Stop();
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"等待输入口{indexes}，状态{waitvalue}完成（{stopwatch.Elapsed}）");
                        return;
                    Timeout:
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"等待输入口{indexes}，状态{waitvalue}超时（{stopwatch.Elapsed}）");
                        throw new Exception($"等待输入口{indexes}，等待状态{waitvalue}超时（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"等待输入口{indexes}，状态{waitvalue}线程异常中断（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"等待输入口{indexes}，等待状态{waitvalue}线程异常中断（{stopwatch.Elapsed}）");
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        private void AwaitMoveAbs(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    else
                        throw new Exception($"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    else
                        throw new Exception($"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    return;
                }
                if (state.Axis < Axis.Length && AxisStates[state.Axis][4] == 1 && AxisStates[state.Axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[state.Axis])
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}复位单轴阻塞绝对定位开始启动，定位地址{state.Position}，定位速度：{state.Speed}");
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = state.Position;
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], state.Axis, (int)state.Position, 1));
                            Thread.Sleep(50);
                        }
                        do
                        {
                            Thread.Sleep(20);
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (AxisStates[state.Axis][4] == 0);
                        do
                        {
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (LTDMC.dmc_check_success_pulse(Card_Number[0], state.Axis) != 1);
                        do
                        {
                            Thread.Sleep(20);
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (AxisStates[state.Axis][4] == 0);
                        stopwatch.Stop();
                        if (AxisStates[state.Axis][7] == 0)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位复位到位完成（{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            };
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(state.Axis);
                        //Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }

        public override void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞绝对定位启动错误！ {axis}轴在运动中！");
                    else
                        throw new Exception($"{axis}单轴阻塞绝对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞绝对定位启动错误！ {axis}轴未上使能！");
                    else
                        throw new Exception($"{axis}单轴阻塞绝对定位启动错误！ {axis}轴未上使能！");
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
                if (axis < Axis.Length && AxisStates[axis][4] == 1 && AxisStates[axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    MoveState state;
                    try
                    {
                        lock (Motion_Lok[axis])
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴阻塞绝对定位开始启动，定位地址{position}，定位速度：{speed}");
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
                            if (IMoveStateQueue.Exists(e => e.Axis == axis))
                            {
                                var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                                IMoveStateQueue.Remove(colose);
                            }
                            TargetLocation[state.Axis] = state.Position;
                            IMoveStateQueue.Add(state);
                            stopwatch.Restart();
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], axis, (int)position, 1));
                            Thread.Sleep(50);
                        }
                        do
                        {
                            Thread.Sleep(20);
                            if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                goto Timeout;
                        } while (AxisStates[axis][4] == 0);
                        do
                        {
                            if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                goto Timeout;
                        } while (LTDMC.dmc_check_success_pulse(Card_Number[0], axis) != 1);
                        do
                        {
                            Thread.Sleep(20);
                            if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                goto Timeout;
                        } while (AxisStates[axis][4] == 0);
                        stopwatch.Stop();
                        if (AxisStates[axis][7] == 0)
                        {
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴定位地址{position}，单轴阻塞绝对定位到位完成 （{stopwatch.Elapsed}）");
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{axis}轴定位地址{position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                            return;
                            //if (CardLogEvent != null)
                            //    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，到位编码器误差过大！（{stopwatch.Elapsed}）");
                            //throw new Exception($"{axis}轴定位地址{position}，到位编码器误差过大！（{stopwatch.Elapsed}）");
                            //return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(axis);
                        //Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{axis}轴定位地址{position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
                        return;
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        private void AwaitMoveHome(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴在运动中！");
                    else
                        throw new Exception($"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴未上使能！");
                    else
                        throw new Exception($"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴处于停止中！");
                    return;
                }
                if (state.Axis < Axis.Length && AxisStates[state.Axis][4] == 1 && AxisStates[state.Axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[state.Axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}单轴阻塞原点回归复位启动！");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = state.Position;
                            LTDMC.dmc_set_home_pin_logic(Card_Number[0], state.Axis, 0, 0);//设置原点低电平有效
                            LTDMC.dmc_set_profile(Card_Number[0], state.Axis, state.Speed, state.Speed * 2, state.ACC, state.Dcc, state.Dcc);//设置起始速度、运行速度、停止速度、加速时间、减速时间
                            LTDMC.dmc_set_homemode(Card_Number[0], state.Axis, 1, 1, state.HomeModel, 0);//设置回零模式
                            CardErrorMessage(LTDMC.nmc_home_move(Card_Number[0], state.Axis));
                            Thread.Sleep(50);
                        }
                        do
                        {
                            Thread.Sleep(20);
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;

                        } while (AxisStates[state.Axis][4] == 0);
                        do
                        {
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (LTDMC.dmc_check_done(Card_Number[0], state.Axis) != 1);
                        stopwatch.Stop();
                        if (!Stop_sign[state.Axis])
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}轴原点回归完成，零点误差：{AxisStates[state.Axis][0]} （{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            LTDMC.dmc_set_position(Card_Number[0], state.Axis, 0);
                            LTDMC.dmc_set_encoder(Card_Number[0], state.Axis, 0);
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(state.Axis);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.Axis}轴阻塞原点回归复位超时停止！（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{state.Axis}轴阻塞原点回归复位超时停止！（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴阻塞原点回归复位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴阻塞原点回归复位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }
        public override void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞原点回归启动错误！ {axis}轴在运动中！");
                    else
                        throw new Exception($"{axis}单轴阻塞原点回归启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞原点回归启动错误！ {axis}轴未上使能！");
                    else
                        throw new Exception($"{axis}单轴阻塞原点回归启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞原点回归启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴阻塞原点回归启动错误！ {axis}轴处于停止中！");
                    return;
                }
                if (axis < Axis.Length && AxisStates[axis][4] == 1 && AxisStates[axis][5] == 4)
                {
                    MoveState state;
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴阻塞原点回归启动！");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            stopwatch.Restart();
                            double tv = Math.Abs(home_speed);
                            LTDMC.dmc_set_home_pin_logic(Card_Number[0], axis, 0, 0);//设置原点低电平有效
                            LTDMC.dmc_set_profile(Card_Number[0], axis, tv, tv * 2, acc, dcc, dcc);//设置起始速度、运行速度、停止速度、加速时间、减速时间
                            if (home_speed > 0)
                            {
                                LTDMC.dmc_set_homemode(Card_Number[0], axis, 1, 1, home_model, 0);//设置回零模式
                            }
                            else
                            {
                                LTDMC.dmc_set_homemode(Card_Number[0], axis, 0, 1, home_model, 0);//设置回零模式
                            }
                            state = new MoveState()
                            {
                                Axis = axis,
                                Speed = home_speed,
                                Position = 0,
                                Movetype = 6,
                                OutTime = timeout,
                                Handle = DateTime.Now,
                            };
                            if (IMoveStateQueue.Exists(e => e.Axis == axis))
                            {
                                var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                                IMoveStateQueue.Remove(colose);
                            }
                            IMoveStateQueue.Add(state);
                            CardErrorMessage(LTDMC.nmc_home_move(Card_Number[0], axis));
                            Thread.Sleep(50);
                        }
                        do
                        {
                            Thread.Sleep(20);
                            if (timeout != 0 && stopwatch.Elapsed.TotalMilliseconds > timeout)
                                goto Timeout;

                        } while (AxisStates[axis][4] == 0);
                        do
                        {
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (LTDMC.dmc_check_done(Card_Number[0], state.Axis) != 1);
                        stopwatch.Stop();
                        if (!Stop_sign[axis])
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴原点回归完成，零点误差：{AxisStates[axis][0]} （{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }

                            LTDMC.dmc_set_position(Card_Number[0], axis, 0);
                            LTDMC.dmc_set_encoder(Card_Number[0], axis, 0);
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(axis);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{axis}轴阻塞原点回归超时停止！（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{axis}轴阻塞原点回归超时停止！（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}轴阻塞原点回归线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}轴阻塞原点回归线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }


        private void AwaitMoveLines(MoveState state)
        {
            if (IsOpenCard)
            {
                foreach (var item in state.Axises)
                {
                    if (AxisStates[item][4] != 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在运动中！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴未上使能！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[state.Axis])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在停止中！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在停止中！");
                        return;
                    }
                    CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], item));
                }
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)1);
                if (coordinate != -1)
                {
                    if (state.UsingAxisNumber == state.Axises.Length && state.UsingAxisNumber == state.Positions.Length)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            lock (Motion_Lok[state.Axis])
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补启动！");
                                ushort[] axis = new ushort[state.UsingAxisNumber];
                                int[] pos = new int[state.UsingAxisNumber];
                                axis = state.Axises;
                                ushort movtype = 0;
                                if (state.Movetype == 7)
                                {
                                    movtype = 0;
                                    for (int i = 0; i < state.UsingAxisNumber; i++)
                                    {
                                        TargetLocation[state.Axises[i]] = state.Positions[i];
                                        pos[i] = Convert.ToInt32(state.Positions[i] - AxisStates[state.Axises[i]][0]);
                                    }
                                }
                                else
                                {
                                    movtype = 1;
                                    for (int i = 0; i < state.UsingAxisNumber; i++)
                                    {
                                        TargetLocation[state.Axises[i]] = state.Positions[i];
                                        pos[i] = Convert.ToInt32(state.Positions[i]);
                                    }
                                }
                                stopwatch.Restart();
                                LTDMC.dmc_set_vector_profile_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), 0, state.Speed, state.ACC, 0, 0);
                                CardErrorMessage(LTDMC.dmc_line_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), state.UsingAxisNumber, axis, pos, movtype));
                                Thread.Sleep(50);
                            }
                            do
                            {
                                Thread.Sleep(20);
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;
                            } while (CoordinateSystemStates[coordinate] == 0);
                            do
                            {
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;
                            } while (CoordinateSystemStates[coordinate] != 1);
                            if (CoordinateSystemStates[coordinate] == 1)
                            {
                                foreach (var item in state.Axises)
                                {
                                    do
                                    {
                                        Thread.Sleep(20);
                                    } while (AxisStates[item][4] != 1);
                                    if (AxisStates[item][7] != 0 || Stop_sign[item])
                                        goto Stop;
                                }
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补动作完成！({stopwatch.Elapsed})");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[state.CardID], Convert.ToUInt16(coordinate), 0);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{state.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                            Stop:
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{state.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");

                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[state.CardID], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补坐标系忙碌！");
                    else
                        throw new Exception($"{state.UsingAxisNumber}插补坐标系忙碌！");
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
        public override void AwaitMoveLines(ushort card, ControlState t, int time = 0)
        {
            if (IsOpenCard)
            {
                ControlState control = t;
                foreach (var item in control.Axis)
                {
                    if (AxisStates[item][4] != 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
                        return;
                    }
                    CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], item));
                    if (IMoveStateQueue.Exists(e => e.Axis == item))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == item);
                        IMoveStateQueue.Remove(colose);
                    }
                    if (IMoveStateQueue.Exists(e => e.Axises.Contains(item)))
                    {
                        var colose = IMoveStateQueue.FirstOrDefault(x => x.Axises.Contains(item));
                        IMoveStateQueue.Remove(colose);
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
                if (control.locationModel == 0)
                {
                    state.Movetype = 9;//相对
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = control.Position[i];
                        TargetLocation[state.Axises[i]] = AxisStates[state.Axises[i]][0] + state.Positions[i];
                    }
                }
                else
                {
                    state.Movetype = 10;
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        TargetLocation[state.Axises[i]] = state.Positions[i];
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = control.Position[i];
                    }
                }
                IMoveStateQueue.Add(state);
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)1);
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
                                ushort[] axis = new ushort[control.UsingAxisNumber];
                                int[] pos = new int[control.UsingAxisNumber];
                                axis = control.Axis;
                                for (int i = 0; i < pos.Length; i++)
                                {
                                    pos[i] = (int)control.Position[i];
                                }
                                stopwatch.Restart();
                                LTDMC.dmc_set_vector_profile_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), 0, control.Speed, control.Acc, 0, 0);
                                CardErrorMessage(LTDMC.dmc_line_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), control.UsingAxisNumber, axis, pos, (ushort)control.locationModel));
                                Thread.Sleep(50);
                            }
                            do
                            {
                                Thread.Sleep(20);
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (CoordinateSystemStates[coordinate] == 1);
                            do
                            {
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (CoordinateSystemStates[coordinate] != 1);

                            if (CoordinateSystemStates[coordinate] == 1)
                            {
                                foreach (var item in control.Axis)
                                {
                                    do
                                    {
                                        Thread.Sleep(0);
                                    } while (AxisStates[item][4] != 1);
                                    if (AxisStates[item][7] != 0 || Stop_sign[item])
                                        goto Stop;
                                }
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴直线插补动作完成！({stopwatch.Elapsed})");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 0);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{control.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                            return;
                        Stop:
                            stopwatch.Stop();
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{control.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
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
                        CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补坐标系忙碌！");
                    else
                        throw new Exception($"{control.UsingAxisNumber}插补坐标系忙碌！");
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

        private void AwaitMoveRel(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞相对定位复位启动错误！ {state.Axis}轴在运动中！");
                    throw new Exception($"{state.Axis}单轴阻塞相对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞相对定位复位启动错误！ {state.Axis}轴未上使能！");
                    throw new Exception($"{state.Axis}单轴阻塞相对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞相对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    throw new Exception($"{state.Axis}单轴阻塞相对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    return;
                }

                if (state.Axis < Axis.Length && AxisStates[state.Axis][4] == 1 && AxisStates[state.Axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[state.Axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}单轴阻塞相对定位复位开始启动，定位地址{state.Position}，定位速度：{state.Speed}");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            TargetLocation[state.Axis] = state.Position;
                            var t = state.Position - AxisStates[state.Axis][0];
                            stopwatch.Restart();
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], state.Axis, (int)state.Position, 0));
                            Thread.Sleep(50);
                        }
                        do
                        {
                            Thread.Sleep(20);
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (AxisStates[state.Axis][4] == 0);
                        do
                        {
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (LTDMC.dmc_check_success_pulse(Card_Number[0], state.Axis) != 1);
                        do
                        {
                            Thread.Sleep(20);
                            if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                goto Timeout;
                        } while (AxisStates[state.Axis][4] == 0);
                        stopwatch.Stop();
                        if (AxisStates[state.Axis][7] == 0)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞相对定位复位到位完成 （{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞相对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴阻塞相对定位复位外部异常停止！ （{stopwatch.Elapsed}");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(state.Axis);
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞相对定位复位等待到位超时 （{stopwatch.Elapsed}");
                        throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴阻塞相对定位复位等待到位超时 （{stopwatch.Elapsed}");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴阻塞相对定位复位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴阻塞相对定位复位线程异常停止！({stopwatch.Elapsed})");
                        return;
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }
        public override void AwaitMoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴未上使能！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴处于停止中！");
                    return;
                }
                if (axis < Axis.Length && AxisStates[axis][4] == 1 && AxisStates[axis][5] == 4)
                {
                    MoveState state;
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[axis])
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴相对定位开始启动，定位地址{position}，定位速度：{speed}");
                            state = new MoveState()
                            {
                                Axis = axis,
                                CurrentPosition = AxisStates[axis][0],
                                Speed = speed,
                                Position = AxisStates[axis][0] + position,
                                Movetype = 4,
                                OutTime = time,
                                Handle = DateTime.Now,
                            };
                            if (IMoveStateQueue.Exists(e => e.Axis == axis))
                            {
                                var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                                IMoveStateQueue.Remove(colose);
                            }
                            IMoveStateQueue.Add(state);
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = state.Position;
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], axis, (int)position, 0));
                            Thread.Sleep(50);
                        }

                        do
                        {
                            Thread.Sleep(20);
                            if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                goto Timeout;

                        } while (AxisStates[axis][4] == 0);
                        do
                        {
                            if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                goto Timeout;
                        } while (LTDMC.dmc_check_success_pulse(Card_Number[0], axis) != 1);
                        stopwatch.Stop();
                        if (AxisStates[axis][7] == 0)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位到位完成 （{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位外部异常停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位外部异常停止! （{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(state.Axis);
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴相对定位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴相对定位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
                ThisStateMac[axis] = 0;
                AxisStop(axis, 0, false);
                LTDMC.dmc_write_sevon_pin(Card_Number[card], axis, 1);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{axis}轴下使能");
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先初始化板卡再下使能轴");
                else
                    throw new Exception("请先初始化板卡再下使能轴");
            }
        }

        public override void AxisOff()
        {
            if (IsOpenCard)
            {
                for (ushort i = 0; i < Card_Number.Length; i++)
                {
                    for (ushort j = 0; j < Axis.Length; j++)
                    {
                        ThisStateMac[j] = 0;
                        LTDMC.dmc_write_sevon_pin(Card_Number[i], j, 1);
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
            if (IsOpenCard)
            {
                ThisStateMac[axis] = 4;
                LTDMC.dmc_write_sevon_pin(Card_Number[card], axis, 0);
                LTDMC.dmc_set_factor_error(Card_Number[card], axis, 1, FactorValue);
                AxisReset(axis);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"{axis}轴上使能");
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先初始化板卡再使能轴");
                else
                    throw new Exception("请先初始化板卡再使能轴！");
            }
        }

        public override void AxisOn()
        {
            if (IsOpenCard)
            {
                for (int i = 0; i < Card_Number.Length; i++)
                {
                    for (ushort j = 0; j < Axis.Length; j++)
                    {
                        ThisStateMac[j] = 4;
                        LTDMC.dmc_write_sevon_pin(Card_Number[i], j, 0);
                        Stop_sign[j] = false;
                        CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], j));
                        LTDMC.dmc_set_factor_error(Card_Number[i], j, 1, FactorValue);
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
            LTDMC.dmc_write_erc_pin(Card_Number[0], axis, 0);
            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
            LTDMC.dmc_write_erc_pin(Card_Number[0], axis, 1);
        }

        public override void AxisStop(ushort axis, int stop_mode = 0, bool all = false)
        {
            if (IsOpenCard)
            {
                if (axis < Axis.Length)
                {
                    if (!all)
                    {
                        CardErrorMessage(LTDMC.dmc_stop(Card_Number[0], axis, (ushort)stop_mode));
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{axis}轴停止！");
                        if (stop_mode == 0)
                        {
                            Stop_sign[axis] = false;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴减速停止！");
                        }
                        else
                        {
                            Stop_sign[axis] = true;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴紧急停止！");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Axis.Length; i++)
                        {
                            Stop_sign[i] = true;
                            if (AxisStates[axis][6] == 9)
                            {
                                LTDMC.dmc_stop_multicoor(Card_Number[0], 0, 0);
                                LTDMC.dmc_stop_multicoor(Card_Number[0], 1, 0);
                            }
                            else
                            {
                                CardErrorMessage(LTDMC.dmc_emg_stop(Card_Number[0]));

                            }
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
            AutoReadEvent.Reset();
            foreach (var item in Read_ThreadPool)
            {
                item.Abort();
            }
            IsOpenCard = false;
            Thread.Sleep(100);
            CardErrorMessage(LTDMC.dmc_board_close());
            Thismotion = null;
        }

        public override bool[] Getall_IOinput(ushort card)
        {
            if (IsOpenCard)
            {
                if (IO_Input != null)
                {

                    var input = LTDMC.dmc_read_inport(Card_Number[card], 0);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Input[i] = (input & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                    }
                    uint[] ipt = new uint[3];
                    LTDMC.nmc_read_inport(Card_Number[card], 1, 0, ref ipt[0]);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Input[i + 16] = (ipt[0] & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                }
                    LTDMC.nmc_read_inport(Card_Number[card], 2, 0, ref ipt[1]);
                    for (int i = 0; i < 32; i++)
                    {
                        IO_Input[i + 32] = (ipt[1] & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
            }
                    LTDMC.nmc_read_inport(Card_Number[card], 2, 1, ref ipt[2]);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Input[i + 64] = (ipt[2] & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
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
                    var output = LTDMC.dmc_read_outport(Card_Number[card], 0);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Output[i] = (output & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                    }
                    uint[] ipt = new uint[3];
                    LTDMC.nmc_read_outport(Card_Number[card], 1, 0, ref ipt[0]);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Output[i + 16] = (ipt[0] & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                }
                    LTDMC.nmc_read_outport(Card_Number[card], 2, 0, ref ipt[1]);
                    for (int i = 0; i < 32; i++)
                    {
                        IO_Output[i + 32] = (ipt[1] & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                    }
                    LTDMC.nmc_read_outport(Card_Number[card], 2, 1, ref ipt[2]);
                    for (int i = 0; i < 16; i++)
                    {
                        IO_Output[i + 64] = (ipt[2] & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
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

                var state = LTDMC.dmc_axis_io_status(Card_Number[0], axis);
                bools[0] = (state & 1) == 1 ? 1 : 0;// 伺服报警 True=ON 
                bools[1] = (state & 2) == 2 ? 1 : 0;// 正限位 True=ON 
                bools[2] = (state & 4) == 4 ? 1 : 0;// 负限位 True=ON 
                bools[3] = (state & 8) == 8 ? 1 : 0;// 急停 True=ON 
                bools[4] = (state & 16) == 16 ? 1 : 0;// 原点 True=ON 
                bools[5] = (state & 32) == 32 ? 1 : 0;// 正软限位 True=ON 
                bools[6] = (state & 64) == 64 ? 1 : 0;// 负软限位 True=ON
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
            double[] doubles = new double[8];
            if (IsOpenCard)
            {
                int a = 0;
                LTDMC.dmc_get_position_unit(Card_Number[0], axis, ref doubles[0]); //脉冲位置
                LTDMC.dmc_get_encoder_unit(Card_Number[0], axis, ref doubles[1]);//编码器
                doubles[2] = TargetLocation[axis];
                //LTDMC.dmc_get_target_position_unit(Card_Number[0], axis, ref doubles[2]);//目标位置
                doubles[3] = LTDMC.dmc_read_current_speed(Card_Number[0], axis);//速度
                doubles[4] = LTDMC.dmc_check_done(Card_Number[0], axis);//轴运动到位 0=运动中 1=轴停止
                state[0] = ThisStateMac[axis];
                //LTDMC.nmc_get_axis_state_machine(Card_Number[0], axis, ref state[0]);//轴状态机：0：轴处于未启动状态 1：轴处于启动禁止状态 2：轴处于准备启动状态 3：轴处于启动状态 4：轴处于操作使能状态 5：轴处于停止状态 6：轴处于错误触发状态 7：轴处于错误状态
                LTDMC.dmc_get_axis_run_mode(Card_Number[0], axis, ref state[1]);//轴运行模式：0：空闲 1：定位模式 2：定速模式 3：回零模式 4：手轮模式 5：Ptt / Pts 6：Pvt / Pvts 10：Continue
                LTDMC.dmc_get_stop_reason(Card_Number[0], axis, ref a);//轴停止原因获取：0：正常停止  3：LTC 外部触发立即停止，IMD_STOP_AT_LTC 4：EMG 立即停止，IMD_STOP_AT_EMG 5：正硬限位立即停止，IMD_STOP_AT_ELP6：负硬限位立即停止，IMD_STOP_AT_ELN7：正硬限位减速停止，DEC_STOP_AT_ELP8：负硬限位减速停止，DEC_STOP_AT_ELN9：正软限位立即停止，IMD_STOP_AT_SOFT_ELP10：负软限位立即停止，IMD_STOP_AT_SOFT_ELN11：正软限位减速停止，DEC_STOP_AT_SOFT_ELP12：负软限位减速停止，DEC_STOP_AT_SOFT_ELN13：命令立即停止，IMD_STOP_AT_CMD14：命令减速停止，DEC_STOP_AT_CMD15：其它原因立即停止，IMD_STOP_AT_OTHER16：其它原因减速停止，DEC_STOP_AT_OTHER17：未知原因立即停止，IMD_STOP_AT_UNKOWN18：未知原因减速停止，DEC_STOP_AT_UNKOWN     
                Array.Copy(state, 0, doubles, 5, 2);
                doubles[7] = a;
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
            return doubles;
        }

        public override int[] GetEtherCATState(ushort card_number)
        {
            throw new NotImplementedException();
        }
        private void MoveAbs(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    else
                        throw new Exception($"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    else
                        throw new Exception($"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    return;
                }

                if (state.Axis < Axis.Length && AxisStates[state.Axis][4] == 1 && AxisStates[state.Axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[state.Axis])
                        {
                            TargetLocation[state.Axis] = state.Position;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}单轴绝对定位复位开始启动！，定位地址{state.Position}，定位速度：{state.Speed}");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            stopwatch.Restart();
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], state.Axis, (int)state.Position, 1));
                            Thread.Sleep(50);
                        }
                        Task.Factory.StartNew(() =>
                        {
                            do
                            {
                                Thread.Sleep(20);
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;

                            } while (AxisStates[state.Axis][4] == 0);
                            do
                            {
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
                            stopwatch.Stop();
                            if (AxisStates[state.Axis][7] == 0)
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位到位完成（{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                            else
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位等待到位超时（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位等待到位超时（{stopwatch.Elapsed}）");
                        });

                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴绝对定位复位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴绝对定位复位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }
        public override void MoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                    else
                        throw new Exception($"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴未上使能！");
                    else
                        throw new Exception($"{axis}单轴绝对定位启动错误！ {axis}轴未上使能！");
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
                if (axis < Axis.Length && AxisStates[axis][4] == 1 && AxisStates[axis][5] == 4)
                {
                    MoveState state;
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[axis])
                        {

                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴绝对定位开始启动，定位地址{position}，定位速度：{speed}");
                            state = new MoveState()
                            {
                                Axis = axis,
                                CurrentPosition = AxisStates[axis][0],
                                Speed = speed,
                                Position = position,
                                Movetype = 1,
                                OutTime = time,
                                Handle = DateTime.Now,
                            };
                            if (IMoveStateQueue.Exists(e => e.Axis == axis))
                            {
                                var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                                IMoveStateQueue.Remove(colose);
                            }
                            IMoveStateQueue.Add(state);
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = state.Position;
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], axis, (int)position, 1));
                            Thread.Sleep(50);
                        }
                        Task.Factory.StartNew(() =>
                        {
                            do
                            {
                                Thread.Sleep(20);
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (AxisStates[axis][4] == 0);
                            do
                            {
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_success_pulse(Card_Number[0], axis) != 1);
                            stopwatch.Stop();
                            if (AxisStates[axis][7] == 0)
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{axis}轴定位地址{position}，单轴绝对定位到位完成 （{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                            else
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位外部异常停止! （{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴绝对定位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴绝对定位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }

        public override void MoveCircle_Center(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        private void MoveHome(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴在运动中！");
                    else
                        throw new Exception($"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴未上使能！");
                    else
                        throw new Exception($"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴处于停止中！");
                    return;
                }
                if (state.Axis < Axis.Length && AxisStates[state.Axis][4] == 1 && AxisStates[state.Axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[state.Axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}单轴原点回归复位启动！");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = 0;
                            LTDMC.dmc_set_home_pin_logic(Card_Number[0], state.Axis, 0, 0);//设置原点低电平有效
                            LTDMC.dmc_set_profile(Card_Number[0], state.Axis, state.Speed, state.Speed * 2, state.ACC, state.Dcc, state.Dcc);//设置起始速度、运行速度、停止速度、加速时间、减速时间
                            LTDMC.dmc_set_homemode(Card_Number[0], state.Axis, 1, 1, state.HomeModel, 0);//设置回零模式
                            CardErrorMessage(LTDMC.nmc_home_move(Card_Number[0], state.Axis));
                            Thread.Sleep(50);
                        }
                        Task.Factory.StartNew(() =>
                        {
                            do
                            {
                                Thread.Sleep(20);
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;

                            } while (AxisStates[state.Axis][4] == 0);
                            do
                            {
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_done(Card_Number[0], state.Axis) != 1);
                            stopwatch.Stop();
                            if (!Stop_sign[state.Axis])
                            {
                                CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.Axis}轴原点回归完成，零点误差：{AxisStates[state.Axis][0]} （{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }

                                LTDMC.dmc_set_position(Card_Number[0], state.Axis, 0);
                                LTDMC.dmc_set_encoder(Card_Number[0], state.Axis, 0);
                                return;
                            }
                            else
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.Axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{state.Axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴原点回归复位超时停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴原点回归复位超时停止！（{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴原点回归复位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴原点回归复位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }

        public override void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴原点回归启动错误！ {axis}轴在运动中！");
                    else
                        throw new Exception($"{axis}单轴原点回归启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴原点回归启动错误！ {axis}轴未上使能！");
                    else
                        throw new Exception($"{axis}单轴原点回归启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴原点回归启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴原点回归启动错误！ {axis}轴处于停止中！");
                    return;
                }
                if (axis < Axis.Length && AxisStates[axis][4] == 1 && AxisStates[axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    MoveState state;
                    try
                    {
                        lock (Motion_Lok[axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴原点回归启动！");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            stopwatch.Restart();
                            double tv = Math.Abs(home_speed);
                            LTDMC.dmc_set_home_pin_logic(Card_Number[0], axis, 0, 0);//设置原点低电平有效
                            LTDMC.dmc_set_profile(Card_Number[0], axis, tv / 10, tv, acc, dcc, dcc);//设置起始速度、运行速度、停止速度、加速时间、减速时间
                            if (home_speed > 0)
                            {
                                LTDMC.dmc_set_homemode(Card_Number[0], axis, 1, 1, home_model, 0);//设置回零模式
                            }
                            else
                            {
                                LTDMC.dmc_set_homemode(Card_Number[0], axis, 0, 1, home_model, 0);//设置回零模式
                            }
                            //CardErrorMessage(LTDMC.nmc_set_home_profile(Card_Number[0], axis, home_model, home_speed / 2, home_speed, acc, dcc, offpos));
                            state = new MoveState()
                            {
                                Axis = axis,
                                Speed = home_speed,
                                HomeModel = home_model,
                                Movetype = 5,
                                ACC = acc,
                                Dcc = dcc,
                                Home_off = offpos,
                                OutTime = timeout,
                                Handle = DateTime.Now,
                            };
                            if (IMoveStateQueue.Exists(e => e.Axis == axis))
                            {
                                var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                                IMoveStateQueue.Remove(colose);
                            }
                            TargetLocation[state.Axis] = 0;
                            IMoveStateQueue.Add(state);
                            CardErrorMessage(LTDMC.nmc_home_move(Card_Number[0], axis));
                            Thread.Sleep(50);
                        }
                        Task.Factory.StartNew(() =>
                        {
                            do
                            {
                                Thread.Sleep(20);
                                if (timeout != 0 && stopwatch.Elapsed.TotalMilliseconds > timeout)
                                    goto Timeout;

                            } while (AxisStates[axis][4] == 0);
                            do
                            {
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_done(Card_Number[0], state.Axis) != 1);
                            stopwatch.Stop();
                            ushort homest = 0;
                            if (!Stop_sign[axis])
                            {
                                CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{axis}轴原点回归完成，零点误差：{AxisStates[axis][0]} （{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }

                                LTDMC.dmc_set_position(Card_Number[0], axis, 0);
                                LTDMC.dmc_set_encoder(Card_Number[0], axis, 0);
                                return;
                            }
                            else
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(axis);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归超时停止！（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{axis}轴原点回归超时停止！（{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}轴原点回归线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}轴原点回归线程异常停止！({stopwatch.Elapsed})");
                        return;
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
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], axis, 0, speed, acc, dec, 0));//设置速度参数
                            LTDMC.dmc_vmove(Card_Number[0], axis, (ushort)posi_mode);
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

        private void MoveLines(MoveState state)
        {
            if (IsOpenCard)
            {
                foreach (var item in state.Axises)
                {
                    if (AxisStates[item][4] != 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在运动中！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴未上使能！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[state.Axis])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在停止中！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在停止中！");
                        return;
                    }
                    CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], item));
                }
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)1);
                if (coordinate != -1)
                {
                    if (state.UsingAxisNumber == state.Axises.Length && state.UsingAxisNumber == state.Positions.Length)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            lock (Motion_Lok[state.Axis])
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补启动！");
                                ushort[] axis = new ushort[state.UsingAxisNumber];
                                int[] pos = new int[state.UsingAxisNumber];
                                axis = state.Axises;
                                ushort movtype = 0;
                                if (state.Movetype == 7)
                                {
                                    movtype = 0;
                                    for (int i = 0; i < state.UsingAxisNumber; i++)
                                    {
                                        TargetLocation[state.Axises[i]] = state.Positions[i];
                                        pos[i] = Convert.ToInt32(state.Positions[i] - AxisStates[state.Axises[i]][0]);
                                    }
                                }
                                else
                                {
                                    movtype = 1;
                                    for (int i = 0; i < state.UsingAxisNumber; i++)
                                    {
                                        TargetLocation[state.Axises[i]] = state.Positions[i];
                                        pos[i] = Convert.ToInt32(state.Positions[i]);
                                    }
                                }
                                stopwatch.Restart();
                                LTDMC.dmc_set_vector_profile_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), 0, state.Speed, state.ACC, 0, 0);
                                CardErrorMessage(LTDMC.dmc_line_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), state.UsingAxisNumber, axis, pos, movtype));
                                Thread.Sleep(50);
                            }
                            Task.Factory.StartNew(() =>
                            {
                                do
                                {
                                    Thread.Sleep(20);
                                    if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                        goto Timeout;
                                } while (CoordinateSystemStates[coordinate] == 0);
                                do
                                {
                                    if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                        goto Timeout;
                                } while (CoordinateSystemStates[coordinate] != 1);
                                if (CoordinateSystemStates[coordinate] == 1)
                                {
                                    foreach (var item in state.Axises)
                                    {
                                        do
                                        {
                                            Thread.Sleep(20);
                                        } while (AxisStates[item][4] != 1);
                                        if (AxisStates[item][7] != 0 || Stop_sign[item])
                                            goto Stop;
                                    }
                                    stopwatch.Stop();
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, false, $"{state.UsingAxisNumber}轴直线插补动作完成！({stopwatch.Elapsed})");
                                    lock (this)
                                    {
                                        IMoveStateQueue.Remove(state);
                                    }
                                    return;
                                }
                            Timeout:
                                stopwatch.Stop();
                                LTDMC.dmc_stop_multicoor(Card_Number[state.CardID], Convert.ToUInt16(coordinate), 0);
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                                else
                                    throw new Exception($"{state.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                                Stop:
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                                else
                                    throw new Exception($"{state.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            });
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[state.CardID], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        else
                            throw new Exception($"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补坐标系忙碌！");
                    else
                        throw new Exception($"{state.UsingAxisNumber}插补坐标系忙碌！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }
        public override void MoveLines(ushort card, ControlState t, int time = 0)
        {
            if (IsOpenCard)
            {
                ControlState control = t;
                foreach (var item in control.Axis)
                {
                    if (AxisStates[item][4] != 1)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
                        else
                            throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
                        return;
                    }
                    CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], item));
                    if (IMoveStateQueue.Exists(e => e.Axis == item))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == item);
                        IMoveStateQueue.Remove(colose);
                    }
                    if (IMoveStateQueue.Exists(e => e.Axises.Contains(item)))
                    {
                        var colose = IMoveStateQueue.FirstOrDefault(x => x.Axises.Contains(item));
                        IMoveStateQueue.Remove(colose);
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
                if (control.locationModel == 0)
                {
                    state.Movetype = 7;//相对
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = control.Position[i];
                        TargetLocation[state.Axises[i]] = AxisStates[state.Axises[i]][0] + control.Position[i];
                    }
                }
                else
                {
                    state.Movetype = 8;
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = control.Position[i];
                        TargetLocation[state.Axises[i]] = state.Positions[i];
                    }
                }
                IMoveStateQueue.Add(state);
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)1);
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
                                ushort[] axis = new ushort[control.UsingAxisNumber];
                                int[] pos = new int[control.UsingAxisNumber];
                                axis = control.Axis;
                                for (int i = 0; i < pos.Length; i++)
                                {
                                    pos[i] = (int)control.Position[i];
                                }
                                stopwatch.Restart();
                                LTDMC.dmc_set_vector_profile_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), 0, control.Speed, control.Acc, 0, 0);
                                CardErrorMessage(LTDMC.dmc_line_multicoor(Card_Number[0], Convert.ToUInt16(coordinate), control.UsingAxisNumber, axis, pos, (ushort)control.locationModel));
                                Thread.Sleep(50);
                            }
                            Task.Factory.StartNew(() =>
                            {
                                do
                                {
                                    Thread.Sleep(20);
                                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                        goto Timeout;
                                } while (CoordinateSystemStates[coordinate] == 1);
                                do
                                {
                                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                        goto Timeout;
                                } while (CoordinateSystemStates[coordinate] != 1);

                                if (CoordinateSystemStates[coordinate] == 1)
                                {
                                    foreach (var item in control.Axis)
                                    {
                                        do
                                        {
                                            Thread.Sleep(0);
                                        } while (AxisStates[item][4] != 1);
                                        if (AxisStates[item][7] != 0 || Stop_sign[item])
                                            goto Stop;
                                    }
                                    stopwatch.Stop();
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴直线插补动作完成！({stopwatch.Elapsed})");
                                    lock (this)
                                    {
                                        IMoveStateQueue.Remove(state);
                                    }
                                    return;
                                }
                            Timeout:
                                stopwatch.Stop();
                                LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 0);
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                                else
                                    throw new Exception($"{control.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                                return;
                            Stop:
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                                else
                                    throw new Exception($"{control.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                                return;
                            });
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            else
                                throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
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
                        CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补坐标系忙碌！");
                    else
                        throw new Exception($"{control.UsingAxisNumber}插补坐标系忙碌！");
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

        private void MoveRel(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴在运动中！");
                    else
                        throw new Exception($"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴未上使能！");
                    else
                        throw new Exception($"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    else
                        throw new Exception($"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴处于停止中！");
                    return;
                }

                if (state.Axis < Axis.Length && AxisStates[state.Axis][4] == 1 && AxisStates[state.Axis][5] == 4)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[state.Axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}单轴相对定位复位开始启动，定位地址{state.Position}，定位速度：{state.Speed}");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            var t = state.Position - AxisStates[state.Axis][0];
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = t;
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], state.Axis, (int)t, 0));
                            Thread.Sleep(50);
                        }
                        Task.Factory.StartNew(() =>
                        {
                            do
                            {
                                Thread.Sleep(20);
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;

                            } while (AxisStates[state.Axis][4] == 0);
                            do
                            {
                                if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_success_pulse(Card_Number[0], state.Axis) != 1);

                            stopwatch.Stop();
                            if (AxisStates[state.Axis][7] == 0)
                            {

                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位到位完成（{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                            else
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位等待到位超时（{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位等待到位超时（{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴相对定位复位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴相对定位复位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }
        public override void MoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴未上使能！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴处于停止中！");
                    else
                        throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴处于停止中！");
                    return;
                }
                if (axis < Axis.Length && AxisStates[axis][4] == 1 && AxisStates[axis][5] == 4)
                {
                    MoveState state;
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        lock (Motion_Lok[axis])
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴相对定位开始启动，定位地址{position}，定位速度：{speed}");
                            state = new MoveState()
                            {
                                Axis = axis,
                                CurrentPosition = AxisStates[axis][0],
                                Speed = speed,
                                Position = AxisStates[axis][0] + position,
                                Movetype = 2,
                                OutTime = time,
                                Handle = DateTime.Now,
                            };
                            if (IMoveStateQueue.Exists(e => e.Axis == axis))
                            {
                                var colose = IMoveStateQueue.Find(e => e.Axis == axis);
                                IMoveStateQueue.Remove(colose);
                            }
                            IMoveStateQueue.Add(state);
                            stopwatch.Restart();
                            TargetLocation[state.Axis] = state.Position;
                            CardErrorMessage(LTDMC.dmc_set_profile(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove(Card_Number[0], axis, (int)position, 0));
                            Thread.Sleep(50);
                        }
                        Task.Factory.StartNew(() =>
                        {
                            do
                            {
                                Thread.Sleep(20);
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;

                            } while (AxisStates[axis][4] == 0);
                            do
                            {
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_success_pulse(Card_Number[0], axis) != 1);
                            stopwatch.Stop();
                            if (AxisStates[axis][7] == 0)
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位到位完成 （{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                            else
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位外部异常停止！（{stopwatch.Elapsed}）");
                                else
                                    throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位外部异常停止! （{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                            else
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴相对定位线程异常停止！({stopwatch.Elapsed})");
                        else
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴相对定位线程异常停止！({stopwatch.Elapsed})");
                        return;
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
        }

        public override void MoveReset(ushort axis)
        {
            if (IsOpenCard)
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
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                else
                    throw new Exception($"请先调用OpenCard方法！");
            }
        }

        public override bool OpenCard(ushort card_number)
        {
            throw new NotImplementedException();
        }

        public override bool OpenCard()
        {
            lock (this)
            {
                var cardid = LTDMC.dmc_board_init();
                Card_Number = new ushort[cardid];
                Stop_sign = new bool[cardid];
                if (Card_Number.Length > 0)
                {
                    ushort _num = 0;
                    ushort[] cardids = new ushort[cardid];
                    uint[] cardtypes = new uint[cardid];
                    LTDMC.nmc_set_connect_state(0, 3, 1, 0);
                    short res = LTDMC.dmc_get_CardInfList(ref _num, cardtypes, cardids);
                    Card_Number = cardids;
                    uint totalaxis = 0;
                    CardErrorMessage(LTDMC.dmc_get_total_axes(Card_Number[0], ref totalaxis));
                    IO_Input = new bool[80];
                    IO_Output = new bool[80];
                    Axisquantity = (int)totalaxis;
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
                        LTDMC.dmc_set_factor_error(Card_Number[0], Axis[i], 1, FactorValue);
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
        }

        public override void ResetCard(ushort card, ushort reset)
        {
            LTDMC.dmc_board_reset();
            Thread.Sleep(5000);
        }

        public override void SetAxis_iniFile(string path = "AXIS.ini")
        {
            LTDMC.dmc_download_configfile(Card_Number[0], path);
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
                                    StopPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[estop])
                            {
                                if (!IO_Input[estop])
                                {
                                    StopNEvent?.Invoke(new DateTime());
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

        /// <summary>
        /// 设置数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输出点位</param>
        /// <param name="value">输出值</param>
        public override void Set_IOoutput(ushort card, ushort indexes, bool value)
        {
            if (IsOpenCard)
            {

                if (IO_Output != null)
                {
                    if (LevelSignal)
                        value = !value;
                    lock (this)
                    {
                        if (indexes >= 0 && indexes < 16)
                        {
                        CardErrorMessage(LTDMC.dmc_write_outbit(Card_Number[card], indexes, Convert.ToUInt16(!value)));
                    }
                        else if (indexes >= 16 && indexes < 32)
                        {
                            LTDMC.nmc_write_outbit(Card_Number[card], 1, (ushort)(indexes - 16), Convert.ToUInt16(!value));
                        }
                        else if (indexes >= 32 && indexes < 80)
                        {
                            LTDMC.nmc_write_outbit(Card_Number[card], 2, (ushort)(indexes - 32), Convert.ToUInt16(!value));
                        }
                    }
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"设置输出口{indexes}，状态{!value}");
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
        }

        public override void WaitAxis(int[] axis)
        {
            if (IsOpenCard)
            {
                foreach (var item in axis)
                {
                    do
                    {
                        Thread.Sleep(50);
                    } while (AxisStates[item][4] != 1 && AxisStates[item][6] != 0);
                }
            }
        }

        public override void Set_DA(ushort card, ushort channel_number, double voltage_values)
        {
            if (IsOpenCard)
            {
                if (voltage_values == 0)
                    LTDMC.dmc_set_encoder(0, channel_number, 0);
                if (channel_number < 2)
                {
                    CardErrorMessage(LTDMC.dmc_set_da_output(card, channel_number, voltage_values));
                }
                else
                {
                    if (channel_number == 2)
                    {
                        LTDMC.nmc_set_da_output(card, 3, 0, voltage_values);
                    }
                    else if (channel_number == 3)
                    {
                        LTDMC.nmc_set_da_output(card, 3, 1, voltage_values);
                    }
                }
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"设置DA通道：{channel_number}输出电压：{voltage_values}!");
                else
                    throw new Exception("未查找到板卡!");
            }
        }

        public override double Read_DA(ushort card, ushort channel_number)
        {
            double value = 0;
            if (IsOpenCard)
            {
                if (channel_number < 2)
                {
                    LTDMC.dmc_get_da_output(0, channel_number, ref value);
                }
                else
                {
                    if (channel_number == 2)
                    {
                        LTDMC.nmc_get_da_output(card, 3, (ushort)(channel_number - 2), ref value);
                    }
                    else if (channel_number == 3)
                    {
                        LTDMC.nmc_get_da_output(card, 3, (ushort)(channel_number - 2), ref value);
                    }
                }
            }
            return value;
        }

        public override double Read_AD(ushort card, ushort channel_number)
        {
            double value = 0;
            if (IsOpenCard)
            {
                LTDMC.nmc_get_ad_input(card, 3, channel_number, ref value);
            }
            return value;
        }

        public override void Deploy_CAN(ushort card, ushort can_num, bool can_state, ushort can_baud = 0)
        {
            if (IsOpenCard)
            {
                LTDMC.dmc_set_da_enable(0, 1);
                bool a = CardErrorMessage(LTDMC.nmc_set_connect_state(card, can_num, (ushort)(can_state == false ? 0 : 1), can_baud));
                LTDMC.nmc_set_da_mode(0, 3, 0, 0, 0);
                LTDMC.nmc_set_da_mode(0, 3, 1, 0, 0);
                for (int i = 0; i < 4; i++)
                {
                    LTDMC.nmc_set_ad_mode(0, 3, (ushort)i, 0, 0);
                }
                if (a)
                {
                    if (can_state)
                    {
                        CAN_IsOpen = true;
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"打开CAN扩展通讯，CAN节点数：{can_num}!");
                        else
                            throw new Exception("未查找到板卡!");
                    }
                    else
                    {
                        CAN_IsOpen = false;
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"关闭CAN扩展通讯!");
                        else
                            throw new Exception("未查找到板卡!");
            }
        }
            }
        }

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster, ushort entrance)
        {
            Special_io = new bool[80];
            Task.Run(() =>
            {
                while (true)
                {
                    lock (this)
                    {
                        Thread.Sleep(50);
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

                            if (!Special_io[raster])//光栅
                            {
                                if (IO_Input[raster])
                                {
                                    RasterPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[raster])
                            {
                                if (!IO_Input[raster])
                                {
                                    RasterNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[raster] = IO_Input[raster];

                            AutoReadEvent.WaitOne();


                            if (!Special_io[entrance])//门禁
                            {
                                if (IO_Input[entrance])
                                {
                                    EntrancePEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[entrance])
                            {
                                if (!IO_Input[entrance])
                                {
                                    EntranceNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[entrance] = IO_Input[entrance];
                            AutoReadEvent.WaitOne();
                        }
                    }
                }
            });
        }

        public override void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster1, ushort raster2, ushort entrance1, ushort entrance2)
        {
            Special_io = new bool[IO_Input.Length];
            for (int i = 0; i < IO_Input.Length; i++)
        {
                Special_io[i] = IO_Input[i];
            }
            Task.Run(() =>
            {
                while (true)
                {
                    lock (this)
                    {
                        Thread.Sleep(50);
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

                            if (!Special_io[raster2])//光栅
                            {
                                if (IO_Input[raster2])
                                {
                                    RasterPEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[raster2])
                            {
                                if (!IO_Input[raster2])
                                {
                                    RasterNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[raster2] = IO_Input[raster2];
                            AutoReadEvent.WaitOne();

                            if (!Special_io[entrance1])//门禁1
                            {
                                if (IO_Input[entrance1])
                                {
                                    EntrancePEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[entrance1])
                            {
                                if (!IO_Input[entrance1])
                                {
                                    EntranceNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[entrance1] = IO_Input[entrance1];
                            AutoReadEvent.WaitOne();
                            if (!Special_io[entrance2])//门禁1
                            {
                                if (IO_Input[entrance2])
                                {
                                    EntrancePEvent?.Invoke(new DateTime());
                                }
                            }
                            if (Special_io[entrance2])
                            {
                                if (!IO_Input[entrance2])
                                {
                                    EntranceNEvent?.Invoke(new DateTime());
                                }
                            }
                            Special_io[entrance2] = IO_Input[entrance2];
                            AutoReadEvent.WaitOne();
                        }
                    }
                }
            });
        }

        public override void Set_IOoutput_Enum(ushort card, OutPut indexes_enm, bool value)
        {
            if (IsOpenCard)
            {
                ushort indexes = Convert.ToUInt16(indexes_enm);
                if (IO_Output != null)
                {
                    if (LevelSignal)
                        value = !value;
                    lock (this)
                    {
                        if (indexes >= 0 && indexes < 16)
                        {
                            CardErrorMessage(LTDMC.dmc_write_outbit(Card_Number[card], indexes, Convert.ToUInt16(!value)));
                        }
                        else if (indexes >= 16 && indexes < 32)
                        {
                            LTDMC.nmc_write_outbit(Card_Number[card], 1, (ushort)(indexes - 16), Convert.ToUInt16(!value));
                        }
                        else if (indexes >= 32 && indexes < 80)
                        {
                            LTDMC.nmc_write_outbit(Card_Number[card], 2, (ushort)(indexes - 32), Convert.ToUInt16(!value));
                        }
                    }
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"设置输出口{indexes}，状态{!value}");
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
        }

        public override void AwaitIOinput_Enum(ushort card, InPuts indexes_enm, bool waitvalue, int timeout = 0)
        {
           if (IsOpenCard)
            {
                ushort indexes = Convert.ToUInt16(indexes_enm);
                if (IO_Input != null)
                {
                    if (LevelSignal)
                        waitvalue = !waitvalue;
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        stopwatch.Restart();
                        do
                        {
                            Thread.Sleep(20);
                            if (timeout != 0 && stopwatch.Elapsed.TotalMilliseconds > timeout)
                                goto Timeout;

                        } while (IO_Input[indexes] != waitvalue);
                        stopwatch.Stop();
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"等待输入口{indexes}，状态{waitvalue}完成（{stopwatch.Elapsed}）");
                        return;
                    Timeout:
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"等待输入口{indexes}，状态{waitvalue}超时（{stopwatch.Elapsed}）");
                        throw new Exception($"等待输入口{indexes}，等待状态{waitvalue}超时（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"等待输入口{indexes}，状态{waitvalue}线程异常中断（{stopwatch.Elapsed}）");
                        else
                            throw new Exception($"等待输入口{indexes}，等待状态{waitvalue}线程异常中断（{stopwatch.Elapsed}）");
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }
    }
}
