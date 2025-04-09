using System.Runtime.InteropServices;

namespace MotionClass
{
    public class CMCDLL_NET
    {

        /********************************************************************************************************************************************************************
                                                               1 ���ƿ��򿪺���
        ********************************************************************************************************************************************************************/
        //1.1 ��ʼ������                        									    [1,100]                       [0,99]                     �궨��1.1 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Open_Net")]
        public static extern short MCF_Open_Net(ushort Connection_Number, ref ushort Station_Number, ref ushort Station_Type);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Close_Net")]
        public static extern short MCF_Close_Net();

        /********************************************************************************************************************************************************************
                                                              2 ͨ�������������
        ********************************************************************************************************************************************************************/
        //2.1 ͨ��IOȫ���������                               [OUT31,OUT0]                     [0,99]               
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Output_Net")]
        public static extern short MCF_Set_Output_Net(uint All_Output_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Output_Net")]
        public static extern short MCF_Get_Output_Net(ref uint All_Output_Logic, ushort StationNumber = 0);
        //2.2 ͨ��IO��λ�������                          							�궨��2.3.1        �궨��2.3.2  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Output_Bit_Net")]
        public static extern short MCF_Set_Output_Bit_Net(ushort Bit_Output_Number, ushort Bit_Output_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Output_Bit_Net")]
        public static extern short MCF_Get_Output_Bit_Net(ushort Bit_Output_Number, ref ushort Bit_Output_Logic, ushort StationNumber = 0);
        //2.3 ͨ��IO������ã���λ�������ʱ�亯��                       �궨��2.3.1                      �궨��2.3.2                      [0,65535]                       [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Output_Time_Bit_Net")]
        public static extern short MCF_Set_Output_Time_Bit_Net(ushort Bit_Output_Number, ushort Bit_Output_Logic, ushort Output_Time_1MS, ushort StationNumber = 0);
        //    ͨ��IO������ã���λ���������ʱ�亯��           �궨��2.3.1                           [0,1000]                          [-2^31,(2^31-1)]            [0,65535]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Compare_Output_Bit_Net")]
        public static extern short MCF_Set_Compare_Output_Bit_Net(ushort Compare_Output_Number, ushort Compare_Output_1MS, ushort Compare_dDist, ushort StationNumber = 0);

        //2.4 ͨ��IOȫ�����뺯��                               [Input31,Input0]                 [Input48,Input32]               [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Net")]
        public static extern short MCF_Get_Input_Net(ref uint All_Input_Logic1, ref uint All_Input_Logic2, ushort StationNumber = 0);
        //2.5 ͨ��IO��λ���뺯��                         							�궨��2.4.1        �궨��2.4.2  [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Bit_Net")]
        public static extern short MCF_Get_Input_Bit_Net(ushort Bit_Input_Number, ref ushort Bit_Input_Logic, ushort StationNumber = 0);
        //2.6 ͨ��IO��λ���������ظ��ٲ����������             [Bit_Input_0,Bit_Input_3]        [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Input_Fall_Bit_Net")]
        public static extern short MCF_Clear_Input_Fall_Bit_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        //2.7 ͨ��IO��λ�����½��ظ��ٲ����ȡ����             [Bit_Input_0,Bit_Input_3]        �궨��2.7                      [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Fall_Bit_Net")]
        public static extern short MCF_Get_Input_Fall_Bit_Net(ushort Bit_Input_Number, ref ushort Bit_Input_Fall, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              3 ��ר�������������
        ********************************************************************************************************************************************************************/
        //3.1 �ŷ�ʹ�����ú���                              						 �궨��0.0    �궨��3.1          [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Servo_Enable_Net")]
        public static extern short MCF_Set_Servo_Enable_Net(ushort Axis, ushort Servo_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_Enable_Net")]
        public static extern short MCF_Get_Servo_Enable_Net(ushort Axis, ref ushort Servo_Logic, ushort StationNumber = 0);
        //3.2 �ŷ�������λ���ú���                         							 �궨��0.0    �궨��3.2           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Servo_Alarm_Reset_Net")]
        public static extern short MCF_Set_Servo_Alarm_Reset_Net(ushort Axis, ushort Alarm_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_Alarm_Reset_Net")]
        public static extern short MCF_Get_Servo_Alarm_Reset_Net(ushort Axis, ref ushort Alarm_Logic, ushort StationNumber = 0);
        //3.3 �ŷ����������ȡ����                         							 �궨��0.0    �궨��3.3                    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_Alarm_Net")]
        public static extern short MCF_Get_Servo_Alarm_Net(ushort Axis, ref ushort Servo_Alarm_State, ushort StationNumber = 0);
        //3.4 �ŷ���λ��������ȡ����                   							 �궨��0.0    �궨��3.4                   [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Servo_INP_Net")]
        public static extern short MCF_Get_Servo_INP_Net(ushort Axis, ref ushort Servo_INP_State, ushort StationNumber = 0);
        //3.5 ������Z�������ȡ����              									 �궨��0.0    �궨��3.5           [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Z_Net")]
        public static extern short MCF_Get_Z_Net(ushort Axis, ref ushort Z_State, ushort StationNumber = 0);
        //3.6 ԭ�������ȡ����                      								 �궨��0.0    �궨��3.6              [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Net")]
        public static extern short MCF_Get_Home_Net(ushort Axis, ref ushort Home_State, ushort StationNumber = 0);
        //3.7 ����λ�����ȡ����                              						 �궨��0.0    �궨��3.7                        [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Positive_Limit_Net")]
        public static extern short MCF_Get_Positive_Limit_Net(ushort Axis, ref ushort Positive_Limit_State, ushort StationNumber = 0);
        //3.8 ����λ�����ȡ����                              						 �궨��0.0    �궨��3.8                        [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Negative_Limit_Net")]
        public static extern short MCF_Get_Negative_Limit_Net(ushort Axis, ref ushort Negative_Limit_State, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              4 �����ú���
        ********************************************************************************************************************************************************************/
        //4.1 ����ͨ��������ú���                        							 �궨��0.0    �궨��4.1  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Pulse_Mode_Net")]
        public static extern short MCF_Set_Pulse_Mode_Net(ushort Axis, uint Pulse_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Pulse_Mode_Net")]
        public static extern short MCF_Get_Pulse_Mode_Net(ushort Axis, ref uint Pulse_Mode, ushort StationNumber = 0);
        //4.2 λ�����ú��� 															 �궨��0.0    [-2^31,(2^31-1)]    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Position_Net")]
        public static extern short MCF_Set_Position_Net(ushort Axis, int Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Position_Net")]
        public static extern short MCF_Get_Position_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        //4.3 ���������ú���                          								 �궨��0.0    [-2^31,(2^31-1)]  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Encoder_Net")]
        public static extern short MCF_Set_Encoder_Net(ushort Axis, int Encoder, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Encoder_Net")]
        public static extern short MCF_Get_Encoder_Net(ushort Axis, ref int Encoder, ushort StationNumber = 0);
        //4.4 �ٶȻ�ȡ                            									 �궨��0.0    [-2^15,(2^15-1)]      [-2^15,(2^15-1)]        [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Vel_Net")]
        public static extern short MCF_Get_Vel_Net(ushort Axis, ref double Command_Vel, ref double Encode_Vel, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              5 ��Ӳ������ֹͣ�˶�����
        ********************************************************************************************************************************************************************/
        //5.1 5.1 ͨ��IO���븴�ã���Ϊ����ֹͣ����                 					�궨��2.4.1              �궨��5.1      [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_EMG_Bit_Net")]
        public static extern short MCF_Set_EMG_Bit_Net(ushort EMG_Input_Number, ushort EMG_Mode, ushort StationNumber = 0);
        //    ͨ��IO���븴�ã���Ϊ����ֹͣ                    [0,3]                   �궨��0.0            [Bit_Input_0,Bit_Input_15]       �궨��5.4                   [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Input_Trigger_Net")]
        public static extern short MCF_Set_Input_Trigger_Net(ushort Channel, ushort Axis, ushort Bit_Input_Number, uint Trigger_Mode, ushort StationNumber = 0);

        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Input_Trigger_Net")]
        public static extern short MCF_Get_Input_Trigger_Net(ushort Channel, ref ushort Axis, ref ushort Bit_Input_Number, ref uint Trigger_Mode, ushort StationNumber = 0);

        //5.2 �����λ�����˶�ֹͣ����                  							 �궨��0.0    [-2^31,2^31]P     >      [-2^31,2^31]P           [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Soft_Limit_Net")]
        public static extern short MCF_Set_Soft_Limit_Net(ushort Axis, int Positive_Position, int Negative_Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Soft_Limit_Net")]
        public static extern short MCF_Get_Soft_Limit_Net(ushort Axis, ref int Positive_Position, ref int Negative_Position, ushort StationNumber = 0);
        //5.3 �����λ�����˶�ֹͣ���غ���                     						 �궨��0.0    �궨��5.3               [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Soft_Limit_Enable_Net")]
        public static extern short MCF_Set_Soft_Limit_Enable_Net(ushort Axis, uint Soft_Limit_Enable, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Soft_Limit_Enable_Net")]
        public static extern short MCF_Get_Soft_Limit_Enable_Net(ushort Axis, ref uint Soft_Limit_Enable, ushort StationNumber = 0);
        //5.4 �ŷ����������˶�ֹͣ����                       						 �궨��0.0    �궨��5.4          [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Alarm_Trigger_Net")]
        public static extern short MCF_Set_Alarm_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Alarm_Trigger_Net")]
        public static extern short MCF_Get_Alarm_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.5 Index�����˶�ֹͣ����                          						 �궨��0.0    �궨��5.4         [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Index_Trigger_Net")]
        public static extern short MCF_Set_Index_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Index_Trigger_Net")]
        public static extern short MCF_Get_Index_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.6 ԭ�㴥���˶�ֹͣ����                          						 �궨��0.0     �궨��5.4         [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Home_Trigger_Net")]
        public static extern short MCF_Set_Home_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Trigger_Net")]
        public static extern short MCF_Get_Home_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.7 ����λ�����˶�ֹͣ����                       							 �궨��0.0    �궨��5.4          [0,99]    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_ELP_Trigger_Net")]
        public static extern short MCF_Set_ELP_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_ELP_Trigger_Net")]
        public static extern short MCF_Get_ELP_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.8 ����λ�����˶�ֹͣ����                       							 �궨��0.0    �궨��5.4          [0,99]    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_ELN_Trigger_Net")]
        public static extern short MCF_Set_ELN_Trigger_Net(ushort Axis, uint Trigger_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_ELN_Trigger_Net")]
        public static extern short MCF_Get_ELN_Trigger_Net(ushort Axis, ref uint Trigger_Mode, ushort StationNumber = 0);
        //5.9 ԭ�㴥��λ�ü�¼����	                           						 �궨��0.0   [-2^31,(2^31-1)]  [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Rise_Position_Net")]
        public static extern short MCF_Get_Home_Rise_Position_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Fall_Position_Net")]
        public static extern short MCF_Get_Home_Fall_Position_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Rise_Encoder_Net")]
        public static extern short MCF_Get_Home_Rise_Encoder_Net(ushort Axis, ref int Encoder, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Home_Fall_Encoder_Net")]
        public static extern short MCF_Get_Home_Fall_Encoder_Net(ushort Axis, ref int Encoder, ushort StationNumber = 0);
        //5.10 ��״̬�������                               						 �궨��0.0    [0,99]   
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Axis_State_Net")]
        public static extern short MCF_Clear_Axis_State_Net(ushort Axis, ushort StationNumber = 0);
        //5.11 ��״̬����ֹͣ�˶���ѯ����                             				�궨��0.0           MC_Retrun.h[0,28]      [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Axis_State_Net")]
        public static extern short MCF_Get_Axis_State_Net(ushort Axis, ref short Reason, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              6 ���ԭ�㺯��
        ********************************************************************************************************************************************************************/
        //6.1 ���û������                                 							 �궨��0.0    [1,35]                  �궨��6.1.1         �궨��6.1.2       �궨��6.1.3         (0,10M]P/S     (0,10M]P/S       [-2^31,(2^31-1)]     [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Set_Net")]
        public static extern short MCF_Search_Home_Set_Net(ushort Axis, ushort Search_Home_Mode, ushort Limit_Logic, ushort Home_Logic, ushort Index_Logic, double H_dMaxV, double L_dMaxV, int Offset_Position, ushort Trigger_Source, ushort StationNumber = 0);
        //6.2 ���û�������                                  						 �궨��0.0   [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Start_Net")]
        public static extern short MCF_Search_Home_Start_Net(ushort Axis, ushort StationNumber = 0);
        //6.3 ���û���ֹͣ                                  						 �궨��0.0   [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Stop_Net")]
        public static extern short MCF_Search_Home_Stop_Net(ushort Axis, ushort StationNumber = 0);
        //6.4 ��ȡ����״̬                                       					 �궨��0.0   MC_Retrun.h{0,31,32}  [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Get_State_Net")]
        public static extern short MCF_Search_Home_Get_State_Net(ushort Axis, ref ushort Home_State, ushort StationNumber = 0);
        //6.5 ���û��㻺ͣʱ��                                  					 	 �궨��0.0     [0,1000] ��λ��ms 		[0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Stop_Time_Net")]
        public static extern short MCF_Search_Home_Stop_Time_Net(ushort Axis, ushort Stop_Time, ushort StationNumber = 0);
        //6.6 ���û�����ɺ󱣳�λ��ֵ                                       		 �궨��0.0            [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Keep_Position_Net")]
        public static extern short MCF_Search_Home_Keep_Position_Net(ushort Axis, ushort StationNumber = 0);
        //6.7 ���û�����ɺ󱣳ֱ�����ֵ                                  			 �궨��0.0            [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Keep_Encoder_Net")]
        public static extern short MCF_Search_Home_Keep_Encoder_Net(ushort Axis, ushort StationNumber = 0);
        //6.8 ���û��������λ���뿪�ٶ�                       �궨��0.0            [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Search_Home_Leave_Vel_Net")]
        public static extern short MCF_Search_Home_Leave_Vel_Net(ushort Axis, double M_dMaxV, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                              7 ��λ�˶����ƺ���
        ********************************************************************************************************************************************************************/
        //7.1 �ٶȿ��ƺ���                     										 �궨��0.0    (0,10M]P/S   (0,1T]P^2/S    [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_JOG_Net")]
        public static extern short MCF_JOG_Net(ushort Axis, double dMaxV, double dMaxA, ushort StationNumber = 0);
        //7.2 �����˶�λ�øı亯��                              					 	�궨��0.0    [-2^31,(2^31-1)]   �궨��0.3      [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_dDist_Change_Net")]
        public static extern short MCF_Uniaxial_dDist_Change_Net(ushort Axis, int dDist, ushort Position_Mode, ushort StationNumber = 0);
        //7.3 �����˶��ٶȸı亯��                              					 	�궨��0.0    (0,10M]P/S    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_dMaxV_Change_Net")]
        public static extern short MCF_Uniaxial_dMaxV_Change_Net(ushort Axis, double dMaxV, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_dMaxA_Change_Net")]
        public static extern short MCF_Uniaxial_dMaxA_Change_Net(ushort Axis, double dMaxA, ushort StationNumber = 0);
        //7.4 �������ߺ���                                  						 �궨��0.0    [0,dMaxV]      (0,10M]P/S   (0,1T]P^2/S   (0,100T]P^3/S [0,dMaxV]       �궨��0.4       [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Axis_Profile_Net")]
        public static extern short MCF_Set_Axis_Profile_Net(ushort Axis, double dV_ini, double dMaxV, double dMaxA, double dJerk, double dV_end, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Axis_Profile_Net")]
        public static extern short MCF_Get_Axis_Profile_Net(ushort Axis, ref double dV_ini, ref double dMaxV, ref double dMaxA, ref double dJerk, ref double dV_end, ref ushort Profile, ushort StationNumber = 0);
        //7.5 �����˶�����                          								 �궨��0.0   [-2^31,(2^31-1)]  �궨��0.3       [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_Net")]
        public static extern short MCF_Uniaxial_Net(ushort Axis, int dDist, ushort Position_Mode, ushort StationNumber = 0);
        //7.6 ����ֹͣ���ߺ���                                  					 	�궨��0.0     (0,1T]P^2/S  (0,100T]P^3/S  �궨��0.4       [0,99]   
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Axis_Stop_Profile_Net")]
        public static extern short MCF_Set_Axis_Stop_Profile_Net(ushort Axis, double dMaxA, double dJerk, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Axis_Stop_Profile_Net")]
        public static extern short MCF_Get_Axis_Stop_Profile_Net(ushort Axis, ref double dMaxA, ref double dJerk, ref ushort Profile, ushort StationNumber = 0);
        //7.7 ��ֹͣ����                             								 �궨��0.0    �궨��7.7              [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Axis_Stop_Net")]
        public static extern short MCF_Axis_Stop_Net(ushort Axis, ushort Axis_Stop_Mode, ushort StationNumber = 0);
        //7.8 �����˶��ı����ں���                             						�궨��0.0           [1,1000]MS           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Uniaxial_Cycle_Change_Net")]
        public static extern short MCF_Uniaxial_Cycle_Change_Net(ushort Axis, ushort Cycle, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              8 �岹�˶����ƺ���
        ********************************************************************************************************************************************************************/
        //8.1 ����ϵ���ߺ���                                     					 �궨��0.1           [0,dMaxV]     (0,10M]P/S    (0,1T]P^2/S   (0,100T]P^3/S  [0,dMaxV]      �궨��0.4       [0,99]     
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Coordinate_Profile_Net")]
        public static extern short MCF_Set_Coordinate_Profile_Net(ushort Coordinate, double dV_ini, double dMaxV, double dMaxA, double dJerk, double dV_end, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Coordinate_Profile_Net")]
        public static extern short MCF_Get_Coordinate_Profile_Net(ushort Coordinate, ref double dV_ini, ref double dMaxV, ref double dMaxA, ref double dJerk, ref double dV_end, ref ushort Profile, ushort StationNumber = 0);
        //8.2 Բ�뾶�岹�˶�����                      								 �궨��0.1          �궨��0.0            [-2^31,(2^31-1)]    [-2^31,(2^31-1)]   �궨��0.5         �궨��0.3            [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Arc2_Radius_Net")]
        public static extern short MCF_Arc2_Radius_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, int Arc_Radius, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.3 ԲԲ�Ĳ岹�˶�����                      								 �궨��0.1          �궨��0.0             [-2^31,(2^31-1)]       [-2^31,(2^31-1)]    �궨��0.5         �궨��0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Arc2_Centre_Net")]
        public static extern short MCF_Arc2_Centre_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ref int Center_List, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.4 ֱ�߲岹�˶�����                   									 �궨��0.1          �궨��0.0              [-2^31,(2^31-1)]   �궨��0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Line2_Net")]
        public static extern short MCF_Line2_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Line3_Net")]
        public static extern short MCF_Line3_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Line4_Net")]
        public static extern short MCF_Line4_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        //8.5 ����ϵֹͣ���ߺ���                                       				 �궨��0.1                 (0,1T]P^2/S   (0,100T]P^3/S  �궨��0.4
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Coordinate_Stop_Profile_Net")]
        public static extern short MCF_Set_Coordinate_Stop_Profile_Net(ushort Coordinate, double dMaxA, double dJerk, ushort Profile, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Coordinate_Stop_Profile_Net")]
        public static extern short MCF_Get_Coordinate_Stop_Profile_Net(ushort Coordinate, ref double dMaxA, ref double dJerk, ref ushort Profile, ushort StationNumber = 0);
        //8.6 ������Բ�뾶�岹�˶�����                         �궨��0.1               �궨��0.0                 [-2^31,(2^31-1)] [-2^31,(2^31-1)]  �궨��0.5                 �궨��0.3                    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Screw3_Radius_Net")]
        public static extern short MCF_Screw3_Radius_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, int Arc_Radius, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.7 ������ԲԲ�Ĳ岹�˶�����                         �궨��0.1               �궨��0.0                 [-2^31,(2^31-1)] [-2^31,(2^31-1)]  �궨��0.5                 �궨��0.3                    [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Screw3_Centre_Net")]
        public static extern short MCF_Screw3_Centre_Net(ushort Coordinate, ref ushort Axis_List, ref int dDist_List, ref int Center_List, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //8.8 ����ϵֹͣ����                                   �궨��0.1              �궨��5.6                           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Coordinate_Stop_Net")]
        public static extern short MCF_Coordinate_Stop_Net(ushort Coordinate, ushort Coordinate_Stop_Mode, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              9 ����������
        ********************************************************************************************************************************************************************/
        //9.1 ������ֹͣ���ߺ���                                   					 �궨��0.2             (0,1T]P^2/S  (0,100T]P^3/S  �궨��0.4       [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Stop_Profile_Net")]
        public static extern short MCF_Buffer_Set_Stop_Profile_Net(ushort Buffer_Number, double dMaxA, double dJerk, ushort Profile, ushort StationNumber = 0);
        //9.2 ������ֹͣ����                           								 �궨��0.2             �궨��9.2                [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Stop_Net")]
        public static extern short MCF_Buffer_Stop_Net(ushort Buffer_Number, ushort Buffer_Stop_Mode, ushort StationNumber = 0);
        //9.3 ���������߸ı��ٶȱ���                           						�궨��0.2                    (0,10]                [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Change_Velocity_Ratio_Net")]
        public static extern short MCF_Buffer_Change_Velocity_Ratio_Net(ushort Buffer_Number, double Velocity_Ratio, ushort StationNumber = 0);
        //9.4 ������������ʼ����                        							 	�궨��0.2             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Start_Net")]
        public static extern short MCF_Buffer_Start_Net(ushort Buffer_Number, ushort StationNumber = 0);
        //9.5 �������ٶȱ���                                   						�궨��0.2                    �궨��9.5                                [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Velocity_Ratio_Enable_Net")]
        public static extern short MCF_Buffer_Set_Velocity_Ratio_Enable_Net(ushort Buffer_Number, ushort Velocity_Ratio_Enable = 0, ushort StationNumber = 0);
        //9.6 ������ǰհ�����ٱ�                                 					 �궨��0.2             (0,1]                      [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Reduce_Ratio_Net")]
        public static extern short MCF_Buffer_Set_Reduce_Ratio_Net(ushort Buffer_Number, double Reduce_Ratio = 1.0, ushort StationNumber = 0);
        //9.7 ���������ߺ���                                  						 �궨��0.2             [0,dMaxV]     (0,10M]P/S    (0,1T]P^2/S   (0,100T]P^3/S  [0,dMaxV]      �궨��0.4       [0,99]   
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Profile_Net")]
        public static extern short MCF_Buffer_Set_Profile_Net(ushort Buffer_Number, double dV_ini, double dMaxV, double dMaxA, double dJerk, double dV_end, ushort Profile, ushort StationNumber = 0);
        //9.8 �����������˶�                               							 �궨��0.2             �궨��0.0    [-2^31,(2^31-1)]  �궨��0.3      [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Uniaxial_Net")]
        public static extern short MCF_Buffer_Uniaxial_Net(ushort Buffer_Number, ushort Axis, int dDist, ushort Position_Mode, ushort StationNumber = 0);
        //�����������˶�����ͬ�����溯��  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Sync_Follow_Net")]
        public static extern short MCF_Buffer_Sync_Follow_Net(ushort Buffer_Number, ushort Axis, int dDist, ushort StationNumber = 0);
        //9.9 ������ֱ�߲岹�˶�                        							 	�궨��0.2             �궨��0.0              [-2^31,(2^31-1)]   �궨��0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Line2_Net")]
        public static extern short MCF_Buffer_Line2_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Line3_Net")]
        public static extern short MCF_Buffer_Line3_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Line4_Net")]
        public static extern short MCF_Buffer_Line4_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ushort Position_Mode, ushort StationNumber = 0);
        //9.10 ������ƽ��Բ�뾶�岹�˶�����                      						 �궨��0.2             �궨��0.0              [-2^31,(2^31-1)]  [-2^31,(2^31-1)]   �궨��0.5         �궨��0.3             [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Arc_Radius_Net")]
        public static extern short MCF_Buffer_Arc_Radius_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, int Arc_Radius, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //9.11 ������ƽ��ԲԲ�Ĳ岹�˶�����                      						 �궨��0.2             �궨��0.0             [-2^31,(2^31-1)]       [-2^31,(2^31-1)]   �궨��0.5         �궨��0.3              [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Arc_Centre_Net")]
        public static extern short MCF_Buffer_Arc_Centre_Net(ushort Buffer_Number, ref ushort Axis_List, ref int dDist_List, ref int Center_List, ushort Direction, ushort Position_Mode, ushort StationNumber = 0);
        //9.12 ��������ʱ����                           							 	�궨��0.2             [0,2^31-1]   [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Delay_Net")]
        public static extern short MCF_Buffer_Delay_Net(ushort Buffer_Number, uint number, ushort StationNumber = 0);
        //9.13 ������IO�������                                 					 	�궨��0.2             �궨��2.3.1         �궨��2.3.2   [0,99]     
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Set_Output_Bit_Net")]
        public static extern short MCF_Buffer_Set_Output_Bit_Net(ushort Buffer_Number, ushort Bit_Number, ushort output, ushort StationNumber = 0);
        //9.14 ������IO�ȴ�����                                  					 �궨��0.2             �궨��2.4.1        �궨��2.4.2  (0,2^15-1]       [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Wait_Input_Bit_Net")]
        public static extern short MCF_Buffer_Wait_Input_Bit_Net(ushort Buffer_Number, ushort Bit_Number, ushort Logic, ushort Time_Out, ushort StationNumber = 0);
        //9.15 ��������������                         								 �궨��0.2             [1,2^31-1]           [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_End_Net")]
        public static extern short MCF_Buffer_End_Net(ushort Buffer_Number, ref uint Command_Number, ushort StationNumber = 0);
        //9.16 ������ִ�к���                             							 �궨��0.2             �궨��9.16           [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Execute_Net")]
        public static extern short MCF_Buffer_Execute_Net(ushort Buffer_Number, ushort Execute_Mode, ushort StationNumber = 0);
        //9.17 �������ϵ���������                              						�궨��0.2                    [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Execute_BreakPoint_Net")]
        public static extern short MCF_Buffer_Execute_BreakPoint_Net(ushort Buffer_Number, ushort StationNumber = 0);
        //9.18 ������״̬��ѯ����                              						�궨��0.2                    MC_Retrun.h{0,29,30}                   [0,2^15-1]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Get_State_Net")]
        public static extern short MCF_Buffer_Get_State_Net(ushort Buffer_Number, ref ushort Execute_State, ref ushort Execute_Number, ushort StationNumber = 0);
        //9.19 �����������ָ���ѯ                            						�궨��0.2 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Buffer_Get_Command_Remained_Net")]
        public static extern short MCF_Buffer_Get_Command_Remained_Net(ushort Buffer_Number, ref ushort Command_Number, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              10 ʾ����10K����Ƶ�����ݲ�׽����
        ********************************************************************************************************************************************************************/
        //10.1 ���ݲ�׽��/�رպ���(������MCF_Open_Netǰ����ǰ����,����ֻ֧��һ���˶����ƿ�)                                    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Open_Net")]
        public static extern short MCF_Capture_Open_Net(ushort Capture_Mode = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Close_Net")]
        public static extern short MCF_Capture_Close_Net();
        //10.2 ���ݲ�׽������ݸ��º���                       						 �궨��10.2            
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_State_Net")]
        public static extern short MCF_Capture_State_Net(ref ushort Capture_State);
        //10.3 ��ȡ����������1000��λ����������                �궨��0.0           &Array[1000] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Read_Command_Net")]
        public static extern short MCF_Capture_Read_Command_Net(ushort Axis, ref int Command);
        //10.4 ��ȡ����������1000������������                  �궨��0.0           &Array[1000]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Read_Encoder_Net")]
        public static extern short MCF_Capture_Read_Encoder_Net(ushort Axis, ref int Encoder);
        //10.5 ��ȡ����������1000��ģ��������                  �궨��0.0           &Array[1000]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Read_AD_Net")]
        public static extern short MCF_Capture_Read_AD_Net(ushort Axis, ref int AD);
        //10.6 ADC�����˲�                                                           �궨��0.0           [0,1]                       
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Filter_AD_Net")]
        public static extern short MCF_Capture_Filter_AD_Net(ushort Axis, double Filter_Coefficient = 1);
        //10.7 ���ݲ�׽Ƶ������                                �궨��10.7       
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Capture_Frequency_Net")]
        public static extern short MCF_Capture_Frequency_Net(ushort Capture_Frequency = 1, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                              11 ���ӳ��ֿ��ƺ���
        ********************************************************************************************************************************************************************/
        //11.1 ���ӳ������ú���                      								 �궨��0.0    �궨��0.0         (0,(2^31-1)]      (0,(2^31-1)]   �궨��11.1.1         �궨��11.1.2   [0,99]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Gear_Net")]
        public static extern short MCF_Set_Gear_Net(ushort Axis, ushort Follow_Axis, uint Denominator, uint Molecule, ushort Follow_Source, ushort Dir, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Gear_Net")]
        public static extern short MCF_Get_Gear_Net(ushort Axis, ref ushort Follow_Axis, ref uint Denominator, ref uint Molecule, ref ushort Follow_Source, ref ushort Dir, ushort StationNumber = 0);
        //11.2 ���ӳ��ֿ��غ���                                 					 	�궨��0.0  �궨��11.2         [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Gear_Enable_Net")]
        public static extern short MCF_Set_Gear_Enable_Net(ushort Axis, ushort Gear_Enable, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Gear_Enable_Net")]
        public static extern short MCF_Get_Gear_Enable_Net(ushort Axis, ref ushort Gear_Enable, ushort StationNumber = 0);
        //11.3 ���ӳ����˶�������Զ��ر�                      �궨��0.0           [-2^31,(2^31-1)] [0,99] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Gear_Auto_Disable_Net")]
        public static extern short MCF_Set_Gear_Auto_Disable_Net(ushort Axis, int dDist, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              12 λ�ñȽ��������
        ********************************************************************************************************************************************************************/
        //12.1 ����һάλ�ñȽ���                            				    	�궨��0.0
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Compare_Config_Net")]
        public static extern short MCF_Set_Compare_Config_Net(ushort Axis, ushort Enable, ushort Compare_Source, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Config_Net")]
        public static extern short MCF_Get_Compare_Config_Net(ushort Axis, ref ushort Enable, ref ushort Compare_Source, ushort StationNumber = 0);
        //12.2 ���һάλ������/��ǰ�Ƚϵ�/�ر������          �궨��0.0
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Compare_Points_Net")]
        public static extern short MCF_Clear_Compare_Points_Net(ushort Axis, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Clear_Compare_Current_Points_Net")]
        public static extern short MCF_Clear_Compare_Current_Points_Net(ushort Axis, ushort StationNumber = 0);
        //    ���� MCF_Add_Compare_Point_Net �����ۼӼ���          �궨��0.0           [1,(2^31-1)}
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Disable_Compare_Any_Points_Net")]
        public static extern short MCF_Disable_Compare_Any_Points_Net(ushort Axis, ulong Point_Number, ushort StationNumber = 0);
        //12.3 ���һάλ�ñȽϵ�                            				    	�궨��0.0
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Add_Compare_Point_Net")]
        public static extern short MCF_Add_Compare_Point_Net(ushort Axis, int Position, ushort Dir, ushort Action, ushort Actpara, ushort StationNumber = 0);
        //12.4 ��ȡ��ǰһά�Ƚϵ�λ��                            				    �궨��0.0 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Current_Point_Net")]
        public static extern short MCF_Get_Compare_Current_Point_Net(ushort Axis, ref int Position, ushort StationNumber = 0);
        //12.5 ��ѯ�Ѿ��ȽϹ���һά�Ƚϵ����(ע���������)    �궨��0.0           [0,256]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Points_Runned_Net")]
        public static extern short MCF_Get_Compare_Points_Runned_Net(ushort Axis, ref ushort Point_Number, ushort StationNumber = 0);
        //12.6 ��ѯ���Լ����һά�Ƚϵ����                    �궨��0.0           [0,256]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Points_Remained_Net")]
        public static extern short MCF_Get_Compare_Points_Remained_Net(ushort Axis, ref ushort Point_Number, ushort StationNumber = 0);
        //12.7 ��ѯ����δ���һά�Ƚϵ������λ��                            		�궨��0.0		    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Compare_Points_Incomplete_Net")]
        public static extern short MCF_Get_Compare_Points_Incomplete_Net(ushort Axis, ref ushort Incomplete_Number, ref long Incomplete_Position, ushort StationNumber = 0);



        /********************************************************************************************************************************************************************
                                                              13 PWM�������
        ********************************************************************************************************************************************************************/
        //13.1 ����PWM�������                                 						�궨��13.1.1            �궨��13.1.2           �궨��13.1.3                          				    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Pwm_Config_Net")]
        public static extern short MCF_Set_Pwm_Config_Net(ushort Channel, ushort Enable, ushort Output_Port_Config, ushort Output_Start_Logic, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Pwm_Config_Net")]
        public static extern short MCF_Get_Pwm_Config_Net(ushort Channel, ref ushort Enable, ref ushort Output_Port_Config, ref ushort Output_Start_Logic, ushort StationNumber = 0);
        //13.2 ���PWM�ź�                                     						�궨��13.1.1            [0,1000000]            [0,100]                  (0,(2^31-1)] 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_Pwm_Output_Net")]
        public static extern short MCF_Set_Pwm_Output_Net(ushort Channel, uint Frequency, uint DutyCycle, uint Pwm_Number, ushort StationNumber = 0);
        //13.3 PWM����ź�                                     						�궨��13.1.1            �궨��13.3.1 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Pwm_State_Net")]
        public static extern short MCF_Get_Pwm_State_Net(ushort Channel, ref ushort Finish, ushort StationNumber = 0);

        /********************************************************************************************************************************************************************
                                                              14 ���ֺ���
        ********************************************************************************************************************************************************************/
        //14.1 �������ֹ���                                    						�궨��11.1.2 
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Open_Net")]
        public static extern short MCF_Hand_Wheel_Open_Net(ushort Dir, ushort StationNumber = 0);
        //14.2 �ر����ֹ���                            				    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Close_Net")]
        public static extern short MCF_Hand_Wheel_Close_Net(ushort StationNumber = 0);
        //14.3 ����Ӳ�����ֱ�����ͨ��                          						�궨��0.0                        
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_Encoder_Net")]
        public static extern short MCF_Hand_Wheel_Config_Encoder_Net(ushort Axis, ushort StationNumber = 0);
        //14.4 ����Ӳ�������������������                      						�궨��2.4.1
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_X1_Net")]
        public static extern short MCF_Hand_Wheel_Config_X1_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_X10_Net")]
        public static extern short MCF_Hand_Wheel_Config_X10_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_X100_Net")]
        public static extern short MCF_Hand_Wheel_Config_X100_Net(ushort Bit_Input_Number, ushort StationNumber = 0);
        //14.5 ����Ӳ������������������                      						�궨��0.0           �궨��2.4.1
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Hand_Wheel_Config_Axis_Net")]
        public static extern short MCF_Hand_Wheel_Config_Axis_Net(ushort Axis, ushort Bit_Input_Number, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                              15 ģ���������������
        ********************************************************************************************************************************************************************/
        //15.1 ��ȡ����ADC����                                                      �궨��0.0           [-2^15,(2^15-1)]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Single_Read_AD_Net")]
        public static extern short MCF_Single_Read_AD_Net(ushort Channel, ref short AD, ushort StationNumber = 0);
        //15.2 ��ȡ����DAC���                                  					�궨��0.0           [-2^15,(2^15-1)]
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Single_Write_DA_Net")]
        public static extern short MCF_Single_Write_DA_Net(ushort Channel, short DA, ushort StationNumber = 0);
        //15.3 ����AD˫��Ƚ���ֹͣ��Ӧ��                                  					
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Set_AD_Compare_Net")]
        public static extern short MCF_Set_AD_Compare_Net(ushort Channel, short AD_Compare, ushort Stop_Axis, ushort StationNumber = 0);
        /********************************************************************************************************************************************************************
                                                               16 ϵͳ����
        ********************************************************************************************************************************************************************/
        //16.1 ģ��汾��                              								[0x00000000,0xFFFFFFFF] [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Version_Net")]
        public static extern short MCF_Get_Version_Net(ref uint Version, ushort StationNumber = 0);
        //16.2 ���к�                                         						[0x00000000,0xFFFFFFFF] [0,99]  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Serial_Number_Net")]
        public static extern short MCF_Get_Serial_Number_Net(ref long Serial_Number, ushort StationNumber = 0);
        //16.3 ģ������ʱ��                                        					[0x00000000,0xFFFFFFFF] [0,99]    ��λ����  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Get_Run_Time_Net")]
        public static extern short MCF_Get_Run_Time_Net(ref uint Run_Time, ushort StationNumber = 0);
        //16.4 Flash ��д����Ŀǰ��ʱ��С1Kbytes,Ҳ������һ�� unsigned int Array[256] �������
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Flash_Write_Net")]
        public static extern short MCF_Flash_Write_Net(uint Pass_Word_Setup, ref uint Flash_Write_Data, ushort StationNumber = 0);
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_Flash_Read_Net")]
        public static extern short MCF_Flash_Read_Net(uint Pass_Word_Check, ref uint Flash_Read_Data, ushort StationNumber = 0);
        //16.5 ���������·,һ��һ�գ���������ʹ��(Ĭ��)    
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_LookBack_Enable_Net")]
        public static extern short MCF_LookBack_Enable_Net();
        //16.6 �ر������·��ֻ�����գ������ϻ�ģʽ��ʹ��,���߼���������ģ���Ƿ���  
        [DllImport("MCDLL_NET.DLL", EntryPoint = "MCF_LookBack_Disable_Net")]
        public static extern short MCF_LookBack_Disable_Net();

    }
}