using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotionClass
{
    /// <summary>
    /// 雷赛板卡实现类
    /// </summary>
    public sealed class LeiSaiEtherCAT : MotionBase
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
        /// 数字io输入
        /// </summary>
        public override bool[] IO_Input { get; set; }
        /// <summary>
        /// 数字io输出
        /// </summary>
        public override bool[] IO_Output { get; set; }
        /// <summary>
        /// 板卡号
        /// </summary>
        public override ushort[] Card_Number { get; set; }
        /// <summary>
        /// 轴号
        /// </summary>
        public override ushort[] Axis { get; set; }
        /// <summary>
        /// 总线轴总数
        /// </summary>
        public override int Axisquantity { get; set; }
        /// <summary>
        /// 到位误差
        /// </summary>
        public override ushort FactorValue { get; set; }
        /// <summary>
        /// 定位速度
        /// </summary>
        public override double Speed { get; set; }
        /// <summary>
        /// 加速度
        /// </summary>
        public override double Acc { get; set; }
        /// <summary>
        /// 减速度
        /// </summary>
        public override double Dec { get; set; }
        /// <summary>
        /// 特殊IO
        /// </summary>
        public bool[] Special_io { get; set; }

        /// <summary>
        /// 数据读取后台线程
        /// </summary>
        public override Thread[] Read_ThreadPool { get; set; }

        /// <summary>
        /// 轴状态信息获取 double[][] 一维索引代表轴号，二维索引注释如下
        ///<para>double[][0]= 脉冲位置</para>
        ///<para>double[][1]= 伺服编码器位置</para>
        ///<para>double[][2]= 目标位置</para>
        ///<para>double[][3]= 速度</para>
        ///<para>double[][4]= 轴运动到位 0=运动中 1=轴停止</para>
        ///<para>double[][5]= 轴状态机0：轴处于未启动状态 1：轴处于启动禁止状态 2：轴处于准备启动状态 3：轴处于启动状态 4：轴处于操作使能状态 5：轴处于停止状态 6：轴处于错误触发状态 7：轴处于错误状态</para>
        ///<para>double[][6]= 轴运行模式0：空闲 1：Pmove 2：Vmove 3：Hmove 4：Handwheel 5：Ptt / Pts 6：Pvt / Pvts 10：Continue</para>
        ///<para>double[][7]= 轴停止原因获取0：正常停止  3：LTC 外部触发立即停止  4：EMG 立即停止  5：正硬限位立即停止  6：负硬限位立即停止  7：正硬限位减速停止  8：负硬限位减速停止  9：正软限位立即停止 10：负软限位立即停止11：正软限位减速停止  12：负软限位减速停止  13：命令立即停止  14：命令减速停止  15：其它原因立即停止  16：其它原因减速停止  17：未知原因立即停止  18：未知原因减速停止</para>
        /// </summary>
        public override double[][] AxisStates { get; set; }

        /// <summary>
        /// 数据读取线程管理
        /// </summary>
        public override ManualResetEvent AutoReadEvent { get; set; }

        /// <summary>
        /// 轴定位状态集合
        /// </summary>
        public override List<MoveState> IMoveStateQueue { get; set; }

        /// <summary>
        /// 总线状态数组
        /// <para>int[0]==总线扫描周期</para>
        /// <para>int[1]==总线状态，Value=0为总线正常</para>
        /// </summary>
        public override int[] EtherCATStates { get; set; }

        /// <summary>
        /// 轴专用IO int[][] 一维索引代表轴号，二维索引注释如下
        ///<para>int[][0]=伺服报警</para> 
        ///<para>int[][1]=正限位</para> 
        ///<para>int[][2]=负限位</para> 
        ///<para>int[][3]=急停</para> 
        ///<para>int[][4]=原点</para> 
        ///<para>int[][5]=正软限位</para> 
        ///<para>int[][6]=负软限位</para> 
        /// </summary>
        public override int[][] Axis_IO { get; set; }

        /// <summary>
        /// 插补坐标系状态
        /// <para>short[0] 一维索引为坐标系号</para>
        /// <para>short[]= 0 坐标系运动中</para>
        /// <para>short[]= 1 暂停中</para>
        /// <para>short[]= 2 正常停止</para>
        /// <para>short[]= 3 已被占用但未启动</para>
        /// <para>short[]= 4 坐标系空闲</para>
        /// </summary>
        public override short[] CoordinateSystemStates { get; set; } = new short[5];

        /// <summary>
        /// 板卡是否打开
        /// </summary>
        public override bool IsOpenCard { get; set; }
        /// <summary>
        /// 运动控制类方法内部Taks线程令牌
        /// </summary>
        public override CancellationTokenSource[] Task_Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override CancellationToken[] cancellation_Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool CAN_IsOpen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double[] ADC_RealTime_DA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override double[] ADC_RealTime_AD { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// 板卡运行日志事件
        /// </summary>
        public override event Action<DateTime, bool, string> CardLogEvent;

        public LeiSaiEtherCAT()
        {
            Read_ThreadPool = new Thread[2];
            AutoReadEvent = new ManualResetEvent(true);
            Read_ThreadPool[0] = new Thread(ReadState);
            Read_ThreadPool[1] = new Thread(ReadIO);
            IMoveStateQueue = new List<MoveState>();
            MotionBase.Thismotion = this;
        }

        /// <summary>
        /// 单个轴使能
        /// </summary>
        /// <param name="card">卡号</param>
        /// <param name="axis">轴号</param>
        /// <returns></returns>
        public override void AxisOn(ushort card, ushort axis)
        {
            if (IsOpenCard)
            {
                if (card < Card_Number.Length)
                {
                    if (axis < Axis.Length)
                    {
                        Stop_sign[axis] = false;
                        LTDMC.nmc_set_axis_enable(Card_Number[card], axis);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, false, $"{card}号板卡{axis}号轴上使能");
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, "指定轴号错误");
                        throw new Exception("指定轴号错误");
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, "指定卡号错误");
                    throw new Exception("指定卡号错误");
                }
            }
            else
            {
                throw new Exception("请先初始化板卡再使能轴！");
            }

        }

        /// <summary>
        /// 所有轴使能
        /// </summary>
        /// <returns></returns>
        public override void AxisOn()
        {
            if (IsOpenCard)
            {
                for (int i = 0; i < Card_Number.Length; i++)
                {
                    LTDMC.nmc_set_axis_enable(Card_Number[i], 255);

                    for (ushort j = 0; j < Axis.Length; j++)
                    {
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
                throw new Exception("请先初始化板卡再使能轴！");
            }
        }

        /// <summary>
        /// 单轴下使能
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="axis">轴号</param>
        public override void AxisOff(ushort card, ushort axis)
        {
            if (IsOpenCard)
            {
                if (axis < Axisquantity)
                {
                    LTDMC.nmc_set_axis_disable(Card_Number[card], axis);
                    AxisStop(Axis[axis], 0, false);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"{axis}轴上使能");
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"轴号输入错误！");
                    throw new Exception("轴号输入错误！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先初始化板卡再使能轴！");
                throw new Exception("请先初始化板卡再使能轴！");

            }
        }

        /// <summary>
        /// 所有轴下使能
        /// </summary>
        public override void AxisOff()
        {
            if (IsOpenCard)
            {
                for (int i = 0; i < Card_Number.Length; i++)
                {
                    LTDMC.nmc_set_axis_disable(Card_Number[i], 255);
                }
                AxisStop(Axis[0], 0, true);
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"所有轴下使能");
            }
            else
            {

                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先初始化板卡再下使能轴");
                throw new Exception("请先初始化板卡再下使能轴");
            }
        }

        /// <summary>
        /// 轴基础参数设置
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="equiv">脉冲当量</param>
        /// <param name="startvel">起始速度</param>
        /// <param name="speed">运行速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="stopvel">停止速度</param>
        /// <param name="s_para">S段时间</param>
        /// <param name="posi_mode">运动模式 0：相对坐标模式，1：绝对坐标模式</param>
        /// <param name="stop_mode">制动方式 0：减速停止，1：紧急停止</param>
        public override void AxisBasicSet(ushort axis, double equiv, double startvel, double speed, double acc, double dec, double stopvel, double s_para, int posi_mode, int stop_mode)
        {
            if (IsOpenCard)
            {
                if (axis < Axis.Length)
                {
                    if (axis < 8)
                    {
                        Acc = acc;
                        Dec = dec;
                        Speed = speed;
                        CardErrorMessage(LTDMC.dmc_set_equiv(Card_Number[0], axis, equiv));  //设置脉冲当量
                        CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], axis, startvel, speed, acc, dec, stopvel));//设置速度参数
                        CardErrorMessage(LTDMC.dmc_set_s_profile(Card_Number[0], axis, 0, s_para));//设置S段速度参数
                        CardErrorMessage(LTDMC.dmc_stop(Card_Number[0], axis, (ushort)stop_mode));//制动方式
                        CardErrorMessage(LTDMC.dmc_set_dec_stop_time(Card_Number[0], axis, posi_mode)); //设置减速停止时间
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"轴号输入错误！");
                    throw new Exception($"轴号输入错误！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        /// <summary>
        /// 打开指定板卡
        /// </summary>
        /// <param name="card_number">板卡号</param>
        /// <returns></returns>
        public override bool OpenCard(ushort card_number)
        {
            lock (this)
            {
                var cardid = LTDMC.dmc_board_init();
                Card_Number = new ushort[cardid];
                if (Card_Number.Length > 0)
                {
                    uint totalaxis = 0;
                    for (int i = 0; i < Card_Number.Length; i++)
                    {
                        CardErrorMessage(LTDMC.nmc_get_total_axes((ushort)i, ref totalaxis));
                        Axisquantity += (int)totalaxis;
                    }
                    IsOpenCard = true;
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"初始化{card_number}号板卡，轴数量为{Axisquantity}");
                    return true;
                }
                else
                {
                    IsOpenCard = false;
                    if (Card_Number.Length == 0)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, "未查找到板卡!");
                        throw new Exception("未查找到板卡!");
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, "有 2 张或 2 张以上控制卡的硬件设置卡号相同!");
                        throw new Exception("有 2 张或 2 张以上控制卡的硬件设置卡号相同!");
                    }
                }
            }
        }

        /// <summary>
        /// 打开所有板卡
        /// </summary>
        /// <returns></returns>
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

                    short res = LTDMC.dmc_get_CardInfList(ref _num, cardtypes, cardids);
                    Card_Number = cardids;
                    uint totalaxis = 0;
                    ushort input = 0;
                    ushort output = 0;
                    CardErrorMessage(LTDMC.nmc_get_total_axes(Card_Number[0], ref totalaxis));
                    CardErrorMessage(LTDMC.nmc_get_total_ionum(Card_Number[0], ref input, ref output));
                    IO_Input = new bool[input + 8];
                    IO_Output = new bool[output + 8];
                    Axisquantity = (int)totalaxis;
                    Motion_Lok = new object[Axisquantity];
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"初始化{Card_Number}张板卡，总轴数为{Axisquantity}");
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
                    return true;
                }
                else
                {
                    IsOpenCard = false;
                    if (Card_Number.Length == 0)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, "未查找到板卡!");
                        throw new Exception("未查找到板卡!");
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, "有 2 张或 2 张以上控制卡的硬件设置卡号相同!");
                        throw new Exception("有 2 张或 2 张以上控制卡的硬件设置卡号相同!");
                    }
                }
            }
        }

        /// <summary>
        /// 释放雷赛控制卡
        /// </summary>
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

        /// <summary>
        /// 单轴JOG运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="speed">运行速度</param>
        /// <param name="posi_mode">运动方向，0：负方向，1：正方向</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        public override void MoveJog(ushort axis, double speed, int posi_mode, double acc = 0.5, double dec = 0.5)
        {
            if (IsOpenCard)
            {
                if (axis < Axis.Length)
                {
                    lock (Motion_Lok[axis])
                    {
                        if (AxisStates[axis][4] == 1 && AxisStates[axis][6] == 0 && !Stop_sign[axis])
                        {
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], axis, 0, speed, acc, dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_vmove(Card_Number[0], axis, (ushort)posi_mode));
                        }
                        else if (Stop_sign[axis])
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"轴{axis}停止中，请复位轴状态！");
                            throw new Exception($"轴{axis}停止中，请复位轴状态！");
                        }
                    }
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"轴{axis}进行JOG运动");
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"轴号输入错误！");
                    throw new Exception($"轴号输入错误！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="stop_mode">停止方式 0=减速停止 1=紧急停止</param>
        /// <param name="all">是否全部轴停止</param>
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
                                CardLogEvent(DateTime.Now, false, $"全部轴减速停止！");
                        }
                        else
                        {
                            Stop_sign[axis] = true;
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"全部轴紧急停止！");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Axis.Length; i++)
                        {
                            Stop_sign[i] = true;
                            CardErrorMessage(LTDMC.dmc_emg_stop(Card_Number[0]));
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
                    throw new Exception($"轴号输入错误！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        private void MoveAbs(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    throw new Exception($"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    throw new Exception($"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴绝对定位复位启动错误！ {state.Axis}轴处于停止中！");
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
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}单轴绝对定位复位开始启动！，定位地址{state.Position}，定位速度：{state.Speed}");
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], state.Axis));
                            stopwatch.Restart();
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], state.Axis, state.Position, 1));
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
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位等待到位超时（{stopwatch.Elapsed}）");
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴绝对定位复位等待到位超时（{stopwatch.Elapsed}）");
                        });

                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴绝对定位复位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴绝对定位复位线程异常停止！({stopwatch.Elapsed})");
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

        private void MoveRel(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴在运动中！");
                    throw new Exception($"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴未上使能！");
                    throw new Exception($"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴相对定位复位启动错误！ {state.Axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], state.Axis, t, 0));
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
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位等待到位超时（{stopwatch.Elapsed}）");
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位复位等待到位超时（{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴相对定位复位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴相对定位复位线程异常停止！({stopwatch.Elapsed})");
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

        private void AwaitMoveAbs(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    throw new Exception($"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    throw new Exception($"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞绝对定位复位启动错误！ {state.Axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], state.Axis, state.Position, 1));
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
                        } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
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
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位外部异常停止！（{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(state.Axis);
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                        throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
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

                            var t = state.Position - AxisStates[state.Axis][0];
                            stopwatch.Restart();
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], state.Axis, 0, state.Speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], state.Axis, t, 0));
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
                        } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
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

        private void MoveHome(MoveState state)
        {
            if (IsOpenCard)
            {
                if (AxisStates[state.Axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴在运动中！");
                    throw new Exception($"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴未上使能！");
                    throw new Exception($"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴原点回归复位启动错误！ {state.Axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.nmc_set_home_profile(Card_Number[0], state.Axis, state.HomeModel, state.Speed / 2, state.Speed, state.ACC, state.Dcc, state.Home_off));
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
                            } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
                            stopwatch.Stop();
                            if (AxisStates[state.Axis][0] < FactorValue && AxisStates[state.Axis][0] > -FactorValue && LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) == 1)
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{state.Axis}轴原点回归复位完成，零点误差：{AxisStates[state.Axis][0]} （{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                LTDMC.dmc_set_position_unit(Card_Number[0], state.Axis, 0);
                                LTDMC.dmc_set_encoder_unit(Card_Number[0], state.Axis, 0);
                                return;
                            }
                            else
                            {
                                if (AxisStates[state.Axis][AxisStates[state.Axis].Length - 1] != 0)
                                {
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, true, $"{state.Axis}轴原点回归复位外部异常停止！（{stopwatch.Elapsed}）");
                                    throw new Exception($"{state.Axis}轴原点回归复位外部异常停止！（{stopwatch.Elapsed}）");
                                    return;
                                }
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.Axis}轴到位编码器误差过大！（{stopwatch.Elapsed}）");
                                throw new Exception($"{state.Axis}轴到位编码器误差过大（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴原点回归复位超时停止！（{stopwatch.Elapsed}）");
                            throw new Exception($"{state.Axis}轴原点回归复位超时停止！（{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴原点回归复位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴原点回归复位线程异常停止！({stopwatch.Elapsed})");
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
                    throw new Exception($"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴在运动中！");
                    return;
                }
                if (AxisStates[state.Axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴未上使能！");
                    throw new Exception($"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[state.Axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.Axis}单轴阻塞原点回归复位错误！ {state.Axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.nmc_set_home_profile(Card_Number[0], state.Axis, state.HomeModel, state.Speed / 2, state.Speed, state.ACC, state.Dcc, state.Home_off));

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
                        } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
                        stopwatch.Stop();
                        if (AxisStates[state.Axis][0] < FactorValue && AxisStates[state.Axis][0] > -FactorValue && LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) == 1)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{state.Axis}轴阻塞原点回归复位完成，零点误差：{AxisStates[state.Axis][0]} （{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            LTDMC.dmc_set_position_unit(Card_Number[0], state.Axis, 0);
                            LTDMC.dmc_set_encoder_unit(Card_Number[0], state.Axis, 0);
                            return;
                        }
                        else
                        {
                            if (AxisStates[state.Axis][AxisStates[state.Axis].Length - 1] != 0)
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.Axis}轴阻塞原点回归复位外部异常停止！（{stopwatch.Elapsed}）");
                                throw new Exception($"{state.Axis}轴阻塞原点回归复位外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴阻塞原点回归复位异常停止，到位编码器误差过大！（{stopwatch.Elapsed}）");
                            throw new Exception($"{state.Axis}轴阻塞原点回归异常复位停止，到位编码器误差过大！（{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(state.Axis);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.Axis}轴阻塞原点回归复位超时停止！（{stopwatch.Elapsed}）");
                        throw new Exception($"{state.Axis}轴阻塞原点回归复位超时停止！（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], state.Axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴阻塞原点回归复位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.Axis}轴阻塞原点回归复位线程异常停止！({stopwatch.Elapsed})");
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
                        throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴未上使能！");
                        throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[state.Axis])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在停止中！");
                        throw new Exception($"{state.UsingAxisNumber}轴直线插补启动错误！ {item}轴在停止中！");
                        return;
                    }
                    CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], item));
                }
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
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
                                double[] pos = new double[state.UsingAxisNumber];
                                axis = state.Axises;
                                ushort movtype = 0;
                                if (state.Movetype == 7)
                                {
                                    movtype = 0;
                                    for (int i = 0; i < state.UsingAxisNumber; i++)
                                    {
                                        pos[i] = state.Positions[i] - AxisStates[state.Axises[i]][0];
                                    }
                                }
                                else
                                {
                                    movtype = 1;
                                    for (int i = 0; i < state.UsingAxisNumber; i++)
                                    {
                                        pos[i] = state.Positions[i];
                                    }
                                }
                                stopwatch.Restart();
                                CardErrorMessage(LTDMC.dmc_set_vector_profile_unit(Card_Number[state.CardID], Convert.ToUInt16(coordinate), 0, state.Speed, state.ACC, state.Dcc, 0));
                                CardErrorMessage(LTDMC.dmc_line_unit(Card_Number[state.CardID], Convert.ToUInt16(coordinate), state.UsingAxisNumber, axis, pos, movtype));
                                Thread.Sleep(50);
                            }
                            Task.Factory.StartNew(() =>
                            {
                                do
                                {
                                    Thread.Sleep(20);
                                    if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                        goto Timeout;
                                } while (LTDMC.dmc_check_done_multicoor(Card_Number[state.CardID], Convert.ToUInt16(coordinate)) == 0);
                                do
                                {
                                    if (state.OutTime != 0 && stopwatch.Elapsed.TotalMilliseconds > state.OutTime)
                                        goto Timeout;
                                } while (CoordinateSystemStates[coordinate] != 4);
                                if (CoordinateSystemStates[coordinate] == 4)
                                {
                                    foreach (var item in state.Axises)
                                    {
                                        do
                                        {
                                            Thread.Sleep(20);
                                        } while (AxisStates[item][4] != 1);
                                        if (AxisStates[item][7] != 0)
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
                                throw new Exception($"{state.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                            Stop:
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                                throw new Exception($"{state.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            });
                        }
                        catch (ThreadAbortException ex)
                        {
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[state.CardID], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{state.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{state.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        throw new Exception($"{state.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{state.UsingAxisNumber}插补坐标系忙碌！");
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

        /// <summary>
        /// 单轴绝对定位（非阻塞模式，调用该方法后需要自行处理是否运动完成）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">定位地址</param>
        /// <param name="speed">定位速度</param>
        ///  <param name="time">超时时间</param>
        public override void MoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                    throw new Exception($"{axis}单轴绝对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴未上使能！");
                    throw new Exception($"{axis}单轴绝对定位启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴绝对定位启动错误！ {axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], axis, position, 1));
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
                            } while (LTDMC.dmc_check_success_encoder(Card_Number[0], axis) != 1);
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
                                throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位外部异常停止! （{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                            throw new Exception($"{axis}轴定位地址{position}，单轴绝对定位等待到位超时 （{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴绝对定位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴绝对定位线程异常停止！({stopwatch.Elapsed})");
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

        /// <summary>
        /// 单轴相对定位（非阻塞模式，调用该方法后需要自行处理是否运动完成）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">定位地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">超时时间</param>
        public override void MoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                    throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴未上使能！");
                    throw new Exception($"{axis}单轴相对定位启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴相对定位启动错误！ {axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], axis, position, 0));
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
                            } while (LTDMC.dmc_check_success_encoder(Card_Number[0], axis) != 1);
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
                                throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位外部异常停止! （{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(state.Axis);
                            Console.WriteLine(stopwatch.Elapsed);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{state.Axis}轴定位地址{state.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                            throw new Exception($"{state.Axis}轴定位地址{state.Position}，单轴相对定位等待到位超时 （{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴相对定位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴相对定位线程异常停止！({stopwatch.Elapsed})");
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
        ///double[7]= 轴停止原因获取0：正常停止 1：ALM 立即停止  2：ALM 减速停止  3：LTC 外部触发立即停止  4：EMG 立即停止  5：正硬限位立即停止  6：负硬限位立即停止  7：正硬限位减速停止  8：负硬限位减速停止  9：正软限位立即停止  
        ///10：负软限位立即停止11：正软限位减速停止  12：负软限位减速停止  13：命令立即停止  14：命令减速停止  15：其它原因立即停止  16：其它原因减速停止  17：未知原因立即停止  18：未知原因减速停止
        /// </returns>
        public override double[] GetAxisState(ushort axis)
        {
            ushort[] state = new ushort[2];
            double[] doubles = new double[8];
            if (IsOpenCard)
            {
                int a = 0;
                LTDMC.dmc_get_position_unit(Card_Number[0], axis, ref doubles[0]); //脉冲位置
                LTDMC.dmc_get_encoder_unit(Card_Number[0], axis, ref doubles[1]);//编码器
                LTDMC.dmc_get_target_position_unit(Card_Number[0], axis, ref doubles[2]);//目标位置
                LTDMC.dmc_read_current_speed_unit(Card_Number[0], axis, ref doubles[3]);//速度
                doubles[4] = LTDMC.dmc_check_done(Card_Number[0], axis);//轴运动到位 0=运动中 1=轴停止
                LTDMC.nmc_get_axis_state_machine(Card_Number[0], axis, ref state[0]);//轴状态机：0：轴处于未启动状态 1：轴处于启动禁止状态 2：轴处于准备启动状态 3：轴处于启动状态 4：轴处于操作使能状态 5：轴处于停止状态 6：轴处于错误触发状态 7：轴处于错误状态
                LTDMC.dmc_get_axis_run_mode(Card_Number[0], axis, ref state[1]);//轴运行模式：0：空闲 1：定位模式 2：定速模式 3：回零模式 4：手轮模式 5：Ptt / Pts 6：Pvt / Pvts 10：Continue
                LTDMC.dmc_get_stop_reason(Card_Number[0], axis, ref a);//轴停止原因获取：0：正常停止 1：ALM 立即停止 2：ALM 减速停止  3：LTC 外部触发立即停止，IMD_STOP_AT_LTC 4：EMG 立即停止，IMD_STOP_AT_EMG 5：正硬限位立即停止，IMD_STOP_AT_ELP6：负硬限位立即停止，IMD_STOP_AT_ELN7：正硬限位减速停止，DEC_STOP_AT_ELP8：负硬限位减速停止，DEC_STOP_AT_ELN9：正软限位立即停止，IMD_STOP_AT_SOFT_ELP10：负软限位立即停止，IMD_STOP_AT_SOFT_ELN11：正软限位减速停止，DEC_STOP_AT_SOFT_ELP12：负软限位减速停止，DEC_STOP_AT_SOFT_ELN13：命令立即停止，IMD_STOP_AT_CMD14：命令减速停止，DEC_STOP_AT_CMD15：其它原因立即停止，IMD_STOP_AT_OTHER16：其它原因减速停止，DEC_STOP_AT_OTHER17：未知原因立即停止，IMD_STOP_AT_UNKOWN18：未知原因减速停止，DEC_STOP_AT_UNKOWN     
                Array.Copy(state, 0, doubles, 5, 2);
                doubles[7] = a;
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
            return doubles;
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
                throw new Exception($"请先调用OpenCard方法！");
            }
            return bools;
        }

        /// <summary>
        /// 复位轴停止前定位动作
        /// </summary>
        /// <param name="axis">轴号</param>
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
                            case 9: MoveLines(item); break;
                            case 10: MoveLines(item); break;
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
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        /// <summary>
        /// 数据刷新后台线程
        /// </summary>
        private void ReadState()
        {
            AxisStates = new double[Axis.Length][];
            Axis_IO = new int[Axis.Length][];
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Restart();
                Thread.Sleep(10);
                EtherCATStates = GetEtherCATState(0);
                for (ushort i = 0; i < Axis.Length; i++)
                {
                    AxisStates[i] = GetAxisState(i);
                    Axis_IO[i] = GetAxisExternalio(i);
                }
                for (ushort i = 0; i < 5; i++)
                {
                    CoordinateSystemStates[i] = LTDMC.dmc_conti_get_run_state(Card_Number[0], i);
                }
                stopwatch.Stop();
                Console.WriteLine("State=>" + stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        private void ReadIO()
        {
            AxisStates = new double[Axis.Length][];
            Axis_IO = new int[Axis.Length][];
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Restart();
                Thread.Sleep(10);
                for (ushort i = 0; i < Card_Number.Length; i++)
                {
                    IO_Input = Getall_IOinput(i);
                    IO_Output = Getall_IOoutput(i);
                }
                stopwatch.Stop();
                Console.WriteLine("IO=>" + stopwatch.Elapsed);//数据刷新用时
                AutoReadEvent.WaitOne();
            }
        }

        /// <summary>
        /// 单轴绝对定位（阻塞模式，调用该方法后定位运动完成后或超时返回）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">绝对地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">等待超时时长：0=一直等待直到定位完成</param>
        public override void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞绝对定位启动错误！ {axis}轴在运动中！");
                    throw new Exception($"{axis}单轴阻塞绝对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞绝对定位启动错误！ {axis}轴未上使能！");
                    throw new Exception($"{axis}单轴阻塞绝对定位启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞绝对定位启动错误！ {axis}轴处于停止中！");
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
                            IMoveStateQueue.Add(state);
                            stopwatch.Restart();
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], axis, position, 1));
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
                        } while (LTDMC.dmc_check_success_encoder(Card_Number[0], axis) != 1);
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
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                        throw new Exception($"{axis}轴定位地址{position}，单轴阻塞绝对定位等待到位超时（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴阻塞绝对定位线程异常停止！({stopwatch.Elapsed})");
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

        /// <summary>
        /// 单轴相对定位（阻塞模式，调用该方法后定位运动完成后或超时返回）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">相对地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">等待超时时长：0=一直等待直到定位完成</param>
        public override void AwaitMoveRel(ushort axis, double position, double speed, int time = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞相对定位启动错误！ {axis}轴在运动中！");
                    throw new Exception($"{axis}单轴阻塞相对定位启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞相对定位启动错误！ {axis}轴未上使能！");
                    throw new Exception($"{axis}单轴阻塞相对定位启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞相对定位启动错误！ {axis}轴处于停止中！");
                    throw new Exception($"{axis}单轴阻塞相对定位启动错误！ {axis}轴处于停止中！");
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
                                CardLogEvent(DateTime.Now, false, $"{axis}单轴阻塞相对定位开始启动，定位地址{position}，定位速度：{speed}");
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
                            CardErrorMessage(LTDMC.dmc_set_profile_unit(Card_Number[0], axis, 0, speed, Acc, Dec, 0));//设置速度参数
                            CardErrorMessage(LTDMC.dmc_pmove_unit(Card_Number[0], axis, position, 0));
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
                        } while (LTDMC.dmc_check_success_encoder(Card_Number[0], axis) != 1);
                        do
                        {
                            Thread.Sleep(20);
                            if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                goto Timeout;
                        } while (AxisStates[axis][4] == 0);
                        stopwatch.Stop();
                        if (AxisStates[axis][7] == 0)
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴定位地址{position}，单轴阻塞相对定位到位完成：（{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            return;
                        }
                        else
                        {
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞相对定位外部异常停止！（{stopwatch.Elapsed}）");
                            throw new Exception($"{axis}轴定位地址{position}，单轴阻塞相对定位外部异常停止！（{stopwatch.Elapsed}）");
                            return;

                            //if (CardLogEvent != null)
                            //    CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，到位编码器误差过大! （{stopwatch.Elapsed}）");
                            //throw new Exception($"{axis}轴定位地址{position}，到位编码器误差过大! （{stopwatch.Elapsed}）");
                            //return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(axis);
                        Console.WriteLine(stopwatch.Elapsed);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{axis}轴定位地址{position}，单轴阻塞相对定位等待到位超时（{stopwatch.Elapsed}）");
                        throw new Exception($"{axis}轴定位地址{position}，单轴阻塞相对定位等待到位超时（{stopwatch.Elapsed}）");
                        return;
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}单轴阻塞相对定位线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}单轴阻塞相对定位线程异常停止！({stopwatch.Elapsed})");
                        return;

                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
                return;
            }
        }

        /// <summary>
        /// 获取板卡全部数字输入
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        public override bool[] Getall_IOinput(ushort card)
        {
            if (IsOpenCard)
            {
                if (IO_Input != null)
                {
                    var input = LTDMC.dmc_read_inport(Card_Number[card], 0);
                    for (int i = 0; i < IO_Input.Length; i++)
                    {
                        IO_Input[i] = (input & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
            return IO_Input;
        }

        /// <summary>
        /// 获取板卡全部数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        public override bool[] Getall_IOoutput(ushort card)
        {
            if (IsOpenCard)
            {
                if (IO_Output != null)
                {
                    var output = LTDMC.dmc_read_outport(Card_Number[card], 0);
                    for (int i = 0; i < IO_Output.Length; i++)
                    {
                        IO_Output[i] = (output & (1 << i)) == 0 ? !LevelSignal : LevelSignal;
                    }
                }
                else
                {
                    if (Card_Number == null)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                        throw new Exception($"请先调用OpenCard方法！");
                    }
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
            return IO_Output;

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
                        CardErrorMessage(LTDMC.dmc_write_outbit(Card_Number[card], indexes, Convert.ToUInt16(value)));
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
                        throw new Exception($"请先调用OpenCard方法！");
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

        /// <summary>
        /// 等待输入信号
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输入口</param>
        /// <param name="waitvalue">等待状态</param>
        /// <param name="timeout">等待超时时间</param>
        public override void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0)
        {
            if (IsOpenCard)
            {
                if (IO_Input != null)
                {
                    if (LevelSignal)
                        waitvalue = false;
                    else
                        waitvalue = true;
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

        /// <summary>
        /// 读取总线状态
        /// </summary>
        /// <param name="card_number">板卡号</param>
        /// <returns>int[0]=总线扫描时长us int[1]总线状态==0正常</returns>
        public override int[] GetEtherCATState(ushort card_number)
        {
            int a = 0;
            int b = 0;
            LTDMC.nmc_get_cycletime(Card_Number[card_number], 2, ref a);//总线循环周期us
            LTDMC.nmc_get_errcode(Card_Number[card_number], 2, ref b);//总线状态 0=正常
            return new int[] { a, b };
        }

        /// <summary>
        /// 运动控制卡复位
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="reset">0=热复位 1=冷复位 2=初始复位</param>
        public override void ResetCard(ushort card, ushort reset)
        {
            if (Card_Number == null)
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法后在进行板卡复位！");
                throw new Exception($"请先调用OpenCard方法后在进行板卡复位！");
            }
            if (card < Card_Number.Length)
            {
                lock (this)
                {
                    AutoReadEvent.Reset();
                    Thread.Sleep(100);
                    switch (reset)
                    {
                        case 0: LTDMC.dmc_soft_reset(Card_Number[card]); break;
                        case 1: LTDMC.dmc_cool_reset(Card_Number[card]); break;
                        case 2: LTDMC.dmc_original_reset(Card_Number[card]); break;
                        default:
                            break;
                    }
                    LTDMC.dmc_board_close();
                    Thread.Sleep(15000);
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Restart();
                    OpenCard();
                    AutoReadEvent.Set();
                    stopwatch.Stop();
                    Console.WriteLine(stopwatch.Elapsed);
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"总线复位完成！（{stopwatch.Elapsed}）");
                }
            }
        }

        /// <summary>
        /// 外部IO单按钮触发事件设置
        /// </summary>
        /// <param name="start">启动按钮输入点</param>
        /// <param name="reset">复位按钮输入点</param>
        /// <param name="stop">停止按钮输入点</param>
        /// <param name="estop">紧急停止按钮输入点</param>
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
        /// 单轴原点回归
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="home_model">回零方式</param>
        /// <param name="home_speed">回零速度</param>
        /// <param name="timeout">动作超时时间</param>
        /// <param name="acc">回零加速度</param>
        /// <param name="dcc">回零减速度</param>
        /// <param name="offpos">零点偏移</param>
        public override void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴原点回归启动错误！ {axis}轴在运动中！");
                    throw new Exception($"{axis}单轴原点回归启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴原点回归启动错误！ {axis}轴未上使能！");
                    throw new Exception($"{axis}单轴原点回归启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴原点回归启动错误！ {axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.nmc_set_home_profile(Card_Number[0], axis, home_model, home_speed / 2, home_speed, acc, dcc, offpos));
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
                            } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
                            stopwatch.Stop();
                            if (AxisStates[axis][0] < FactorValue && AxisStates[axis][0] > -FactorValue && LTDMC.dmc_check_success_encoder(Card_Number[0], axis) == 1)
                            {
                                CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{axis}轴原点回归完成，零点误差：{AxisStates[axis][0]} （{stopwatch.Elapsed}）");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                LTDMC.dmc_set_position_unit(Card_Number[0], axis, 0);
                                LTDMC.dmc_set_encoder_unit(Card_Number[0], axis, 0);
                                return;
                            }
                            else
                            {
                                if (AxisStates[axis][AxisStates[axis].Length - 1] != 0)
                                {
                                    if (CardLogEvent != null)
                                        CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                    throw new Exception($"{axis}轴原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                    return;
                                }
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归异常停止，到位编码器误差过大！（{stopwatch.Elapsed}）");
                                throw new Exception($"{axis}轴原点回归异常停止，到位编码器误差过大！（{stopwatch.Elapsed}）");
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            AxisStop(axis);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴原点回归超时停止！（{stopwatch.Elapsed}）");
                            throw new Exception($"{axis}轴原点回归超时停止！（{stopwatch.Elapsed}）");
                        });
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}轴原点回归线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}轴原点回归线程异常停止！({stopwatch.Elapsed})");
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

        /// <summary>
        /// 单轴阻塞原点回归
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="home_model">回零方式</param>
        /// <param name="home_speed">回零速度</param>
        /// <param name="timeout">等待超时时间</param>
        /// <param name="acc">回零加速度</param>
        /// <param name="dcc">回零减速度</param>
        /// <param name="offpos">零点偏移</param>
        public override void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0)
        {
            if (IsOpenCard)
            {
                if (AxisStates[axis][4] != 1)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞原点回归启动错误！ {axis}轴在运动中！");
                    throw new Exception($"{axis}单轴阻塞原点回归启动错误！ {axis}轴在运动中！");
                    return;
                }
                if (AxisStates[axis][5] != 4)
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞原点回归启动错误！ {axis}轴未上使能！");
                    throw new Exception($"{axis}单轴阻塞原点回归启动错误！ {axis}轴未上使能！");
                    return;
                }
                if (Stop_sign[axis])
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{axis}单轴阻塞原点回归启动错误！ {axis}轴处于停止中！");
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
                            CardErrorMessage(LTDMC.nmc_set_home_profile(Card_Number[0], axis, home_model, home_speed / 2, home_speed, acc, dcc, offpos));
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
                        } while (LTDMC.dmc_check_success_encoder(Card_Number[0], state.Axis) != 1);
                        stopwatch.Stop();
                        if (AxisStates[axis][0] < FactorValue && AxisStates[axis][0] > -FactorValue && LTDMC.dmc_check_success_encoder(Card_Number[0], axis) == 1)
                        {
                            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, false, $"{axis}轴阻塞原点回归完成，零点误差：{AxisStates[axis][0]} （{stopwatch.Elapsed}）");
                            lock (this)
                            {
                                IMoveStateQueue.Remove(state);
                            }
                            LTDMC.dmc_set_position_unit(Card_Number[0], axis, 0);
                            LTDMC.dmc_set_encoder_unit(Card_Number[0], axis, 0);
                            return;
                        }
                        else
                        {
                            if (AxisStates[axis][AxisStates[axis].Length - 1] != 0)
                            {
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{axis}轴阻塞原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                throw new Exception($"{axis}轴阻塞原点回归外部异常停止！（{stopwatch.Elapsed}）");
                                return;
                            }
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{axis}轴阻塞原点回归异常停止，到位编码器误差过大！（{stopwatch.Elapsed}）");
                            throw new Exception($"{axis}轴阻塞原点回归异常停止，到位编码器误差过大！！（{stopwatch.Elapsed}）");
                            return;
                        }
                    Timeout:
                        stopwatch.Stop();
                        AxisStop(axis);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{axis}轴阻塞原点回归超时停止！（{stopwatch.Elapsed}）");
                        throw new Exception($"{axis}轴阻塞原点回归超时停止！（{stopwatch.Elapsed}）");
                    }
                    catch (ThreadAbortException ex)
                    {
                        stopwatch.Stop();
                        LTDMC.dmc_stop(Card_Number[0], axis, 1);
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{axis}轴阻塞原点回归线程异常停止！({stopwatch.Elapsed})");
                        throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{axis}轴阻塞原点回归线程异常停止！({stopwatch.Elapsed})");
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

        /// <summary>
        /// 设置伺服对象字典
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="etherCATLocation">设置从站ID</param>
        /// <param name="primeindex">主索引</param>
        /// <param name="wordindexing">子索引</param>
        /// <param name="bitlength">索引长度</param>
        /// <param name="value">设置值</param>
        public override void SetbjectDictionary(ushort card, ushort etherCATLocation, ushort primeindex, ushort wordindexing, ushort bitlength, int value)
        {
            var a = LTDMC.nmc_set_node_od(Card_Number[card], 2, etherCATLocation, primeindex, wordindexing, bitlength, value);
            if (a == 0)
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, false, $"卡号{card}，EtherCAT地址{etherCATLocation}，主索引{primeindex}，子索引{wordindexing}，长度{bitlength}bit，设置值{value}成功！");
            }
            else
            {
                CardErrorMessage(a);
            }
        }

        /// <summary>
        /// 总线轴错误复位
        /// </summary>
        /// <param name="axis"></param>
        public override void AxisErrorReset(ushort axis)
        {
            if (IsOpenCard)
            {
                if (CardErrorMessage(LTDMC.nmc_clear_axis_errcode(Card_Number[0], axis)))
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, false, $"轴{axis}错误复位完成！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        /// <summary>
        /// 设置板卡轴配置文件
        /// </summary>
        public override void SetAxis_iniFile(string path)
        {
            CardErrorMessage(LTDMC.dmc_download_configfile(Card_Number[0], @"AXIS.ini"));
        }

        /// <summary>
        /// 设置EtherCAT总线配置文件
        /// </summary>
        public override void SetEtherCAT_eniFiel()
        {

        }

        /// <summary>
        /// 多轴线性插补
        /// </summary>
        /// <typeparam name="T">ControlState结构体</typeparam>
        /// <param name="card">板卡号</param>
        /// <param name="t">ControlState 结构参数</param>
        /// <param name="time">超时时间</param>
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
                        throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        throw new Exception($"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
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
                        state.Positions[i] = AxisStates[state.Axises[i]][0] + control.Position[i];
                    }
                }
                else
                {
                    state.Movetype = 8;
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = control.Position[i];
                    }
                }
                IMoveStateQueue.Add(state);
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
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
                                double[] pos = new double[control.UsingAxisNumber];
                                axis = control.Axis;
                                pos = control.Position;
                                stopwatch.Restart();
                                CardErrorMessage(LTDMC.dmc_set_vector_profile_unit(Card_Number[card], Convert.ToUInt16(coordinate), 0, control.Speed, control.Acc, control.Dcc, 0));
                                CardErrorMessage(LTDMC.dmc_line_unit(Card_Number[card], Convert.ToUInt16(coordinate), control.UsingAxisNumber, axis, pos, (ushort)control.locationModel));
                                Thread.Sleep(50);
                            }
                            Task.Factory.StartNew(() =>
                            {
                                do
                                {
                                    Thread.Sleep(20);
                                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                        goto Timeout;
                                } while (LTDMC.dmc_check_done_multicoor(Card_Number[card], Convert.ToUInt16(coordinate)) == 0);
                                do
                                {
                                    if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                        goto Timeout;
                                } while (CoordinateSystemStates[coordinate] != 4);

                                if (CoordinateSystemStates[coordinate] == 4)
                                {
                                    foreach (var item in control.Axis)
                                    {
                                        do
                                        {
                                            Thread.Sleep(0);
                                        } while (AxisStates[item][4] != 1);
                                        if (AxisStates[item][7] != 0)
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
                                throw new Exception($"{control.UsingAxisNumber}轴直线插补动作运动超时！({stopwatch.Elapsed})");
                                return;
                            Stop:
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴直线插补动作外部异常停止！({stopwatch.Elapsed})");
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
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        throw new Exception($"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        return;
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补坐标系忙碌！");
                    throw new Exception($"{control.UsingAxisNumber}插补坐标系忙碌！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
        }

        /// <summary>
        /// 两轴圆弧插补（指定圆心）
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="t">插补结构体</param>
        /// <param name="time">插补动作超时时间</param>
        public override void MoveCircle_Center(ushort card, ControlState t, int time = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 轴状态复位
        /// </summary>
        /// <param name="axis">轴号</param>
        public override void AxisReset(ushort axis)
        {
            Stop_sign[axis] = false;
            CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], axis));
            LTDMC.dmc_set_factor_error(Card_Number[0], axis, 1, FactorValue);
        }

        /// <summary>
        /// 阻塞模式多轴线性插补
        /// </summary>
        /// <typeparam name="T">ControlState结构体</typeparam>
        /// <param name="card">板卡号</param>
        /// <param name="t">ControlState 结构参数</param>
        /// <param name="time">超时时间</param>
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
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴阻塞直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        throw new Exception($"{control.UsingAxisNumber}轴阻塞直线插补启动错误！ {control.Axis[item]}轴在运动中！");
                        return;
                    }
                    if (AxisStates[item][5] != 4)
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴阻塞直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        throw new Exception($"{control.UsingAxisNumber}轴阻塞直线插补启动错误！ {control.Axis[item]}轴未上使能！");
                        return;
                    }
                    if (Stop_sign[item])
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴阻塞直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
                        throw new Exception($"{control.UsingAxisNumber}轴阻塞直线插补启动错误！ {control.Axis[item]}轴处于停止中！");
                        return;
                    }
                    CardErrorMessage(LTDMC.dmc_clear_stop_reason(Card_Number[0], item));
                    if (IMoveStateQueue.Exists(e => e.Axis == item))
                    {
                        var colose = IMoveStateQueue.Find(e => e.Axis == item);
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
                    state.Movetype = 9;//阻塞相对
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = AxisStates[state.Axises[i]][0] + control.Position[i];
                    }
                }
                else
                {
                    state.Movetype = 10;
                    for (int i = 0; i < state.Axises.Length; i++)
                    {
                        state.CurrentPositions[i] = AxisStates[state.Axises[i]][0];
                        state.Positions[i] = control.Position[i];
                    }
                }
                IMoveStateQueue.Add(state);
                var coordinate = Array.IndexOf(CoordinateSystemStates, (short)4);
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
                                    CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴阻塞直线插补启动！");
                                ushort[] axis = new ushort[control.UsingAxisNumber];
                                double[] pos = new double[control.UsingAxisNumber];
                                axis = control.Axis;
                                pos = control.Position;
                                stopwatch.Restart();
                                CardErrorMessage(LTDMC.dmc_set_vector_profile_unit(Card_Number[card], Convert.ToUInt16(coordinate), 0, control.Speed, control.Acc, control.Dcc, 0));
                                CardErrorMessage(LTDMC.dmc_line_unit(Card_Number[card], Convert.ToUInt16(coordinate), control.UsingAxisNumber, axis, pos, (ushort)control.locationModel));
                                Thread.Sleep(50);
                            }
                            do
                            {
                                Thread.Sleep(20);
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (LTDMC.dmc_check_done_multicoor(Card_Number[card], Convert.ToUInt16(coordinate)) == 0);
                            do
                            {
                                if (time != 0 && stopwatch.Elapsed.TotalMilliseconds > time)
                                    goto Timeout;
                            } while (CoordinateSystemStates[coordinate] != 4);

                            if (CoordinateSystemStates[coordinate] == 4)
                            {
                                foreach (var item in control.Axis)
                                {
                                    do
                                    {
                                        Thread.Sleep(0);
                                    } while (AxisStates[item][4] != 1);
                                    if (AxisStates[item][7] != 0)
                                        goto Stop;
                                }
                                stopwatch.Stop();
                                if (CardLogEvent != null)
                                    CardLogEvent(DateTime.Now, false, $"{control.UsingAxisNumber}轴阻塞直线插补动作完成！({stopwatch.Elapsed})");
                                lock (this)
                                {
                                    IMoveStateQueue.Remove(state);
                                }
                                return;
                            }
                        Timeout:
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴阻塞直线插补动作运动超时！({stopwatch.Elapsed})");
                            throw new Exception($"{control.UsingAxisNumber}轴阻塞直线插补动作运动超时！({stopwatch.Elapsed})");
                            return;
                        Stop:
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}轴阻塞直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            throw new Exception($"{control.UsingAxisNumber}轴阻塞直线插补动作外部异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                        catch (ThreadAbortException ex)//Capture Abort
                        {
                            stopwatch.Stop();
                            LTDMC.dmc_stop_multicoor(Card_Number[card], Convert.ToUInt16(coordinate), 1);
                            if (CardLogEvent != null)
                                CardLogEvent(DateTime.Now, true, $"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴阻塞直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            throw new Exception($"{ex.Message}\n{ex.StackTrace}\n{control.UsingAxisNumber}轴阻塞直线插补动作线程异常停止！({stopwatch.Elapsed})");
                            return;
                        }
                    }
                    else
                    {
                        if (CardLogEvent != null)
                            CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        throw new Exception($"{control.UsingAxisNumber}插补总轴数与轴号长度或定位地址长度不匹配！");
                        return;
                    }
                }
                else
                {
                    if (CardLogEvent != null)
                        CardLogEvent(DateTime.Now, true, $"{control.UsingAxisNumber}插补坐标系忙碌！");
                    throw new Exception($"{control.UsingAxisNumber}插补坐标系忙碌！");
                }
            }
            else
            {
                if (CardLogEvent != null)
                    CardLogEvent(DateTime.Now, true, $"请先调用OpenCard方法！");
                throw new Exception($"请先调用OpenCard方法！");
            }
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
    }
}
