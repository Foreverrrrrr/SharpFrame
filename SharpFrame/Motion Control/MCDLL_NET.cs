using System.Runtime.InteropServices;

namespace MotionClass
{
    public class CMCDLL_NET
    {

        /********************************************************************************************************************************************************************
                                                               1 控制卡打开函数
        ********************************************************************************************************************************************************************/
        //1.1 初始化函数                        									    [1,100]                       [0,99]                     宏定义1.1 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Open_Net")]
        public static extern short MCF_Open_Net(ushort Connection_Number, ref ushort Station_Number, ref ushort Station_Type);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Close_Net")]
        public static extern short MCF_Close_Net();

        /********************************************************************************************************************************************************************
                                                              2 通用输入输出函数
        ********************************************************************************************************************************************************************/
        //2.1 通用IO全部输出函数                               [OUT31,OUT0]                     [0,99]               
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Output_Net")]
        public static extern short MCF_Set_Output_Net(uint All_Output_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Output_Net")]
        public static extern short MCF_Get_Output_Net(ref uint All_Output_Logic, ushort StationNumber = 0);
        //2.2 通用IO按位输出函数                          							宏定义2.3.1        宏定义2.3.2  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Output_Bit_Net")]
        public static extern short MCF_Set_Output_Bit_Net(ushort Bit_Output_Number, ushort Bit_Output_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Output_Bit_Net")]
        public static extern short MCF_Get_Output_Bit_Net(ushort Bit_Output_Number, ref ushort Bit_Output_Logic, ushort StationNumber = 0);
        //2.3 通用IO输出复用：按位输出保持时间函数                       宏定义2.3.1                      宏定义2.3.2                      [0,65535]                       [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Output_Time_Bit_Net")]
        public static extern short MCF_Set_Output_Time_Bit_Net(ushort Bit_Output_Number, ushort Bit_Output_Logic, ushort Output_Time_1MS, ushort StationNumber = 0);
        //    通用IO输出复用：按位置输出保持时间函数           宏定义2.3.1                           [0,1000]                          [-2^31,(2^31-1)]            [0,65535]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Compare_Output_Bit_Net")]
        public static extern short MCF_Set_Compare_Output_Bit_Net(ushort Compare_Output_Number, ushort Compare_Output_1MS, ushort Compare_dDist, ushort StationNumber = 0);

        //2.4 通用IO全部输入函数                               [Input31,Input0]                 [Input48,Input32]               [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Net")]
        public static extern short MCF_Get_Input_Net(ref uint All_Input_Logic1, ref uint All_Input_Logic2, ushort StationNumber = 0);
        //2.5 通用IO按位输入函数                         							宏定义2.4.1        宏定义2.4.2  [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Bit_Net")]
        public static extern short MCF_Get_Input_Bit_Net(ushort Bit_Input_Number, ref ushort Bit_Input_Logic, ushort StationNumber = 0);
        //2.6 通用IO按位输入下升沿高速捕获清除函数             [Bit_Input_0,Bit_Input_3]        [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Input_Fall_Bit_Net")]
        public static extern short MCF_Clear_Input_Fall_Bit_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        //2.7 通用IO按位输入下降沿高速捕获读取函数             [Bit_Input_0,Bit_Input_3]        宏定义2.7                      [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Fall_Bit_Net")]
        public static extern short MCF_Get_Input_Fall_Bit_Net(ushort Bit_Input_Number, ref ushort Bit_Input_Fall, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              3 轴专用输入输出函数
        ********************************************************************************************************************************************************************/
        //3.1 伺服使能设置函数                              						 宏定义0.0    宏定义3.1          [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Servo_Enable_Net")]
        public static extern short MCF_Set_Servo_Enable_Net(ushort Axis, ushort Servo_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_Enable_Net")]
        public static extern short MCF_Get_Servo_Enable_Net(ushort Axis, ref ushort Servo_Logic, ushort StationNumber = 0);
        //3.2 伺服报警复位设置函数                         							 宏定义0.0    宏定义3.2           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Servo_Alarm_Reset_Net")]
        public static extern short MCF_Set_Servo_Alarm_Reset_Net(ushort Axis, ushort Alarm_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_Alarm_Reset_Net")]
        public static extern short MCF_Get_Servo_Alarm_Reset_Net(ushort Axis, ref ushort Alarm_Logic, ushort StationNumber = 0);
        //3.3 伺服报警输入获取函数                         							 宏定义0.0    宏定义3.3                    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_Alarm_Net")]
        public static extern short MCF_Get_Servo_Alarm_Net(ushort Axis, ref ushort Servo_Alarm_State, ushort StationNumber = 0);
        //3.4 伺服定位完成输入获取函数                   							 宏定义0.0    宏定义3.4                   [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_INP_Net")]
        public static extern short MCF_Get_Servo_INP_Net(ushort Axis, ref ushort Servo_INP_State, ushort StationNumber = 0);
        //3.5 编码器Z相输入获取函数              									 宏定义0.0    宏定义3.5           [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Z_Net")]
        public static extern short MCF_Get_Z_Net(ushort Axis, ref ushort Z_State, ushort StationNumber = 0);
        //3.6 原点输入获取函数                      								 宏定义0.0    宏定义3.6              [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Net")]
        public static extern short MCF_Get_Home_Net(ushort Axis, ref ushort Home_State, ushort StationNumber = 0);
        //3.7 正限位输入获取函数                              						 宏定义0.0    宏定义3.7                        [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Positive_Limit_Net")]
        public static extern short MCF_Get_Positive_Limit_Net(ushort Axis, ref ushort Positive_Limit_State, ushort StationNumber = 0);
        //3.8 负限位输入获取函数                              						 宏定义0.0    宏定义3.8                        [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Negative_Limit_Net")]
        public static extern short MCF_Get_Negative_Limit_Net(ushort Axis, ref ushort Negative_Limit_State, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              4 轴设置函数
        ********************************************************************************************************************************************************************/
        //4.1 脉冲通道输出设置函数                        							 宏定义0.0    宏定义4.1  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Pulse_Mode_Net")]
        public static extern short MCF_Set_Pulse_Mode_Net(ushort Axis, uint Pulse_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Pulse_Mode_Net")]
        public static extern short MCF_Get_Pulse_Mode_Net(ushort Axis, ref uint Pulse_Mode, ushort StationNumber = 0);
        //4.2 位置设置函数 															 宏定义0.0    [-2^31,(2^31-1)]    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Position_Net")]
        public static extern short MCF_Set_Position_Net(ushort Axis, int Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Position_Net")]
        public static extern short MCF_Get_Position_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        //4.3 编码器设置函数                          								 宏定义0.0    [-2^31,(2^31-1)]  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Encoder_Net")]
        public static extern short MCF_Set_Encoder_Net(ushort Axis, int Encoder, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Encoder_Net")]
        public static extern short MCF_Get_Encoder_Net(ushort Axis, ref int Encoder, ushort StationNumber = 0);
        //4.4 速度获取                            									 宏定义0.0    [-2^15,(2^15-1)]      [-2^15,(2^15-1)]        [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Vel_Net")]
        public static extern short MCF_Get_Vel_Net(ushort Axis, ref double Command_Vel, ref double Encode_Vel, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              5 轴硬件触发停止运动函数
        ********************************************************************************************************************************************************************/
        //5.1 5.1 通用IO输入复用：做为紧急停止函数                 					宏定义2.4.1              宏定义5.1      [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_EMG_Bit_Net")]
        public static extern short MCF_Set_EMG_Bit_Net(ushort EMG_Input_Number, ushort EMG_Mode, ushort StationNumber = 0);
        //    通用IO输入复用：做为触发停止                    [0,3]                   宏定义0.0            [Bit_Input_0,Bit_Input_15]       宏定义5.4                   [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Input_Trigger_Net")]
        public static extern short MCF_Set_Input_Trigger_Net(ushort Channel, ushort Axis, ushort Bit_Input_Number, uint Trigger_Mode, ushort StationNumber = 0);

        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Trigger_Net")]
        public static extern short MCF_Get_Input_Trigger_Net(ushort Channel, ref ushort Axis, ref ushort Bit_Input_Number, ref uint Trigger_Mode, ushort StationNumber = 0);

        //5.2 软件限位触发运动停止函数                  							 宏定义0.0    [-2^31,2^31]P     >      [-2^31,2^31]P           [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Soft_Limit_Net")]
        public static extern short MCF_Set_Soft_Limit_Net(ushort Axis, int Positive_Position, int Negative_Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Soft_Limit_Net")]
        public static extern short MCF_Get_Soft_Limit_Net(ushort Axis, ref int Positive_Position, ref int Negative_Position, ushort StationNumber = 0);
        //5.3 软件限位触发运动停止开关函数                     						 宏定义0.0    宏定义5.3               [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Soft_Limit_Enable_Net")]
        public static extern short MCF_Set_Soft_Limit_Enable_Net(ushort Axis, uint Soft_Limit_Enable, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Soft_Limit_Enable_Net")]
        public static extern short MCF_Get_Soft_Limit_Enable_Net(ushort Axis, ref uint Soft_Limit_Enable, ushort StationNumber = 0);
        //5.4 伺服报警触发运动停止函数                       						 宏定义0.0    宏定义5.4          [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Alarm_Trigger_Net")]
        public static extern short MCF_Set_Alarm_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Alarm_Trigger_Net")]
        public static extern short MCF_Get_Alarm_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.5 Index触发运动停止函数                          						 宏定义0.0    宏定义5.4         [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Index_Trigger_Net")]
        public static extern short MCF_Set_Index_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Index_Trigger_Net")]
        public static extern short MCF_Get_Index_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.6 原点触发运动停止函数                          						 宏定义0.0     宏定义5.4         [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Home_Trigger_Net")]
        public static extern short MCF_Set_Home_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Trigger_Net")]
        public static extern short MCF_Get_Home_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.7 正限位触发运动停止函数                       							 宏定义0.0    宏定义5.4          [0,99]    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_ELP_Trigger_Net")]
        public static extern short MCF_Set_ELP_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_ELP_Trigger_Net")]
        public static extern short MCF_Get_ELP_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.8 负限位触发运动停止函数                       							 宏定义0.0    宏定义5.4          [0,99]    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_ELN_Trigger_Net")]
        public static extern short MCF_Set_ELN_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_ELN_Trigger_Net")]
        public static extern short MCF_Get_ELN_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.9 原点触发位置记录函数	                           						 宏定义0.0   [-2^31,(2^31-1)]  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Rise_Position_Net")]
        public static extern short MCF_Get_Home_Rise_Position_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Fall_Position_Net")]
        public static extern short MCF_Get_Home_Fall_Position_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Rise_Encoder_Net")]
        public static extern short MCF_Get_Home_Rise_Encoder_Net(ushort Axis, ref int Encoder, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Fall_Encoder_Net")]
        public static extern short MCF_Get_Home_Fall_Encoder_Net(ushort Axis, ref int Encoder, ushort StationNumber = 0);
        //5.10 轴状态清除函数                               						 宏定义0.0    [0,99]   
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Axis_State_Net")]
        public static extern short MCF_Clear_Axis_State_Net(ushort Axis, ushort StationNumber = 0);
        //5.11 轴状态触发停止运动查询函数                             				宏定义0.0           MC_Retrun.h[0,28]      [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Axis_State_Net")]
        public static extern short MCF_Get_Axis_State_Net(ushort Axis, ref short Reason, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              6 轴回原点函数
        ********************************************************************************************************************************************************************/
        //6.1 设置回零参数                                 							 宏定义0.0    [1,35]                  宏定义6.1.1         宏定义6.1.2       宏定义6.1.3         (0,10M]P/S     (0,10M]P/S       [-2^31,(2^31-1)]     [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Set_Net")]
        public static extern short MCF_Search_Home_Set_Net(ushort Axis, ushort Search_Home_Mode, ushort Limit_Logic, ushort Home_Logic, ushort Index_Logic, double H_dMaxV, double L_dMaxV, int Offset_Position, ushort Trigger_Source, ushort StationNumber = 0);
        //6.2 设置回零启动                                  						 宏定义0.0   [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Start_Net")]
        public static extern short MCF_Search_Home_Start_Net(ushort Axis, ushort StationNumber = 0);
        //6.3 设置回零停止                                  						 宏定义0.0   [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Stop_Net")]
        public static extern short MCF_Search_Home_Stop_Net(ushort Axis, ushort StationNumber = 0);
        //6.4 获取回零状态                                       					 宏定义0.0   MC_Retrun.h{0,31,32}  [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Get_State_Net")]
        public static extern short MCF_Search_Home_Get_State_Net(ushort Axis, ref ushort Home_State, ushort StationNumber = 0);
        //6.5 设置回零缓停时间                                  					 	 宏定义0.0     [0,1000] 单位：ms 		[0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Stop_Time_Net")]
        public static extern short MCF_Search_Home_Stop_Time_Net(ushort Axis, ushort Stop_Time, ushort StationNumber = 0);
        //6.6 设置回零完成后保持位置值                                       		 宏定义0.0            [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Keep_Position_Net")]
        public static extern short MCF_Search_Home_Keep_Position_Net(ushort Axis, ushort StationNumber = 0);
        //6.7 设置回零完成后保持编码器值                                  			 宏定义0.0            [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Keep_Encoder_Net")]
        public static extern short MCF_Search_Home_Keep_Encoder_Net(ushort Axis, ushort StationNumber = 0);
        //6.8 设置回零在零点位置离开速度                       宏定义0.0            [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Leave_Vel_Net")]
        public static extern short MCF_Search_Home_Leave_Vel_Net(ushort Axis, double M_dMaxV, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                              7 点位运动控制函数
        ********************************************************************************************************************************************************************/
        //7.1 速度控制函数                     										 宏定义0.0    (0,10M]P/S   (0,1T]P^2/S    [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_JOG_Net")]
        public static extern short MCF_JOG_Net(ushort Axis, double dMaxV, double dMaxA, ushort StationNumber = 0);
        //7.2 单轴运动位置改变函数                              					 	宏定义0.0    [-2^31,(2^31-1)]   宏定义0.3      [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_dDist_Change_Net")]
        public static extern short MCF_Uniaxial_dDist_Change_Net(ushort Axis, int dDist, ushort Position_Mode, ushort StationNumber = 0);
        //7.3 单轴运动速度改变函数                              					 	宏定义0.0    (0,10M]P/S    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_dMaxV_Change_Net")]
        public static extern short MCF_Uniaxial_dMaxV_Change_Net(ushort Axis, double dMaxV, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_dMaxA_Change_Net")]
        public static extern short MCF_Uniaxial_dMaxA_Change_Net(ushort Axis, double dMaxA, ushort StationNumber = 0);
        //7.4 单轴曲线函数                                  						 宏定义0.0    [0,dMaxV]      (0,10M]P/S   (0,1T]P^2/S   (0,100T]P^3/S [0,dMaxV]       宏定义0.4       [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Axis_Profile_Net")]
        public static extern short MCF_Set_Axis_Profile_Net(ushort Axis, double dV_ini, double dMaxV, double dMaxA, double dJerk, double dV_end, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Axis_Profile_Net")]
        public static extern short MCF_Get_Axis_Profile_Net(ushort Axis, ref double dV_ini, ref double dMaxV, ref double dMaxA, ref double dJerk, ref double dV_end, ref ushort Profile, ushort StationNumber = 0);
        //7.5 单轴运动函数                          								 宏定义0.0   [-2^31,(2^31-1)]  宏定义0.3       [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_Net")]
        public static extern short MCF_Uniaxial_Net(ushort Axis, int dDist, ushort Position_Mode, ushort StationNumber = 0);
        //7.6 单轴停止曲线函数                                  					 	宏定义0.0     (0,1T]P^2/S  (0,100T]P^3/S  宏定义0.4       [0,99]   
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Axis_Stop_Profile_Net")]
        public static extern short MCF_Set_Axis_Stop_Profile_Net(ushort Axis, double dMaxA, double dJerk, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Axis_Stop_Profile_Net")]
        public static extern short MCF_Get_Axis_Stop_Profile_Net(ushort Axis, ref double dMaxA, ref double dJerk, ref ushort Profile, ushort StationNumber = 0);
        //7.7 轴停止函数                             								 宏定义0.0    宏定义7.7              [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Axis_Stop_Net")]
        public static extern short MCF_Axis_Stop_Net(ushort Axis, ushort Axis_Stop_Mode, ushort StationNumber = 0);
        //7.8 单轴运动改变周期函数                             						宏定义0.0           [1,1000]MS           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_Cycle_Change_Net")]
        public static extern short MCF_Uniaxial_Cycle_Change_Net(ushort Axis, ushort Cycle, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              8 插补运动控制函数
        ********************************************************************************************************************************************************************/
        //8.1 坐标系曲线函数                                     					 宏定义0.1           [0,dMaxV]     (0,10M]P/S    (0,1T]P^2/S   (0,100T]P^3/S  [0,dMaxV]      宏定义0.4       [0,99]     
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Coordinate_Profile_Net")]
        public static extern short MCF_Set_Coordinate_Profile_Net(ushort Coordinate, double dV_ini, double dMaxV, double dMaxA, double dJerk, double dV_end, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Coordinate_Profile_Net")]
        public static extern short MCF_Get_Coordinate_Profile_Net(ushort Coordinate, ref double dV_ini, ref double dMaxV, ref double dMaxA, ref double dJerk, ref double dV_end, ref ushort Profile, ushort StationNumber = 0);
        //8.2 圆半径插补运动函数                      								 宏定义0.1          宏定义0.0            [-2^31,(2^31-1)]    [-2^31,(2^31-1)]   宏定义0.5         宏定义0.3            [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Arc2_Radius_Net")]
        public static extern short MCF_Arc2_Radius_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, int Arc_Radius, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.3 圆圆心插补运动函数                      								 宏定义0.1          宏定义0.0             [-2^31,(2^31-1)]       [-2^31,(2^31-1)]    宏定义0.5         宏定义0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Arc2_Centre_Net")]
        public static extern short MCF_Arc2_Centre_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ref int Center_List, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.4 直线插补运动函数                   									 宏定义0.1          宏定义0.0              [-2^31,(2^31-1)]   宏定义0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Line2_Net")]
        public static extern short MCF_Line2_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Line3_Net")]
        public static extern short MCF_Line3_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Line4_Net")]
        public static extern short MCF_Line4_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        //8.5 坐标系停止曲线函数                                       				 宏定义0.1                 (0,1T]P^2/S   (0,100T]P^3/S  宏定义0.4
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Coordinate_Stop_Profile_Net")]
        public static extern short MCF_Set_Coordinate_Stop_Profile_Net(ushort Coordinate, double dMaxA, double dJerk, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Coordinate_Stop_Profile_Net")]
        public static extern short MCF_Get_Coordinate_Stop_Profile_Net(ushort Coordinate, ref double dMaxA, ref double dJerk, ref ushort Profile, ushort StationNumber = 0);
        //8.6 螺旋线圆半径插补运动函数                         宏定义0.1               宏定义0.0                 [-2^31,(2^31-1)] [-2^31,(2^31-1)]  宏定义0.5                 宏定义0.3                    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Screw3_Radius_Net")]
        public static extern short MCF_Screw3_Radius_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, int Arc_Radius, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.7 螺旋线圆圆心插补运动函数                         宏定义0.1               宏定义0.0                 [-2^31,(2^31-1)] [-2^31,(2^31-1)]  宏定义0.5                 宏定义0.3                    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Screw3_Centre_Net")]
        public static extern short MCF_Screw3_Centre_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ref int Center_List, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.8 坐标系停止函数                                   宏定义0.1              宏定义5.6                           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Coordinate_Stop_Net")]
        public static extern short MCF_Coordinate_Stop_Net(ushort Coordinate, ushort Coordinate_Stop_Mode, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              9 缓冲区函数
        ********************************************************************************************************************************************************************/
        //9.1 缓冲区停止曲线函数                                   					 宏定义0.2             (0,1T]P^2/S  (0,100T]P^3/S  宏定义0.4       [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Stop_Profile_Net")]
        public static extern short MCF_Buffer_Set_Stop_Profile_Net(ushort Buffer_Number, double dMaxA, double dJerk, ushort Profile, ushort StationNumber = 0);
        //9.2 缓冲区停止函数                           								 宏定义0.2             宏定义9.2                [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Stop_Net")]
        public static extern short MCF_Buffer_Stop_Net(ushort Buffer_Number, ushort Buffer_Stop_Mode, ushort StationNumber = 0);
        //9.3 缓冲区在线改变速度倍率                           						宏定义0.2                    (0,10]                [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Change_Velocity_Ratio_Net")]
        public static extern short MCF_Buffer_Change_Velocity_Ratio_Net(ushort Buffer_Number, double Velocity_Ratio, ushort StationNumber = 0);
        //9.4 缓冲区建立开始函数                        							 	宏定义0.2             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Start_Net")]
        public static extern short MCF_Buffer_Start_Net(ushort Buffer_Number, ushort StationNumber = 0);
        //9.5 缓冲区速度倍率                                   						宏定义0.2                    宏定义9.5                                [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Velocity_Ratio_Enable_Net")]
        public static extern short MCF_Buffer_Set_Velocity_Ratio_Enable_Net(ushort Buffer_Number, ushort Velocity_Ratio_Enable = 0, ushort StationNumber = 0);
        //9.6 缓冲区前瞻处理降速比                                 					 宏定义0.2             (0,1]                      [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Reduce_Ratio_Net")]
        public static extern short MCF_Buffer_Set_Reduce_Ratio_Net(ushort Buffer_Number, double Reduce_Ratio = 1.0, ushort StationNumber = 0);
        //9.7 缓冲区曲线函数                                  						 宏定义0.2             [0,dMaxV]     (0,10M]P/S    (0,1T]P^2/S   (0,100T]P^3/S  [0,dMaxV]      宏定义0.4       [0,99]   
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Profile_Net")]
        public static extern short MCF_Buffer_Set_Profile_Net(ushort Buffer_Number, double dV_ini, double dMaxV, double dMaxA, double dJerk, double dV_end, ushort Profile, ushort StationNumber = 0);
        //9.8 缓冲区单轴运动                               							 宏定义0.2             宏定义0.0    [-2^31,(2^31-1)]  宏定义0.3      [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Uniaxial_Net")]
        public static extern short MCF_Buffer_Uniaxial_Net(ushort Buffer_Number, ushort Axis, int dDist, ushort Position_Mode, ushort StationNumber = 0);
        //缓冲区单轴运动距离同步跟随函数  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Sync_Follow_Net")]
        public static extern short MCF_Buffer_Sync_Follow_Net(ushort Buffer_Number, ushort Axis, int dDist, ushort StationNumber = 0);
        //9.9 缓冲区直线插补运动                        							 	宏定义0.2             宏定义0.0              [-2^31,(2^31-1)]   宏定义0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Line2_Net")]
        public static extern short MCF_Buffer_Line2_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Line3_Net")]
        public static extern short MCF_Buffer_Line3_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Line4_Net")]
        public static extern short MCF_Buffer_Line4_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        //9.10 缓冲区平面圆半径插补运动函数                      						 宏定义0.2             宏定义0.0              [-2^31,(2^31-1)]  [-2^31,(2^31-1)]   宏定义0.5         宏定义0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Arc_Radius_Net")]
        public static extern short MCF_Buffer_Arc_Radius_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, int Arc_Radius, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //9.11 缓冲区平面圆圆心插补运动函数                      						 宏定义0.2             宏定义0.0             [-2^31,(2^31-1)]       [-2^31,(2^31-1)]   宏定义0.5         宏定义0.3              [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Arc_Centre_Net")]
        public static extern short MCF_Buffer_Arc_Centre_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ref int Center_List, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //9.12 缓冲区延时函数                           							 	宏定义0.2             [0,2^31-1]   [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Delay_Net")]
        public static extern short MCF_Buffer_Delay_Net(ushort Buffer_Number, uint number, ushort StationNumber = 0);
        //9.13 缓冲区IO输出函数                                 					 	宏定义0.2             宏定义2.3.1         宏定义2.3.2   [0,99]     
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Output_Bit_Net")]
        public static extern short MCF_Buffer_Set_Output_Bit_Net(ushort Buffer_Number, ushort Bit_Number, ushort output, ushort StationNumber = 0);
        //9.14 缓冲区IO等待函数                                  					 宏定义0.2             宏定义2.4.1        宏定义2.4.2  (0,2^15-1]       [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Wait_Input_Bit_Net")]
        public static extern short MCF_Buffer_Wait_Input_Bit_Net(ushort Buffer_Number, ushort Bit_Number, ushort Logic, ushort Time_Out, ushort StationNumber = 0);
        //9.15 缓冲区建立结束                         								 宏定义0.2             [1,2^31-1]           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_End_Net")]
        public static extern short MCF_Buffer_End_Net(ushort Buffer_Number, ref uint Command_Number, ushort StationNumber = 0);
        //9.16 缓冲区执行函数                             							 宏定义0.2             宏定义9.16           [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Execute_Net")]
        public static extern short MCF_Buffer_Execute_Net(ushort Buffer_Number, ushort Execute_Mode, ushort StationNumber = 0);
        //9.17 缓冲区断点启动函数                              						宏定义0.2                    [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Execute_BreakPoint_Net")]
        public static extern short MCF_Buffer_Execute_BreakPoint_Net(ushort Buffer_Number, ushort StationNumber = 0);
        //9.18 缓冲区状态查询函数                              						宏定义0.2                    MC_Retrun.h{0,29,30}                   [0,2^15-1]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Get_State_Net")]
        public static extern short MCF_Buffer_Get_State_Net(ushort Buffer_Number, ref ushort Execute_State, ref ushort Execute_Number, ushort StationNumber = 0);
        //9.19 缓冲区可填充指令查询                            						宏定义0.2 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Get_Command_Remained_Net")]
        public static extern short MCF_Buffer_Get_Command_Remained_Net(ushort Buffer_Number, ref ushort Command_Number, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              10 示波器10K采样频率数据捕捉函数
        ********************************************************************************************************************************************************************/
        //10.1 数据捕捉打开/关闭函数(必须在MCF_Open_Net前面提前调用,而且只支持一个运动控制卡)                                    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Open_Net")]
        public static extern short MCF_Capture_Open_Net(ushort Capture_Mode = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Close_Net")]
        public static extern short MCF_Capture_Close_Net();
        //10.2 数据捕捉检查数据更新函数                       						 宏定义10.2            
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_State_Net")]
        public static extern short MCF_Capture_State_Net(ref ushort Capture_State);
        //10.3 读取采样连续的1000个位置命令数据                宏定义0.0           &Array[1000] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Read_Command_Net")]
        public static extern short MCF_Capture_Read_Command_Net(ushort Axis, ref int Command);
        //10.4 读取采样连续的1000个编码器数据                  宏定义0.0           &Array[1000]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Read_Encoder_Net")]
        public static extern short MCF_Capture_Read_Encoder_Net(ushort Axis, ref int Encoder);
        //10.5 读取采样连续的1000个模拟量数据                  宏定义0.0           &Array[1000]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Read_AD_Net")]
        public static extern short MCF_Capture_Read_AD_Net(ushort Axis, ref int AD);
        //10.6 ADC采样滤波                                                           宏定义0.0           [0,1]                       
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Filter_AD_Net")]
        public static extern short MCF_Capture_Filter_AD_Net(ushort Axis, double Filter_Coefficient = 1);
        //10.7 数据捕捉频率设置                                宏定义10.7       
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Frequency_Net")]
        public static extern short MCF_Capture_Frequency_Net(ushort Capture_Frequency = 1, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                              11 电子齿轮控制函数
        ********************************************************************************************************************************************************************/
        //11.1 电子齿轮设置函数                      								 宏定义0.0    宏定义0.0         (0,(2^31-1)]      (0,(2^31-1)]   宏定义11.1.1         宏定义11.1.2   [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Gear_Net")]
        public static extern short MCF_Set_Gear_Net(ushort Axis, ushort Follow_Axis, uint Denominator, uint Molecule, ushort Follow_Source, ushort Dir, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Gear_Net")]
        public static extern short MCF_Get_Gear_Net(ushort Axis, ref ushort Follow_Axis, ref uint Denominator, ref uint Molecule, ref ushort Follow_Source, ref ushort Dir, ushort StationNumber = 0);
        //11.2 电子齿轮开关函数                                 					 	宏定义0.0  宏定义11.2         [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Gear_Enable_Net")]
        public static extern short MCF_Set_Gear_Enable_Net(ushort Axis, ushort Gear_Enable, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Gear_Enable_Net")]
        public static extern short MCF_Get_Gear_Enable_Net(ushort Axis, ref ushort Gear_Enable, ushort StationNumber = 0);
        //11.3 电子齿轮运动距离后自动关闭                      宏定义0.0           [-2^31,(2^31-1)] [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Gear_Auto_Disable_Net")]
        public static extern short MCF_Set_Gear_Auto_Disable_Net(ushort Axis, int dDist, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              12 位置比较输出函数
        ********************************************************************************************************************************************************************/
        //12.1 设置一维位置比较器                            				    	宏定义0.0
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Compare_Config_Net")]
        public static extern short MCF_Set_Compare_Config_Net(ushort Axis, ushort Enable, ushort Compare_Source, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Config_Net")]
        public static extern short MCF_Get_Compare_Config_Net(ushort Axis, ref ushort Enable, ref ushort Compare_Source, ushort StationNumber = 0);
        //12.2 清除一维位置所有/当前比较点/关闭任意点          宏定义0.0
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Compare_Points_Net")]
        public static extern short MCF_Clear_Compare_Points_Net(ushort Axis, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Compare_Current_Points_Net")]
        public static extern short MCF_Clear_Compare_Current_Points_Net(ushort Axis, ushort StationNumber = 0);
        //    按照 MCF_Add_Compare_Point_Net 数据累加计算          宏定义0.0           [1,(2^31-1)}
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Disable_Compare_Any_Points_Net")]
        public static extern short MCF_Disable_Compare_Any_Points_Net(ushort Axis, ulong Point_Number, ushort StationNumber = 0);
        //12.3 添加一维位置比较点                            				    	宏定义0.0
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Add_Compare_Point_Net")]
        public static extern short MCF_Add_Compare_Point_Net(ushort Axis, int Position, ushort Dir, ushort Action, ushort Actpara, ushort StationNumber = 0);
        //12.4 读取当前一维比较点位置                            				    宏定义0.0 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Current_Point_Net")]
        public static extern short MCF_Get_Compare_Current_Point_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        //12.5 查询已经比较过的一维比较点个数(注意数据溢出)    宏定义0.0           [0,256]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Points_Runned_Net")]
        public static extern short MCF_Get_Compare_Points_Runned_Net(ushort Axis, ref ushort Point_Number, ushort StationNumber = 0);
        //12.6 查询可以加入的一维比较点个数                    宏定义0.0           [0,256]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Points_Remained_Net")]
        public static extern short MCF_Get_Compare_Points_Remained_Net(ushort Axis, ref ushort Point_Number, ushort StationNumber = 0);
        //12.7 查询所有未完成一维比较点个数和位置                            		宏定义0.0		    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Points_Incomplete_Net")]
        public static extern short MCF_Get_Compare_Points_Incomplete_Net(ushort Axis, ref ushort Incomplete_Number, ref long Incomplete_Position, ushort StationNumber = 0);



        /********************************************************************************************************************************************************************
                                                              13 PWM输出函数
        ********************************************************************************************************************************************************************/
        //13.1 设置PWM输出参数                                 						宏定义13.1.1            宏定义13.1.2           宏定义13.1.3                          				    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Pwm_Config_Net")]
        public static extern short MCF_Set_Pwm_Config_Net(ushort Channel, ushort Enable, ushort Output_Port_Config, ushort Output_Start_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Pwm_Config_Net")]
        public static extern short MCF_Get_Pwm_Config_Net(ushort Channel, ref ushort Enable, ref ushort Output_Port_Config, ref ushort Output_Start_Logic, ushort StationNumber = 0);
        //13.2 输出PWM信号                                     						宏定义13.1.1            [0,1000000]            [0,100]                  (0,(2^31-1)] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Pwm_Output_Net")]
        public static extern short MCF_Set_Pwm_Output_Net(ushort Channel, uint Frequency, uint DutyCycle, uint Pwm_Number, ushort StationNumber = 0);
        //13.3 PWM完成信号                                     						宏定义13.1.1            宏定义13.3.1 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Pwm_State_Net")]
        public static extern short MCF_Get_Pwm_State_Net(ushort Channel, ref ushort Finish, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              14 手轮函数
        ********************************************************************************************************************************************************************/
        //14.1 开启手轮功能                                    						宏定义11.1.2 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Open_Net")]
        public static extern short MCF_Hand_Wheel_Open_Net(ushort Dir, ushort StationNumber = 0);
        //14.2 关闭手轮功能                            				    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Close_Net")]
        public static extern short MCF_Hand_Wheel_Close_Net(ushort StationNumber = 0);
        //14.3 设置硬件手轮编码器通道                          						宏定义0.0                        
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_Encoder_Net")]
        public static extern short MCF_Hand_Wheel_Config_Encoder_Net(ushort Axis, ushort StationNumber = 0);
        //14.4 设置硬件手轮速率配置输入点                      						宏定义2.4.1
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_X1_Net")]
        public static extern short MCF_Hand_Wheel_Config_X1_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_X10_Net")]
        public static extern short MCF_Hand_Wheel_Config_X10_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_X100_Net")]
        public static extern short MCF_Hand_Wheel_Config_X100_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        //14.5 设置硬件手轮轴号配置输入点                      						宏定义0.0           宏定义2.4.1
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_Axis_Net")]
        public static extern short MCF_Hand_Wheel_Config_Axis_Net(ushort Axis, ushort Bit_Input_Number, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                              15 模拟量输入输出函数
        ********************************************************************************************************************************************************************/
        //15.1 读取单次ADC采样                                                      宏定义0.0           [-2^15,(2^15-1)]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Single_Read_AD_Net")]
        public static extern short MCF_Single_Read_AD_Net(ushort Channel, ref short AD, ushort StationNumber = 0);
        //15.2 读取单次DAC输出                                  					宏定义0.0           [-2^15,(2^15-1)]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Single_Write_DA_Net")]
        public static extern short MCF_Single_Write_DA_Net(ushort Channel, short DA, ushort StationNumber = 0);
        //15.3 设置AD双向比较器停止对应轴                                  					
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_AD_Compare_Net")]
        public static extern short MCF_Set_AD_Compare_Net(ushort Channel, short AD_Compare, ushort Stop_Axis, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                               16 系统函数
        ********************************************************************************************************************************************************************/
        //16.1 模块版本号                              								[0x00000000,0xFFFFFFFF] [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Version_Net")]
        public static extern short MCF_Get_Version_Net(ref uint Version, ushort StationNumber = 0);
        //16.2 序列号                                         						[0x00000000,0xFFFFFFFF] [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Serial_Number_Net")]
        public static extern short MCF_Get_Serial_Number_Net(ref long Serial_Number, ushort StationNumber = 0);
        //16.3 模块运行时间                                        					[0x00000000,0xFFFFFFFF] [0,99]    单位：秒  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Run_Time_Net")]
        public static extern short MCF_Get_Run_Time_Net(ref uint Run_Time, ushort StationNumber = 0);
        //16.4 Flash 读写功能目前暂时大小1Kbytes,也即定义一个 unsigned int Array[256] 存放数据
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Flash_Write_Net")]
        public static extern short MCF_Flash_Write_Net(uint Pass_Word_Setup, ref uint Flash_Write_Data, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Flash_Read_Net")]
        public static extern short MCF_Flash_Read_Net(uint Pass_Word_Check, ref uint Flash_Read_Data, ushort StationNumber = 0);
        //16.5 开启网络回路,一发一收，正常控制使用(默认)    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_LookBack_Enable_Net")]
        public static extern short MCF_LookBack_Enable_Net();
        //16.6 关闭网络回路，只发不收，测试老化模式下使用,或者检测各个级联模块是否工作  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_LookBack_Disable_Net")]
        public static extern short MCF_LookBack_Disable_Net();

    }
}