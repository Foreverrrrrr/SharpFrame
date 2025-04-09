using System;
using System.Threading;


namespace MotionClass
{
    /// <summary>
    /// 运动控制接口
    /// </summary>
    internal interface IControlBaseInterface
    {
        /// <summary>
        /// 总线轴总数
        /// </summary>
        int Axisquantity { get; set; }

        /// <summary>
        /// 板卡是否打开
        /// </summary>
        bool IsOpenCard { get; set; }

        /// <summary>
        /// 总线状态数组
        /// <para>int[0]==总线扫描周期</para>
        /// <para>int[1]==总线状态，Value=0为总线正常</para>
        /// </summary>
        int[] EtherCATStates { get; set; }

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
        double[][] AxisStates { get; set; }

        /// <summary>
        /// 插补坐标系状态
        /// <para>short[0] 一维索引为坐标系号</para>
        /// <para>short[]= 0 坐标系运动中</para>
        /// <para>short[]= 1 暂停中</para>
        /// <para>short[]= 2 正常停止</para>
        /// <para>short[]= 3 已被占用但未启动</para>
        /// <para>short[]= 4 坐标系空闲</para>
        /// </summary>
        short[] CoordinateSystemStates { get; set; }

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
        int[][] Axis_IO { get; set; }

        /// <summary>
        /// 数字io输入
        /// </summary>
        bool[] IO_Input { get; set; }

        /// <summary>
        /// 数字io输出
        /// </summary>
        bool[] IO_Output { get; set; }

        /// <summary>
        /// 板卡号
        /// </summary>
        ushort[] Card_Number { get; set; }

        /// <summary>
        /// 轴号
        /// </summary>
        ushort[] Axis { get; set; }

        /// <summary>
        /// 到位误差
        /// </summary>
        ushort FactorValue { get; set; }

        /// <summary>
        /// 数据读取后台线程
        /// </summary>
        Thread[] Read_ThreadPool { get; set; }

        /// <summary>
        /// 数据读取线程管理
        /// </summary>
        ManualResetEvent AutoReadEvent { get; set; }

        /// <summary>
        /// 运动控制板卡方法异常事件
        /// </summary>
        event Action<Int64, string> CardErrorMessageEvent;

        /// <summary>
        /// 板卡运行日志事件
        /// </summary>
        event Action<DateTime, bool, string> CardLogEvent;

        /// <summary>
        /// 打开指定板卡
        /// </summary>
        /// <param name="card_number">板卡号</param>
        /// <returns></returns>
        bool OpenCard(ushort card_number);

        /// <summary>
        /// 打开所有板卡
        /// </summary>
        /// <returns></returns>
        bool OpenCard();

        /// <summary>
        /// 释放控制卡
        /// </summary>
        void CloseCard();

        /// <summary>
        /// 单个轴使能
        /// </summary>
        /// <param name="card">卡号</param>
        /// <param name="axis">轴号</param>
        /// <returns></returns>
        void AxisOn(ushort card, ushort axis);

        /// <summary>
        /// 所有轴使能
        /// </summary>
        /// <returns></returns>
        void AxisOn();

        /// <summary>
        /// 单轴下使能
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="axis">轴号</param>
        void AxisOff(ushort card, ushort axis);

        /// <summary>
        /// 所有轴下使能
        /// </summary>
        void AxisOff();

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
        void AxisBasicSet(ushort axis, double equiv, double startvel, double speed, double acc, double dec, double stopvel, double s_para, int posi_mode, int stop_mode);

        /// <summary>
        /// 单轴JOG运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="speed">运行速度</param>
        /// <param name="posi_mode">运动方向，0：负方向，1：正方向</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        void MoveJog(ushort axis, double speed, int posi_mode = 0, double acc = 0.5, double dec = 0.5);

        /// <summary>
        /// 轴停止
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="stop_mode">停止方式 0=减速停止 1=紧急停止</param>
        /// <param name="all">是否全部轴停止</param>
        void AxisStop(ushort axis, int stop_mode, bool all);

        /// <summary>
        /// 轴状态复位
        /// </summary>
        /// <param name="axis">轴号</param>
        void AxisReset(ushort axis);

        /// <summary>
        /// 复位轴停止前定位动作
        /// </summary>
        /// <param name="axis">轴号</param>
        void MoveReset(ushort axis);

        /// <summary>
        /// 单轴绝对定位（非阻塞模式，调用该方法后需要自行处理是否运动完成）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">定位地址</param>
        /// <param name="speed">定位速度</param>
        ///  <param name="time">超时时间</param>
        void MoveAbs(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 单轴相对定位（非阻塞模式，调用该方法后需要自行处理是否运动完成）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">定位地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">超时时间</param>
        void MoveRel(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 单轴绝对定位（阻塞模式，调用该方法后定位运动完成后或超时返回）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">绝对地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">等待超时时长：0=一直等待直到定位完成</param>
        void AwaitMoveAbs(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 单轴相对定位（阻塞模式，调用该方法后定位运动完成后或超时返回）
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">相对地址</param>
        /// <param name="speed">定位速度</param>
        /// <param name="time">等待超时时长：0=一直等待直到定位完成</param>
        void AwaitMoveRel(ushort axis, double position, double speed, int time = 0);

        /// <summary>
        /// 读取总线状态
        /// </summary>
        /// <param name="card_number">板卡号</param>
        /// <returns>int[0]=总线扫描时长us int[1]总线状态==0正常</returns>
        int[] GetEtherCATState(ushort card_number);

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
        double[] GetAxisState(ushort axis);

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
        int[] GetAxisExternalio(ushort axis);

        /// <summary>
        /// 获取板卡全部数字输入
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        bool[] Getall_IOinput(ushort card);

        /// <summary>
        /// 获取板卡全部数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <returns></returns>
        bool[] Getall_IOoutput(ushort card);

        /// <summary>
        /// 设置数字输出
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输出点位</param>
        /// <param name="value">输出值</param>
        void Set_IOoutput(ushort card, ushort indexes, bool value);

        /// <summary>
        /// 等待输入信号
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="indexes">输入口</param>
        /// <param name="waitvalue">等待状态</param>
        /// <param name="timeout">等待超时时间</param>
        void AwaitIOinput(ushort card, ushort indexes, bool waitvalue, int timeout = 0);

        /// <summary>
        /// 外部IO单按钮触发事件设置
        /// </summary>
        /// <param name="start">启动按钮输入点</param>
        /// <param name="reset">复位按钮输入点</param>
        /// <param name="stop">停止按钮输入点</param>
        /// <param name="estop">紧急停止按钮输入点</param>
        void SetExternalTrigger(ushort start, ushort reset, ushort stop, ushort estop);

        /// <summary>
        /// 运动控制卡复位
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="reset">0=热复位 1=冷复位 2=初始复位</param>
        void ResetCard(ushort card, ushort reset);

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
        void MoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0);

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
        void AwaitMoveHome(ushort axis, ushort home_model, double home_speed, int timeout = 0, double acc = 0.5, double dcc = 0.5, double offpos = 0);

        /// <summary>
        /// 设置伺服对象字典
        /// </summary>
        /// <param name="card">板卡号</param>
        /// <param name="etherCATLocation">设置从站ID</param>
        /// <param name="primeindex">主索引</param>
        /// <param name="wordindexing">子索引</param>
        /// <param name="bitlength">索引长度</param>
        /// <param name="value">设置值</param>
        void SetbjectDictionary(ushort card, ushort etherCATLocation, ushort primeindex, ushort wordindexing, ushort bitlength, int value);

        /// <summary>
        /// 总线轴错误复位
        /// </summary>
        /// <param name="axis"></param>
        void AxisErrorReset(ushort axis);

        /// <summary>
        /// 设置板卡轴配置文件
        /// </summary>
        void SetAxis_iniFile(string path = "AXIS.ini");

        void SetEtherCAT_eniFiel();

        void WaitAxis(int[] axis);
    }
}
