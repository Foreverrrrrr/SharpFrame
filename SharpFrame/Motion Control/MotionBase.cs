using SharpFrame.Motion_Control;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace MotionClass
{
    /// <summary>
    /// 运动控制基类
    /// </summary>
    public abstract class MotionBase : IControlBaseInterface
    {
        [DllImport("winmm")]
        public static extern uint timeGetTime();
        [DllImport("winmm")]
        public static extern void timeBeginPeriod(int t);
        [DllImport("winmm")]
        public static extern void timeEndPeriod(int t);

        public enum InPuts
        {
        }

        public enum OutPut
        {
        }

        /// <summary>
        /// 运动控制类实例对象
        /// </summary>
        public static MotionBase Thismotion { get; set; }

        /// <summary>
        /// 停止标志
        /// </summary>
        public static bool[] Stop_sign { get; set; }

        /// <summary>
        /// 线程锁
        /// </summary>
        public static object[] Motion_Lok;

        /// <summary>
        /// 数字io输入
        /// </summary>
        public abstract bool[] IO_Input { get; set; }


        /// <summary>
        /// 数字io输出
        /// </summary>
        public abstract bool[] IO_Output { get; set; }

        /// <summary>
        /// 板卡号
        /// </summary>
        public abstract ushort[] Card_Number { get; set; }

        /// <summary>
        /// CAN通讯状态
        /// </summary>
        public abstract bool CAN_IsOpen { get; set; }

        /// <summary>
        /// 实时DA值
        /// </summary>
        public abstract double[] ADC_RealTime_DA { get; set; }

        /// <summary>
        /// 实时AD值
        /// </summary>
        public abstract double[] ADC_RealTime_AD { get; set; }

        /// <summary>
        /// 轴号
        /// </summary>
        public abstract ushort[] Axis { get; set; }

        /// <summary>
        /// 总线状态数组
        /// <para>int[0]==总线扫描周期</para>
        /// <para>int[1]==总线状态，Value=0为总线正常</para>
        /// </summary>
        public abstract int[] EtherCATStates { get; set; }

        /// <summary>
        /// 轴状态信息获取 double[][] 一维索引代表轴号，二维索引注释如下
        ///<para>double[][0]= 脉冲位置</para>
        ///<para>double[][1]= 伺服编码器位置</para>
        ///<para>double[][2]= 目标位置</para>
        ///<para>double[][3]= 速度</para>
        ///<para>double[][4]= 轴运动到位 0=运动中 1=轴停止</para>
        ///<para>double[][5]= 轴状态机0：轴处于未启动状态 1：轴处于启动禁止状态 2：轴处于准备启动状态 3：轴处于启动状态 4：轴处于操作使能状态 5：轴处于停止状态 6：轴处于错误触发状态 7：轴处于错误状态</para>
        ///<para>double[][6]= 轴运行模式0：空闲 1：Pmove 2：Vmove 3：Hmove 4：Handwheel 5：Ptt / Pts 6：Pvt / Pvts 10：Continue</para>
        ///<para>double[][7]= 轴停止原因获取0：正常停止 1：ALM 立即停止  2：ALM 减速停止 3：LTC 外部触发立即停止  4：EMG 立即停止  5：正硬限位立即停止  6：负硬限位立即停止  7：正硬限位减速停止  8：负硬限位减速停止  9：正软限位立即停止 10：负软限位立即停止11：正软限位减速停止  12：负软限位减速停止  13：命令立即停止  14：命令减速停止  15：其它原因立即停止  16：其它原因减速停止  17：未知原因立即停止  18：未知原因减速停止</para>
        /// </summary>
        public abstract double[][] AxisStates { get; set; }
        /// <summary>
        /// 输入输出高低电平反转
        /// </summary>
        public virtual bool LevelSignal { get; set; }

        /// <summary>
        /// 数据读取后台线程
        /// </summary>
        public abstract Thread[] Read_ThreadPool { get; set; }

        /// <summary>
        /// 数据读取线程管理
        /// </summary>
        public abstract ManualResetEvent AutoReadEvent { get; set; }

        /// <summary>
        /// 运动控制类方法内部Taks线程令牌
        /// </summary>
        public abstract CancellationTokenSource[] Task_Token { get; set; }

        public abstract CancellationToken[] cancellation_Token { get; set; }

        /// <summary>
        /// 板卡运行日志事件
        /// </summary>
        public abstract event Action<DateTime, bool, string> CardLogEvent;

        /// <summary>
        /// 运动控制板卡方法异常事件
        /// </summary>
        public virtual event Action<Int64, string> CardErrorMessageEvent;

        /// <summary>
        /// 启动按钮上升沿触发事件
        /// </summary>
        public virtual event Action<DateTime> StartPEvent;

        /// <summary>
        /// 启动按钮下降沿触发事件
        /// </summary>
        public virtual event Action<DateTime> StartNEvent;

        /// <summary>
        /// 复位按钮上升沿触发事件
        /// </summary>
        public virtual event Action<DateTime> ResetPEvent;

        /// <summary>
        /// 复位按钮下降沿触发事件
        /// </summary>
        public virtual event Action<DateTime> ResetNEvent;

        /// <summary>
        /// 停止按钮上升沿触发事件
        /// </summary>
        public virtual event Action<DateTime> StopPEvent;

        /// <summary>
        /// 停止按钮下降沿触发事件
        /// </summary>
        public virtual event Action<DateTime> StopNEvent;

        /// <summary>
        /// 紧急停止按钮上升沿触发事件
        /// </summary>
        public virtual event Action<DateTime> EStopPEvent;

        /// <summary>
        /// 紧急停止按钮下降沿触发事件
        /// </summary>
        public virtual event Action<DateTime> EStopNEvent;

        /// <summary>
        /// 光栅上升沿触发事件
        /// </summary>
        public virtual event Action<DateTime> RasterPEvent;

        /// <summary>
        /// 光栅按钮下降沿触发事件
        /// </summary>
        public virtual event Action<DateTime> RasterNEvent;

        /// <summary>
        /// 门禁上升沿触发事件
        /// </summary>
        public virtual event Action<DateTime> EntrancePEvent;

        /// <summary>
        /// 门禁按钮下降沿触发事件
        /// </summary>
        public virtual event Action<DateTime> EntranceNEvent;

        /// <summary>
        /// 轴定位状态结构
        /// </summary>
        public struct MoveState
        {
            /// <summary>
            /// 卡号
            /// </summary>
            public ushort CardID { get; set; }
            /// <summary>
            /// 定位前位置
            /// </summary>
            public double CurrentPosition { get; set; }
            /// <summary>
            /// 目标位置
            /// </summary>
            public double Position { get; set; }
            /// <summary>
            /// 轴定位指令
            /// <para>1=单轴绝对定位</para>
            /// <para>2=单轴相对定位</para>
            /// <para>3=单轴阻塞绝对定位</para>
            /// <para>4=单轴阻塞相对定位</para>
            /// <para>5=单轴原点回归</para>
            /// <para>6=单轴阻塞原点回归</para>
            /// <para>7=多轴线性相对插补定位</para>
            /// <para>8=多轴线性绝对插补定位</para>
            /// <para>9=阻塞多轴线性相对插补定位</para>
            /// <para>10=阻塞多轴线性绝对插补定位</para>
            /// </summary>
            public ushort Movetype { get; set; }
            /// <summary>
            /// 定位速度
            /// </summary>
            public double Speed { get; set; }
            /// <summary>
            /// 定位轴号
            /// </summary>
            public ushort Axis { get; set; }
            /// <summary>
            /// 原点回归模式
            /// </summary>
            public ushort HomeModel { get; set; }
            /// <summary>
            /// 加速度
            /// </summary>
            public double ACC { get; set; }
            /// <summary>
            /// 减速度
            /// </summary>
            public double Dcc { get; set; }
            /// <summary>
            /// 零点偏置
            /// </summary>
            public double Home_off { get; set; }
            /// <summary>
            /// 等待超时时间（ms）
            /// </summary>
            public int OutTime { get; set; }

            /// <summary>
            /// 插补使用轴总数
            /// </summary>
            public ushort UsingAxisNumber { get; set; }
            private ushort[] _axises;
            /// <summary>
            /// 插补轴号
            /// </summary>
            public ushort[] Axises
            {
                get { return _axises; }
                set
                {
                    if (value.Length > UsingAxisNumber)
                    {
                        _axises = null;
                        throw new Exception("轴数量大于设置插补轴总数");
                    }
                    else
                    {
                        _axises = value;
                        Axis = value[0];
                    }

                }
            }
            private double[] _positions;
            /// <summary>
            /// 插补定位目标位置
            /// </summary>
            public double[] Positions
            {
                get { return _positions; }
                set
                {
                    if (value.Length > UsingAxisNumber)
                    {
                        _positions = null;
                        throw new Exception("定位地址数量大于设置插补轴总数");
                    }
                    else
                    {
                        _positions = value;
                    }
                }
            }
            /// <summary>
            /// 插补定位前位置
            /// </summary>
            public double[] CurrentPositions { get; set; }
            /// <summary>
            /// 状态句柄
            /// </summary>
            public DateTime Handle { get; set; }
        }

        /// <summary>
        /// 插补配置结构
        /// </summary>
        public struct ControlState
        {
            /// <summary>
            /// 插补使用轴总数
            /// </summary>
            public ushort UsingAxisNumber { get; set; }

            /// <summary>
            /// 插补速度
            /// </summary>
            public double Speed { get; set; }

            /// <summary>
            /// 插补加速度
            /// </summary>
            public double Acc { get; set; }

            /// <summary>
            /// 插补减速度
            /// </summary>
            public double Dcc { get; set; }

            /// <summary>
            ///定位模式 0=相对 1=绝对
            /// </summary>
            public int locationModel { get; set; }

            private ushort[] _axis;
            /// <summary>
            /// 插补轴号
            /// </summary>
            public ushort[] Axis
            {
                get { return _axis; }
                set
                {
                    if (value.Length > UsingAxisNumber)
                    {
                        _axis = null;
                        throw new Exception("轴数量大于设置插补轴总数");
                    }
                    else
                    {
                        _axis = value;

                    }
                }
            }

            private double[] _position;
            /// <summary>
            /// 定位地址
            /// </summary>
            public double[] Position
            {
                get { return _position; }
                set
                {
                    if (value.Length > UsingAxisNumber)
                    {
                        _position = null;
                        throw new Exception("定位地址数量大于设置插补轴总数");
                    }
                    else
                    {
                        _position = value;
                    }
                }
            }
        }

        /// <summary>
        /// 板卡品牌名称
        /// </summary>
        public enum CardName
        {
            /// <summary>
            /// 雷赛总线卡
            /// </summary>
            LeiSaiEtherCat,
            /// <summary>
            /// 雷赛脉冲卡
            /// </summary>
            LeiSaiPulse,
            /// <summary>
            /// 高川板卡
            /// </summary>
            GaoChuān,
            /// <summary>
            /// 摩升泰板卡
            /// </summary>
            MoShengTai,
            /// <summary>
            ///凌华板卡
            /// </summary>    
            LingHua
        }

        /// <summary>
        /// 板卡厂商
        /// </summary>
        public static string CardBrand { get; set; }

        /// <summary>
        /// 轴定位状态集合
        /// </summary>
        public virtual List<MoveState> IMoveStateQueue { get; set; }

        /// <summary>
        /// 到位误差
        /// </summary>
        public abstract ushort FactorValue { get; set; }
        /// <summary>
        /// 定位速度
        /// </summary>
        public abstract double Speed { get; set; }
        /// <summary>
        /// 加速度
        /// </summary>
        public abstract double Acc { get; set; }
        /// <summary>
        /// 减速度
        /// </summary>
        public abstract double Dec { get; set; }

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
        public abstract int[][] Axis_IO { get; set; }

        /// <summary>
        /// 插补坐标系状态
        /// <para>short[0] 一维索引为坐标系号</para>
        /// <para>short[]= 0 坐标系运动中</para>
        /// <para>short[]= 1 暂停中</para>
        /// <para>short[]= 2 正常停止</para>
        /// <para>short[]= 3 已被占用但未启动</para>
        /// <para>short[]= 4 坐标系空闲</para>
        /// </summary>
        public abstract short[] CoordinateSystemStates { get; set; }

        /// <summary>
        /// 板卡是否打开
        /// </summary>
        public abstract bool IsOpenCard { get; set; }

        /// <summary>
        /// 总线轴总数
        /// </summary>
        public abstract int Axisquantity { get; set; }

        /// <summary>
        /// 获取板卡对象
        /// </summary>
        /// <param name="modelname">板卡厂商名称</param>
        /// <returns>返回板卡对象</returns>
        /// <exception cref="Exception"> MotionBase类：GetClass方法中出现异常</exception>
        public static MotionBase GetClassType(CardName modelname)
        {
            try
            {
                CardBrand = modelname.ToString();
                var Assemblyname = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                Type type = Type.GetType(Assemblyname + "." + modelname.ToString());
                dynamic obj = type.Assembly.CreateInstance(type.ToString());
                MotionBase classBase = obj as MotionBase;
                return classBase;
            }
            catch (Exception ex)
            {
                throw new Exception(" MotionBase类：GetClass方法中出现异常", ex);
            }
        }

        /// <summary>
        /// 板卡方法异常查询
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="type">方法返回值</param>
        /// <param name="returnPattern">是否查询</param>
        /// <returns></returns>
        public virtual bool CardErrorMessage<T>(T type, bool returnPattern = true) where T : struct
        {
            if (Convert.ToInt64(type) != 0)
            {
                string data = "";
                if (returnPattern)
                {
                    ErrorQuery.Query(CardBrand, Convert.ToInt32(type));
                    //data = SQLHelper.Readdata(CardBrand, Convert.ToInt32(type));
                }
                if (CardErrorMessageEvent != null)
                    CardErrorMessageEvent(Convert.ToInt64(type), data);
                throw new Exception("type:" + data);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 打开指定板卡
        /// </summary>
        /// <param name="card_number">板卡号</param>
        /// <returns></returns>
        public abstract bool OpenCard(ushort card_number);

        /// <summary>
        /// 打开所有板卡
        /// </summary>
        /// <returns></returns>
        public abstract bool OpenCard();

        /// <summary>
        /// 单个轴使能
        /// </summary>
        /// <param name="card">卡号</param>
        /// <param name="axis">轴号</param>
        /// <returns></returns>
        public abstract void AxisOn(ushort card, ushort axis);

        /// <summary>
        /// 所有轴使能
        /// </summary>
        /// <returns></returns>
        public abstract void AxisOn();

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
        public abstract void AxisBasicSet(ushort axis, double equiv, double startvel, double speed, double acc, double dec, double stopvel, double s_para, int posi_mode, int stop_mode);

        /// <summary>
        /// 单轴JOG运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="speed">运行速度</param>
        /// <param name="posi_mode">运动方向，0：负方向，1：正方向</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        public abstract void MoveJog(ushort axis, double speed, int posi_mode, double acc = 0.5, double dec = 0.5);

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="stop_mode">停止方式 0=减速停止 1=紧急停止</param>
        /// <param name="all">是否全部轴停止</param>
        public abstract void AxisStop(ushort axis, int stop_mode, bool all);

        /// <summary>
        /// 单轴绝对定位（非阻塞模式，调用该方法后需要自行处理是否运动完成）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">定位地址</param>
        /// <param name="speed">定位速度</param>
        ///  <param name="time">超时时间</param>
        public abstract void MoveAbs(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 单轴相对定位（非阻塞模式，调用该方法后需要自行处理是否运动完成）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">定位地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">超时时间</param>
        public abstract void MoveRel(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 获取轴状态信息
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns> 
        /// 返回值 double[6] 数组
        /// <para>double[0]= 脉冲位置</para>
        /// <para>double[1]= 伺服编码器位置</para>
        /// <para>double[2]= 速度</para>
        /// <para>double[3]= 目标位置</para>
        /// <para>double[4]= 轴运动到位     0=运动中 1=轴停止</para>
        /// <para>double[5]= 轴状态机       0：轴处于未启动状态 1：轴处于启动禁止状态 2：轴处于准备启动状态 3：轴处于启动状态 4：轴处于操作使能状态 5：轴处于停止状态 6：轴处于错误触发状态 7：轴处于错误状态</para>
        /// <para>double[6]= 轴运行模式     0：空闲  1：Pmove 2：Vmove 3：Hmove 4：Handwheel 5：Ptt / Pts 6：Pvt / Pvts 10：Continue</para>
        /// <para>double[7]= 轴停止原因获取 0：正常停止 3：LTC 外部触发立即停止 4：EMG 立即停止 5：正硬限位立即停止 6：负硬限位立即停止 7：正硬限位减速停止 8：负硬限位减速停止 9：正软限位立即停止</para>
        /// <para>                          10：负软限位立即停止 11：正软限位减速停止 12：负软限位减速停止 13：命令立即停止 14：命令减速停止 15：其它原因立即停止 16：其它原因减速停止 17：未知原因立即停止 18：未知原因减速停止</para>
        /// </returns>
        public abstract double[] GetAxisState(ushort axis);

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
        public abstract int[] GetAxisExternalio(ushort axis);

        /// <summary>
        /// 复位轴停止前定位动作
        /// </summary>
        /// <param name="axis">轴号</param>
        public abstract void MoveReset(ushort axis);

        /// <summary>
        /// 单轴绝对定位（阻塞模式，调用该方法后定位运动完成后或超时返回）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">绝对地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">等待超时时长：0=一直等待直到定位完成</param>
        public abstract void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 单轴相对定位（阻塞模式，调用该方法后定位运动完成后或超时返回）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">相对地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">等待超时时长：0=一直等待直到定位完成</param>
        public abstract void AwaitMoveRel(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 获取板卡全部数字输入
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        public abstract bool[] Getall_IOinput(ushort card);



        /// <summary>
        /// 获取板卡全部数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        public abstract bool[] Getall_IOoutput(ushort card);

        /// <summary>
        /// 设置数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输出点位</param>
        /// <param name="value">输出值</param>
        public abstract void Set_IOoutput(ushort card, ushort indexes, bool value);

        /// <summary>
        /// 设置数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输出点位</param>
        /// <param name="value">输出值</param>
        public abstract void Set_IOoutput_Enum(ushort card, OutPut indexes, bool value);

        /// <summary>
        /// 设置紧急停止外部IO
        /// </summary>
        public virtual void Set_ExigencyIO(ushort card, ushort in_put, uint stop_model)
        {
            throw new NotImplementedException("该板卡无效设置紧急停止IO");
        }

        /// <summary>
        /// 等待输入信号
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输入口</param>
        /// <param name="waitvalue">等待状态</param>
        /// <param name="timeout">等待超时时间</param>
        public abstract void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0);

        /// <summary>
        /// 等待输入信号
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输入口</param>
        /// <param name="waitvalue">等待状态</param>
        /// <param name="timeout">等待超时时间</param>
        public abstract void AwaitIOinput_Enum(ushort card, InPuts indexes, bool waitvalue, int timeout = 0);

        /// <summary>
        /// 外部IO单按钮触发事件设置
        /// </summary>
        /// <param name="start">启动按钮输入点</param>
        /// <param name="reset">复位按钮输入点</param>
        /// <param name="stop">停止按钮输入点</param>
        /// <param name="estop">紧急停止按钮输入点</param>
        public abstract void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop);

        /// <summary>
        /// 外部IO单按钮触发事件设置
        /// </summary>
        /// <param name="start">启动按钮输入点</param>
        /// <param name="reset">复位按钮输入点</param>
        /// <param name="stop">停止按钮输入点</param>
        /// <param name="estop">紧急停止按钮输入点</param>
        /// <param name="raster">光栅</param>
        /// <param name="entrance">门禁</param>
        public abstract void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster, ushort entrance);
        public abstract void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop, ushort raster1, ushort raster2, ushort entrance1, ushort entrance2);
        /// <summary>
        /// 读取总线状态
        /// </summary>
        /// <param name="card_number">板卡号</param>
        /// <returns>int[0]=总线扫描时长us int[1]总线状态==0正常</returns>
        public abstract int[] GetEtherCATState(ushort card_number);

        /// <summary>
        /// 运动控制卡复位
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="reset">0=热复位 1=冷复位 2=初始复位</param>
        public abstract void ResetCard(ushort card, ushort reset);

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
        public abstract void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0);

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
        public abstract void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0);

        /// <summary>
        /// 设置伺服对象字典
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="etherCATLocation">设置从站ID</param>
        /// <param name="primeindex">主索引</param>
        /// <param name="wordindexing">子索引</param>
        /// <param name="bitlength">索引长度</param>
        /// <param name="value">设置值</param>
        public abstract void SetbjectDictionary(ushort card, ushort etherCATLocation, ushort primeindex, ushort wordindexing, ushort bitlength, int value);

        /// <summary>
        /// 总线轴错误复位
        /// </summary>
        /// <param name="axis"></param>
        public abstract void AxisErrorReset(ushort axis);

        /// <summary>
        /// 设置板卡轴配置文件
        /// </summary>
        public abstract void SetAxis_iniFile(string path = "AXIS.ini");

        /// <summary>
        /// 设置EtherCAT总线配置文件
        /// </summary>
        public abstract void SetEtherCAT_eniFiel();

        /// <summary>
        /// 多轴线性插补
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="t">插补结构体</param>
        /// <param name="time">插补动作超时时间</param>
        public abstract void MoveLines(ushort card, ControlState t, int time = 0);


        /// <summary>
        /// 阻塞多轴线性插补
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="t">插补结构体</param>
        /// <param name="time">插补动作超时时间</param>
        public abstract void AwaitMoveLines(ushort card, ControlState t, int time = 0);

        /// <summary>
        /// 两轴圆弧插补（圆心）
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="t">插补结构体</param>
        /// <param name="time">插补动作超时时间</param>
        public abstract void MoveCircle_Center(ushort card, ControlState t, int time = 0);

        /// <summary>
        /// 单轴下使能
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="axis">轴号</param>
        public abstract void AxisOff(ushort card, ushort axis);

        /// <summary>
        /// 所有轴下使能
        /// </summary>
        public abstract void AxisOff();

        /// <summary>
        /// 释放控制卡
        /// </summary>
        public abstract void CloseCard();

        /// <summary>
        /// 轴状态复位
        /// </summary>
        /// <param name="axis">轴号</param>
        public abstract void AxisReset(ushort axis);
        public abstract void WaitAxis(int[] axis);

        /// <summary>
        /// 设置DA输出
        /// </summary>
        /// <param name="card">卡号</param>
        /// <param name="channel_number">通道号</param>
        /// <param name="voltage_values">电压值</param>
        public abstract void Set_DA(ushort card, ushort channel_number, double voltage_values);


        /// <summary>
        /// 读取DA输出
        /// </summary>
        /// <param name="card">卡号</param>
        /// <param name="channel_number">通道号</param>
        /// <returns>读取电压值</returns>
        public abstract double Read_DA(ushort card, ushort channel_number);

        /// <summary>
        /// 读取AD输出
        /// </summary>
        /// <param name="card">控制卡号</param>
        /// <param name="channel_number">AD通道号</param>
        /// <returns></returns>
        public abstract double Read_AD(ushort card, ushort channel_number);

        /// <summary>
        /// 配置CAN扩展
        /// </summary>
        /// <param name="card">控制卡号</param>
        /// <param name="can_num">CAN连接数  1-8</param>
        /// <param name="can_state">CAN连接状态 false==断开 true==连接</param>
        /// <param name="can_baud">CAN波特率  0=1000Kbps</param>
        public abstract void Deploy_CAN(ushort card, ushort can_num, bool can_state, ushort can_baud = 0);
    }

}
