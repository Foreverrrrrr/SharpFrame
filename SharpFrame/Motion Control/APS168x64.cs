using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace MotionClass
{

    //ADLINK Structure++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_SAMP_DATA_4CH
    {
        public Int32 tick;
        public Int32 data0; //Total channel = 4
        public Int32 data1;
        public Int32 data2;
        public Int32 data3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOVE_PARA
    {
        public Int16 i16_accType;	//Axis parameter
        public Int16 i16_decType;	//Axis parameter
        public Int32 i32_acc;		//Axis parameter
        public Int32 i32_dec;		//Axis parameter
        public Int32 i32_initSpeed;	//Axis parameter
        public Int32 i32_maxSpeed;	//Axis parameter
        public Int32 i32_endSpeed;  //Axis parameter
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT_DATA
    {
        public Int32 i32_pos;       // Position data (relative or absolute) (pulse)
        public Int16 i16_accType;   // Acceleration pattern 0: T-curve,  1: S-curve
        public Int16 i16_decType;   // Deceleration pattern 0: T-curve,  1: S-curve
        public Int32 i32_acc;       // Acceleration rate ( pulse / ss )
        public Int32 i32_dec;       // Deceleration rate ( pulse / ss )
        public Int32 i32_initSpeed; // Start velocity	( pulse / s )
        public Int32 i32_maxSpeed;  // Maximum velocity  ( pulse / s )
        public Int32 i32_endSpeed;  // End velocity		( pulse / s )
        public Int32 i32_angle;     // Arc move angle    ( degree, -360 ~ 360 )
        public Int32 u32_dwell;     // Dwell times       ( unit: ms )
        public Int32 i32_opt;    	// Option //0xABCD , D:0 absolute, 1:relative
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA
    {
        // Point table structure (One dimension)
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.001 degree), [-360000,360000]
        public Int32 i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32 i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32 i32_ve;         // ending velocity (pulse/s), [0,2147484647]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_2D
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
        public Int32 i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32 i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32 i32_ve;         // ending velocity (pulse/s), [0,2147484647]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_2D_F64
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Double f64_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
        public Double f64_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Double f64_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Double f64_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Double f64_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Double f64_ve;         // ending velocity (pulse/s), [0,2147484647]
        public Double f64_sf;              // s-factor [0.0 ~ 1.0]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_4DL
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_z;          // z-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_u;          // u-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32 i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32 i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32 i32_ve;         // ending velocity (pulse/s), [0,2147484647]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT_DATA_EX
    {
        public Int32 i32_pos;           //(Center)Position data (could be relative or absolute value) 
        public Int16 i16_accType;       //Acceleration pattern 0: T curve, 1:S curve   
        public Int16 i16_decType;       // Deceleration pattern 0: T curve, 1:S curve 
        public Int32 i32_acc;           //Acceleration rate ( pulse / sec 2 ) 
        public Int32 i32_dec;           //Deceleration rate ( pulse / sec 2  ) 
        public Int32 i32_initSpeed;     //Start velocity ( pulse / s ) 
        public Int32 i32_maxSpeed;      //Maximum velocity    ( pulse / s ) 
        public Int32 i32_endSpeed;      //End velocity  ( pulse / s )     
        public Int32 i32_angle;         //Arc move angle ( degree, -360 ~ 360 ) 
        public UInt32 u32_dwell;        //dwell times ( unit: ms ) *Divided by system cycle time. 
        public Int32 i32_opt;           //Point move option. (*) 
        public Int32 i32_pitch;			// pitch for helical move
        public Int32 i32_totalheight;   // total hight
        public Int16 i16_cw;			// cw or ccw
        public Int16 i16_opt_ext;       // option extend
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINT_DATA2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int32[] i32_pos;                   // Position data (relative or absolute) (pulse)

        public Int32 i32_initSpeed;               // Start velocity	( pulse / s ) 
        public Int32 i32_maxSpeed;                // Maximum velocity  ( pulse / s ) 
        public Int32 i32_angle;                   // Arc move angle    ( degree, -360 ~ 360 ) 
        public UInt32 u32_dwell;                  // Dwell times       ( unit: ms ) 
        public Int32 i32_opt;                     // Option //0xABCD , D:0 absolute, 1:relative
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINT_DATA3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int32[] i32_pos;

        public Int32 i32_maxSpeed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Int32[] i32_endPos;

        public Int32 i32_dir;
        public Int32 i32_opt;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VAO_DATA
    {
        //Param
        public Int32 outputType;    //Output type, [0, 3]
        public Int32 inputType;     //Input type, [0, 1]
        public Int32 config;        //PWM configuration according to output type
        public Int32 inputSrc;      //Input source by axis, [0, 0xf]

        //Mapping table
        public Int32 minVel;                             //Minimum linear speed, [ positive ]
        public Int32 velInterval;                        //Speed interval, [ positive ]
        public Int32 totalPoints;                        //Total points, [1, 32]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] mappingDataArr;   //mapping data array
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTSTS
    {
        public UInt16 BitSts;           //b0: Is PTB work? [1:working, 0:Stopped]
                                        //b1: Is point buffer full? [1:full, 0:not full]
                                        //b2: Is point buffer empty? [1:empty, 0:not empty]
                                        //b3, b4, b5: Reserved for future
                                        //b6~: Be always 0
        public UInt16 PntBufFreeSpace;
        public UInt16 PntBufUsageSpace;
        public UInt32 RunningCnt;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LPSTS
    {
        public UInt32 MotionLoopLoading;
        public UInt32 HostLoopLoading;
        public UInt32 MotionLoopLoadingMax;
        public UInt32 HostLoopLoadingMax;
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct DEBUG_DATA
    {
        public UInt16 ServoOffCondition;
        public Double DspCmdPos;
        public Double DspFeedbackPos;
        public Double FpgaCmdPos;
        public Double FpgaFeedbackPos;
        public Double FpgaOutputVoltage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEBUG_STATE
    {
        public UInt16 AxisState;
        public UInt16 GroupState;
        public UInt16 AxisSuperState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTDWL
    {
        public Double DwTime; //Unit is ms
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTLINE
    {
        public Int32 Dim;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Double[] Pos;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTA2CA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] Index;       //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] Center;  //Center Arr

        public Double Angle;                          //Angle
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)]
    public struct PTA2CE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] Index; //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] Center; //

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] End; // 

        public Int16 Dir; //
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTA3CA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Normal; //Normal Arr

        public Double Angle;                         //Angle
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTA3CE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] End;    //End Arr

        public Int16 Dir; //
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTHCA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Normal; //Normal Arr

        public Double Angle;                         //Angle
        public Double DeltaH;
        public Double FinalR;
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
    public struct PTHCE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] Normal; //Normal Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Double[] End;    //End Arr

        public Int16 Dir; //
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PTINFO
    {
        public Int32 Dimension;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Int32[] AxisArr;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct STR_SAMP_DATA_8CH
    {
        public Int32 tick;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Int32[] data; //Total channel = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STR_SAMP_DATA_8CH_ASYNC
    {
        public Int32 tick;
        public Int32 data0;
        public Int32 data1;
        public Int32 data2;
        public Int32 data3;
        public Int32 data4;
        public Int32 data5;
        public Int32 data6;
        public Int32 data7;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAMP_PARAM
    {
        public Int32 rate;  //Sampling rate
        public Int32 edge;  //Trigger edge
        public Int32 level; //Trigger level
        public Int32 trigCh;    //Trigger channel

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int32[] sourceByCh;
        //Sampling source by channel. E.g.,
        // sourceByCh[0] --> Channel 0 sampling source number
        // sourceByCh[1] --> Chaneel 0 sampling axis number
        // sourceByCh[2] --> Channel 1 sampling source number
        // sourceByCh[3] --> Chaneel 1 sampling axis number
        // .....
        // sourceByCh[14] --> Channel 7 sampling source number
        // sourceByCh[15] --> Chaneel 7 sampling axis number 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOG_DATA
    {
        public Int16 i16_jogMode;	  // Jog mode. 0:Free running mode, 1:Step mode
        public Int16 i16_dir;		  // Jog direction. 0:positive, 1:negative direction
        public Int16 i16_accType;	  // Acceleration pattern 0: T-curve,  1: S-curve
        public Int32 i32_acc;		  // Acceleration rate ( pulse / ss )
        public Int32 i32_dec;		  // Deceleration rate ( pulse / ss )
        public Int32 i32_maxSpeed;	  // Positive value, maximum velocity  ( pulse / s )
        public Int32 i32_offset;	  // Positive value, a step (pulse)
        public Int32 i32_delayTime;  // Delay time, ( range: 0 ~ 65535 millisecond, align by cycle time)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HOME_PARA
    {
        public ushort u8_homeMode;
        public ushort u8_homeDir;
        public ushort u8_curveType;
        public Int32 i32_orgOffset;
        public Int32 i32_acceleration;
        public Int32 i32_startVelocity;
        public Int32 i32_maxVelocity;
        public Int32 i32_OrgVelocity;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ASYNCALL
    {
        public void* h_event;
        public Int32 i32_ret;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TSK_INFO
    {
        public UInt16 State;        // 
        public UInt16 RunTimeErr;     // 
        public UInt16 IP;
        public UInt16 SP;
        public UInt16 BP;
        public UInt16 MsgQueueSts;
    }

    //New ADCNC structure define
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D_F64
    {
        /* This structure extends original point data contents from "I32" to "F64" 
										   for internal computation. It's important to prevent data overflow. */
        public UInt32 u32_opt;        // option, [0x00000000, 0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-9223372036854775808, 9223372036854775807]
        public Double f64_y;          // y-axis component (pulse), [-9223372036854775808, 9223372036854775807]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000, 360000]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D_RPS
    {
        /* This structure adds another variable to record what point was be saved */
        public UInt32 u32_opt;        // option, [0x00000000, 0xFFFFFFFF]
        public Int32 i32_x;          // x-axis component (pulse), [-2147483648, 2147483647]
        public Int32 i32_y;          // y-axis component (pulse), [-2147483648, 2147483647]
        public Int32 i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000, 360000]
        public UInt32 crpi;              // current reading point index
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POS_DATA_2D_F64_RPS
    {
        /* This structure adds another variable to record what point was be saved */
        public UInt32 u32_opt;        // option, [0x00000000, 0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-2147483648, 2147483647]
        public Double f64_y;          // y-axis component (pulse), [-2147483648, 2147483647]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000, 360000]
        public UInt32 crpi;               // current reading point index
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PNT_DATA_2D_EXT
    {
        public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Double f64_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Double f64_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Double f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_acc; // acceleration rate (pulse/ss), [0,2147484647]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_dec; // deceleration rate (pulse/ss), [0,2147484647]		

        public Int32 crossover;
        public Int32 Iboundary;     // initial boundary

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_vi; // initial velocity (pulse/s), [0,2147484647]

        public UInt32 vi_cmpr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_vm; // maximum velocity (pulse/s), [0,2147484647]

        public UInt32 vm_cmpr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] f64_ve; // ending velocity (pulse/s), [0,2147484647]

        public UInt32 ve_cmpr;
        public Int32 Eboundary;     // end boundary		
        public Double f64_dist;     // point distance
        public Double f64_angle;        // path angle between previous & current point		
        public Double f64_radius;       // point radiua (used in arc move)
        public Int32 i32_arcstate;
        public UInt32 spt;          // speed profile type

        // unit time measured by DSP sampling period
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Double[] t;

        // Horizontal & Vertical line flag
        public Int32 HorizontalFlag;
        public Int32 VerticalFlag;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DO_DATA_EX
    {
        public UInt32 Do_ValueL;        //bit[0~31]
        public UInt32 Do_ValueH;        //bit[32~63]
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DI_DATA_EX
    {
        public UInt32 Di_ValueL;        //bit[0~31]
        public UInt32 Di_ValueH;        //bit[32~63]
    }

    //**********************************************
    // New header functions; 20151102
    //**********************************************
    [StructLayout(LayoutKind.Sequential)]
    public struct MCMP_POINT
    {
        public Double axisX; // x axis data for multi-dimension comparator 0
        public Double axisY; // y axis data for multi-dimension comparator 1
        public Double axisZ; // z axis data for multi-dimension comparator 2
        public Double axisU; // u axis data for multi-dimension comparator 3
        public UInt32 chInBit; // pwm output channel in bit format; 20150609
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    public struct EC_MODULE_INFO
    {
        public Int32 VendorID;
        public Int32 ProductCode;
        public Int32 RevisionNo;
        public Int32 TotalAxisNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Int32[] Axis_ID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Int32[] Axis_ID_manual;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] All_ModuleType;

        public Int32 DI_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] DI_ModuleType;

        public Int32 DO_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] DO_ModuleType;

        public Int32 AI_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] AI_ModuleType;

        public Int32 AO_ModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] AO_ModuleType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EC_Sub_MODULE_INFO
    {
        public Int32 VendorID;
        public Int32 ProductCode;
        public Int32 RevisionNo;
        public Int32 TotalSubModuleNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] SubModuleID;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EC_Sub_MODULE_OD_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] DataName;

        public Int32 BitLength;
        public Int32 DataType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] DataTypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PDO_OFFSET
    {
        public UInt16 DataType;
        public UInt32 ByteSize;
        public UInt32 ByteOffset;
        public UInt32 Index;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] NameArr;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OD_DESC_ENTRY
    {
        public UInt32 DataTypeNum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] DataTypeName;

        public UInt32 BitLen;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] Description;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] Access;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] PdoMapInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] UnitType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] DefaultValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] MinValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] MaxValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Speed_profile
    {
        public Int32 VS;        // start velocity ,range 1 ~ 4,000,000 (pulse)
        public Int32 Vmax;      // Maximum  velocity ,range 1 ~ 4,000,000
        public Int32 Acc;       // Acceleration ,range 1 ~ 500000000
        public Int32 Dec;       // Deceleration ,range 1 ~ 500000000
        public Double s_factor; // range 0 ~ 10

    }
    //	For latch function, 2019.06.10
    [StructLayout(LayoutKind.Sequential)]
    public struct LATCH_POINT
    {
        public Double position; 		// Latched position
        public Int32 ltcSrcInBit; 	// Latch source: bit 0~7: DI; bit 8~11: trigger channel
    }


    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++			
    public class APS168
    {
        // System & Initialization
        [DllImport("APS168x64.dll")] public static extern Int32 APS_initial(ref System.Int32 BoardID_InBits, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_close();
        [DllImport("APS168x64.dll")] public static extern Int32 APS_version();
        [DllImport("APS168x64.dll")] public static extern Int32 APS_device_driver_version(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_info(System.Int32 Axis_ID, ref System.Int32 Board_ID, ref System.Int32 Axis_No, ref System.Int32 Port_ID, ref System.Int32 Module_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_board_param(System.Int32 Board_ID, System.Int32 BOD_Param_No, System.Int32 BOD_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_board_param(System.Int32 Board_ID, System.Int32 BOD_Param_No, ref System.Int32 BOD_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_axis_param(System.Int32 Axis_ID, System.Int32 AXS_Param_No, System.Int32 AXS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_param(System.Int32 Axis_ID, System.Int32 AXS_Param_No, ref System.Int32 AXS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_device_info(System.Int32 Board_ID, System.Int32 Info_No, ref System.Int32 Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_card_name(System.Int32 Board_ID, ref System.Int32 CardName);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_disable_device(System.Int32 DeviceName);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_param_from_file(string pXMLFile);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_first_axisId(System.Int32 Board_ID, ref System.Int32 StartAxisID, ref System.Int32 TotalAxisNum);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_system_timer(System.Int32 Board_ID, ref System.Int32 Timer);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_system_loading(System.Int32 Board_ID, ref System.Double Loading1, ref System.Double Loading2, ref System.Double Loading3, ref System.Double Loading4);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_security_key(System.Int32 Board_ID, System.Int32 OldPassword, System.Int32 NewPassword);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_check_security_key(System.Int32 Board_ID, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_security_key(System.Int32 Board_ID);

        //Control driver mode [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_curr_sys_ctrl_mode(System.Int32 Axis_ID, ref System.Int32 Mode);

        //Virtual board settings [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_register_virtual_board(System.Int32 VirCardIndex, System.Int32 Count);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_virtual_board_info(System.Int32 VirCardIndex, ref System.Int32 Count);

        //Parameters setting by float [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_axis_param_f(System.Int32 Axis_ID, System.Int32 AXS_Param_No, System.Double AXS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_param_f(System.Int32 Axis_ID, System.Int32 AXS_Param_No, ref System.Double AXS_Param);

        //[For PCI-7856, MNET series]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_param_to_file(System.Int32 Board_ID, string pXMLFile);

        //Motion queue status [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mq_free_space(System.Int32 Axis_ID, ref System.Int32 Space);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mq_usage(System.Int32 Axis_ID, ref System.Int32 Usage);

        //Motion stop code [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_stop_code(System.Int32 Axis_ID, ref System.Int32 Code);

        //Helical interpolation [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_helix_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Int32 Pitch, System.Int32 TotalHeight, System.Int32 CwOrCcw);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_helix_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_PosOffset_Array, System.Int32 Max_Arc_Speed, System.Int32 Pitch, System.Int32 TotalHeight, System.Int32 CwOrCcw);

        //Helical interpolation [For PCI(e)-8154/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_helical_move(System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32[] End_Pos_Array, System.Int32 Pitch, System.Int32 Dir, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_helical_move(System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32[] End_Offset_Array, System.Int32 Pitch, System.Int32 Dir, System.Int32 Max_Speed);

        //Circular interpolation( Support 2D and 3D ) [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move_3pe(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Pass_Pos_Array, System.Int32[] End_Pos_Array, System.Int32 Max_Arc_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_arc_move_3pe(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Pass_PosOffset_Array, System.Int32[] End_PosOffset_Array, System.Int32 Max_Arc_Speed);

        //Field bus motion interrupt [For PCI-7856, MNET series]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_int_factor_motion(System.Int32 Axis_ID, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_int_factor_motion(System.Int32 Axis_ID, System.Int32 Factor_No, ref System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_int_factor_error(System.Int32 Axis_ID, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_int_factor_error(System.Int32 Axis_ID, System.Int32 Factor_No, ref System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_int_motion(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_field_bus_error_int_motion(System.Int32 Axis_ID, System.Int32 Time_Out);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_int_factor_di(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 bitsOfCheck);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_int_factor_di(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 bitsOfCheck);

        //Flash functions [For PCI-8253/56, PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_parameter_to_flash(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_parameter_from_flash(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_parameter_from_default(System.Int32 Board_ID);

        //SSCNET-3 functions [For PCI-8392(H)] 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_sscnet(System.Int32 Board_ID, ref System.Int32 AxisFound_InBits);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_sscnet(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_param(System.Int32 Axis_ID, System.Int32 Para_No1, ref System.Int32 Para_Dat1, System.Int32 Para_No2, ref System.Int32 Para_Dat2);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_servo_param(System.Int32 Axis_ID, System.Int32 Para_No1, System.Int32 Para_Dat1, System.Int32 Para_No2, System.Int32 Para_Dat2);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_alarm(System.Int32 Axis_ID, ref System.Int32 Alarm_No, ref System.Int32 Alarm_Detail);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_sscnet_servo_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_sscnet_servo_param(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_abs_position(System.Int32 Axis_ID, ref System.Int32 Cyc_Cnt, ref System.Int32 Res_Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_sscnet_servo_abs_position(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_sscnet_servo_abs_position(System.Int32 Axis_ID, System.Int32 Abs_Option, ref System.Int32 Cyc_Cnt, ref System.Int32 Res_Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_link_status(System.Int32 Board_ID, ref System.Int32 Link_Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_servo_monitor_src(System.Int32 Axis_ID, System.Int32 Mon_No, System.Int32 Mon_Src);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_monitor_src(System.Int32 Axis_ID, System.Int32 Mon_No, ref System.Int32 Mon_Src);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sscnet_servo_monitor_data(System.Int32 Axis_ID, System.Int32 Arr_Size, System.Int32[] Data_Arr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_control_mode(System.Int32 Axis_ID, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_abs_enable(System.Int32 Board_ID, System.Int32 Option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sscnet_abs_enable_by_axis(System.Int32 Axis_ID, System.Int32 Option);

        //Motion IO & motion status functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_status(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_io_status(System.Int32 Axis_ID);

        //Monitor functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command(System.Int32 Axis_ID, ref System.Int32 Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_command(System.Int32 Axis_ID, System.Int32 Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_servo_on(System.Int32 Axis_ID, System.Int32 Servo_On);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_position(System.Int32 Axis_ID, ref System.Int32 Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_position(System.Int32 Axis_ID, System.Int32 Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_velocity(System.Int32 Axis_ID, ref System.Int32 Velocity);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feedback_velocity(System.Int32 Axis_ID, ref System.Int32 Velocity);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_error_position(System.Int32 Axis_ID, ref System.Int32 Err_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_target_position(System.Int32 Axis_ID, ref System.Int32 Targ_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_f(System.Int32 Axis_ID, ref System.Double Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_command_f(System.Int32 Axis_ID, System.Double Command);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_position_f(System.Int32 Axis_ID, ref System.Double Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_position_f(System.Int32 Axis_ID, System.Double Position);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_velocity_f(System.Int32 Axis_ID, ref System.Double Velocity);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_target_position_f(System.Int32 Axis_ID, ref System.Double Targ_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_error_position_f(System.Int32 Axis_ID, ref System.Double Err_Pos);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feedback_velocity_f(System.Int32 Axis_ID, ref System.Double Velocity);

        // Single axis motion
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_velocity_move(System.Int32 Axis_ID, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_home_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_emg_stop(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move2(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 End_Speed, System.Int32 Acc_Rate, System.Int32 Dec_Rate);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move2(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 End_Speed, System.Int32 Acc_Rate, System.Int32 Dec_Rate);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_home_move2(System.Int32 Axis_ID, System.Int32 Dir, System.Int32 Acc, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 ORG_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_home_escape(System.Int32 Axis_ID);

        //JOG functions [For PCI-8392(H), PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_jog_param(System.Int32 Axis_ID, ref JOG_DATA pStr_Jog, System.Int32 Mask);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_jog_param(System.Int32 Axis_ID, ref JOG_DATA pStr_Jog);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_jog_mode_switch(System.Int32 Axis_ID, System.Int32 Turn_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_jog_start(System.Int32 Axis_ID, System.Int32 STA_On);

        // Interpolation
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_linear_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Int32 Angle);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_arc_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32 Max_Arc_Speed, System.Int32 Angle);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move_f(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Double Angle);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_arc_move_f(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32 Max_Arc_Speed, System.Double Angle);

        // Interrupt functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_int_enable(System.Int32 Board_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_int_factor(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_int_factor(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, ref System.Int32 Enable);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_int_factorH(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_int_no_to_handle(System.Int32 Int_No);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_single_int(System.Int32 Int_No, System.Int32 Time_Out);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_multiple_int(System.Int32 Int_Count, System.Int32[] Int_No_Array, System.Int32 Wait_All, System.Int32 Time_Out);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_int(System.Int32 Int_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_int(System.Int32 Int_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_error_int(System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Time_Out);


        //Sampling functions [For PCI-8392(H), PCI-8253/56, PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sampling_param(System.Int32 Board_ID, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_param(System.Int32 Board_ID, System.Int32 ParaNum, ref System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_4CH DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_async(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_4CH DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_count(System.Int32 Board_ID, ref System.Int32 SampCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_wait_sampling(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_auto_sampling(System.Int32 Board_ID, System.Int32 StartStop);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_data(System.Int32 Board_ID, ref System.Int32 Length, ref STR_SAMP_DATA_4CH DataArr, ref System.Int32 Status);

        //Sampling functions extension [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_sampling_param_ex(System.Int32 Board_ID, ref SAMP_PARAM Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_param_ex(System.Int32 Board_ID, ref SAMP_PARAM Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_ex(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, [Out] STR_SAMP_DATA_8CH[] DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_trigger_sampling_async_ex(System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_8CH_ASYNC DataArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_sampling_data_ex(System.Int32 Board_ID, ref System.Int32 Length, ref STR_SAMP_DATA_8CH DataArr, ref System.Int32 Status);

        //DIO & AIO functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_d_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_output(System.Int32 Board_ID, System.Int32 DO_Group, ref System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_input(System.Int32 Board_ID, System.Int32 DI_Group, ref System.Int32 DI_Data);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_d_channel_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 Ch_No, System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_channel_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 Ch_No, ref System.Int32 DO_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_d_channel_input(System.Int32 Board_ID, System.Int32 DI_Group, System.Int32 Ch_No, ref System.Int32 DI_Data);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_a_input_value(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Double Convert_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_a_input_data(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Int32 Raw_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_a_output_value(System.Int32 Board_ID, System.Int32 Channel_No, System.Double Convert_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_write_a_output_data(System.Int32 Board_ID, System.Int32 Channel_No, System.Int32 Raw_Data);
        //AIO [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_read_a_output_value(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Double Convert_Data);

        //Point table move functions [For PCI-8253/56, PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_point_table(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_running_point_index(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_start_point_index(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_end_point_index(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_pause(System.Int32 Axis_ID, System.Int32 Pause_en);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_repeat(System.Int32 Axis_ID, System.Int32 Repeat_en);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_table_move_repeat_count(System.Int32 Axis_ID, ref System.Int32 RepeatCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 StartIndex, System.Int32 EndIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_tableEx(System.Int32 Axis_ID, System.Int32 Index, ref PNT_DATA Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_tableEx_2D(System.Int32 Axis_ID, System.Int32 Axis_ID_2, System.Int32 Index, ref PNT_DATA_2D Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_4DL(System.Int32[] Axis_ID_Array, System.Int32 Index, ref PNT_DATA_4DL Point);

        //Point table + IO - Pause / Resume [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_ex_pause(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_ex_rollback(System.Int32 Axis_ID, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_table_move_ex_resume(System.Int32 Axis_ID);

        //Point table with extend option [For PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_ex(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA_EX Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_point_table_ex(System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA_EX Point);

        //Point table Feeder [For PCI-8253/56, PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_group(System.Int32 GroupId, System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_group(System.Int32 GroupId, ref System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_free_feeder_group(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_feeder_buffer(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_point_2D(System.Int32 GroupId, ref PNT_DATA_2D PtArray, System.Int32 Size, System.Int32 LastFlag);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_point_2D_ex(System.Int32 GroupId, ref PNT_DATA_2D_F64 PtArray, System.Int32 Size, System.Int32 LastFlag);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_feeder_move(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_status(System.Int32 GroupId, ref System.Int32 State, ref System.Int32 ErrCode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_running_index(System.Int32 GroupId, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_feeder_feed_index(System.Int32 GroupId, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_ex_pause(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_ex_rollback(System.Int32 GroupId, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_ex_resume(System.Int32 GroupId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_feeder_cfg_acc_type(System.Int32 GroupId, System.Int32 Type);

        //Point table functions [For MNET-4XMO-C]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_mode2(System.Int32 Axis_ID, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table2(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Index, ref POINT_DATA2 Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_continuous_move2(System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_single_move2(System.Int32 Axis_ID, System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_running_point_index2(System.Int32 Axis_ID, ref System.Int32 Index);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_status2(System.Int32 Axis_ID, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_setting_continuous_move2(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 TotalPoints, ref POINT_DATA2 Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table2_maximum_speed_check(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Index, ref POINT_DATA2 Point);

        //Point table functions [For HSL-4XMO]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table3(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Index, ref POINT_DATA3 Point);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_point_table_move3(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 StartIndex, System.Int32 EndIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_point_table_param3(System.Int32 FirstAxid, System.Int32 ParaNum, System.Int32 ParaDat);

        //Digital filter functions [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_filter_param(System.Int32 Axis_ID, System.Int32 Filter_paramNo, System.Int32 param_val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_filter_param(System.Int32 Axis_ID, System.Int32 Filter_paramNo, ref System.Int32 param_val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_device_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Info_No, ref System.Int32 Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_first_axisno(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 AxisNo, ref System.Int32 TotalAxes);

        //Field bus DIO slave functions [For PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_channel_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_channel_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_channel_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Int32 DI_Value);

        //Field bus AIO slave function
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_a_output_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value, System.Int16 RunStep);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_a_input_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value, System.Int16 RunStep);

        //Field bus comparing trigger functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_linear(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, System.Int32 StartPoint, System.Int32 RepeatTimes, System.Int32 Interval);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_table(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, System.Int32[] DataArr, System.Int32 ArraySize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_manual(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_trigger_manual_s(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgChInBit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_table_cmp(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_linear_cmp(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_trigger_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh, ref System.Int32 TrgCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_trigger_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_linear_cmp_remain_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_table_cmp_remain_count(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_encoder(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 EncCh, ref System.Int32 EncCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_encoder(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 EncCh, System.Int32 EncCnt);
        // Only support [For PCIe-8338 + EtherCAT 4xMO]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_timer_counter(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TmrCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_timer_counter(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TmrCh, System.Int32 Cnt);

        //Field bus latch functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_field_bus_ltc_fifo(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_point(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 ArraySize, ref LATCH_POINT LatchPoint);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_ltc_fifo_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_ltc_fifo(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_usage(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 Usage);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_free_space(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 FreeSpace);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ltc_fifo_status(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 FLtcCh, ref System.Int32 Status);



        // Comparing trigger functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_trigger_count(System.Int32 Board_ID, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_trigger_fifo_cmp(System.Int32 Board_ID, System.Int32 FCmpCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_fifo_cmp(System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_fifo_status(System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 FifoSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_fifo_data(System.Int32 Board_ID, System.Int32 FCmpCh, System.Int32[] DataArr, System.Int32 ArraySize, System.Int32 ShiftFlag);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_encoder_counter(System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 TrgCnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_encoder_counter(System.Int32 Board_ID, System.Int32 TrgCh, ref System.Int32 TrgCnt);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_timer(System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 Start);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_timer_counter(System.Int32 Board_ID, System.Int32 TmrCh, ref System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_timer_counter(System.Int32 Board_ID, System.Int32 TmrCh, System.Int32 Cnt);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_trigger_timer(System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 Start);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_timer_counter(System.Int32 Board_ID, System.Int32 TmrCh, ref System.Int32 TmrCnt);


        //VAO functions [For PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_param(System.Int32 Board_ID, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_vao_param(System.Int32 Board_ID, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_table(System.Int32 Board_ID, System.Int32 Table_No, System.Int32 MinVelocity, System.Int32 VelInterval, System.Int32 TotalPoints, System.Int32[] MappingDataArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_switch_vao_table(System.Int32 Board_ID, System.Int32 Table_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_vao(System.Int32 Board_ID, System.Int32 Output_Ch, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_vao_status(System.Int32 Board_ID, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_check_vao_param(System.Int32 Board_ID, System.Int32 Table_No, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_param_ex(System.Int32 Board_ID, System.Int32 Table_No, ref VAO_DATA VaoData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_vao_param_ex(System.Int32 Board_ID, System.Int32 Table_No, ref VAO_DATA VaoData);

        //Simultaneous move
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_relative_simultaneous_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32[] Max_Speed_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_absolute_simultaneous_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32[] Max_Speed_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_velocity_simultaneous_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Max_Speed_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_Release_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_release_simultaneous_move(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_emg_stop_simultaneous_move(System.Int32 Axis_ID);

        //Override functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_speed_override(System.Int32 Axis_ID, System.Int32 MaxSpeed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_ovrd(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_ovrd(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed);

        //Point table functions [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_dwell(System.Int32 Board_ID, System.Int32 PtbId, ref PTDWL Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_line(System.Int32 Board_ID, System.Int32 PtbId, ref PTLINE Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc2_ca(System.Int32 Board_ID, System.Int32 PtbId, ref PTA2CA Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc2_ce(System.Int32 Board_ID, System.Int32 PtbId, ref PTA2CE Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc3_ca(System.Int32 Board_ID, System.Int32 PtbId, ref PTA3CA Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_arc3_ce(System.Int32 Board_ID, System.Int32 PtbId, ref PTA3CE Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_spiral_ca(System.Int32 Board_ID, System.Int32 PtbId, ref PTHCA Prof, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_spiral_ce(System.Int32 Board_ID, System.Int32 PtbId, ref PTHCE Prof, ref PTSTS Status);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_enable(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 Dimension, System.Int32[] AxisArr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_disable(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pt_info(System.Int32 Board_ID, System.Int32 PtbId, ref PTINFO Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_vs(System.Int32 Board_ID, System.Int32 PtbId, System.Double Vs);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_get_vs(System.Int32 Board_ID, System.Int32 PtbId, ref System.Double Vs);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_start(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_stop(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pt_status(System.Int32 Board_ID, System.Int32 PtbId, ref PTSTS Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_pt_buffer(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_roll_back(System.Int32 Board_ID, System.Int32 PtbId, System.Double Max_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_get_error(System.Int32 Board_ID, System.Int32 PtbId, ref System.Int32 ErrCode);

        //Cmd buffer setting
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_ext_set_do_ch(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 Channel, System.Int32 OnOff);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_ext_set_table_no(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 CtrlNo, System.Int32 TableNo);

        //Profile buffer setting
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_absolute(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_relative(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_buffered(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_inp(System.Int32 Board_ID, System.Int32 PtbId);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_blend_dec(System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_blend_dist(System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_trans_blend_pcnt(System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_acc(System.Int32 Board_ID, System.Int32 PtbId, System.Double Acc);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_dec(System.Int32 Board_ID, System.Int32 PtbId, System.Double Dec);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_acc_dec(System.Int32 Board_ID, System.Int32 PtbId, System.Double AccDec);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_s(System.Int32 Board_ID, System.Int32 PtbId, System.Double Sf);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_vm(System.Int32 Board_ID, System.Int32 PtbId, System.Double Vm);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_set_ve(System.Int32 Board_ID, System.Int32 PtbId, System.Double Ve);

        //Program download functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_vmc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_vmc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_load_amc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_save_amc_program(System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_task_mode(System.Int32 Board_ID, System.Int32 TaskNum, System.Byte Mode, System.UInt16 LastIP);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_task_mode(System.Int32 Board_ID, System.Int32 TaskNum, ref System.Byte Mode, ref System.UInt16 LastIP);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_task(System.Int32 Board_ID, System.Int32 TaskNum, System.Int32 CtrlCmd);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_task_info(System.Int32 Board_ID, System.Int32 TaskNum, ref TSK_INFO Info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_task_msg(System.Int32 Board_ID, System.UInt16 QueueSts, ref System.UInt16 ActualSize, System.Byte[] CharArr);

        //Latch functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_encoder(System.Int32 Axis_ID, ref System.Int32 Encoder);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_latch_counter(System.Int32 Axis_ID, System.Int32 Src, ref System.Int32 Counter);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_latch_event(System.Int32 Axis_ID, System.Int32 Src, ref System.Int32 Event);

        //Raw command counter [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_counter(System.Int32 Axis_ID, ref System.Int32 Counter);

        //Reset raw command counter [For PCIe-8338]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_command_counter(System.Int32 Axis_ID);

        //Watch dog timer 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_start(System.Int32 Board_ID, System.Int32 TimerNo, System.Int32 TimeOut);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_get_timeout_period(System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 TimeOut);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_reset_counter(System.Int32 Board_ID, System.Int32 TimerNo);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_get_counter(System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 Counter);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_set_action_event(System.Int32 Board_ID, System.Int32 TimerNo, System.Int32 EventByBit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wdt_get_action_event(System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 EventByBit);

        //Multi-axes simultaneuos move start/stop [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_move_trigger(System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_move_multi(System.Int32 Dimension, System.Int32[] Axis_ID_Array);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_emg_stop_multi(System.Int32 Dimension, System.Int32[] Axis_ID_Array);

        //Gear/Gantry functions [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_gear(System.Int32 Axis_ID, System.Int32 Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gear_status(System.Int32 Axis_ID, ref System.Int32 Status);

        //Multi-latch functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_ltc_counter(System.Int32 Board_ID, System.Int32 CntNum, System.Int32 CntValue);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_counter(System.Int32 Board_ID, System.Int32 CntNum, ref System.Int32 CntValue);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_ltc_fifo_param(System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_param(System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_latch(System.Int32 Board_ID, System.Int32 LatchSignalInBits);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_ltc_fifo(System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_ltc_fifo(System.Int32 Board_ID, System.Int32 FLtcCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_data(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_usage(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Usage);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_free_space(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 FreeSpace);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_status(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Status);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ltc_fifo_point(System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 ArraySize, [In, Out] LATCH_POINT[] LatchPoint);

        //Single latch functions 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_latch2(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_latch_data2(System.Int32 Axis_ID, System.Int32 LatchNum, ref System.Int32 LatchData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_backlash_en(System.Int32 Axis_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_backlash_en(System.Int32 Axis_ID, ref System.Int32 Enable);

        //ODM functions for Mechatrolink
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_mlink(System.Int32 Board_ID, ref System.Int32 AxisFound_InBits);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_mlink(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_mlink_servo_param(System.Int32 Axis_ID, System.Int32 Para_No, System.Int32 Para_Dat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_servo_param(System.Int32 Axis_ID, System.Int32 Para_No, ref System.Int32 Para_Dat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_config_mlink(System.Int32 Board_ID, System.Int32 TotalAxes, ref System.Int32 AxesArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_rv_ptr(System.Int32 Axis_ID, out IntPtr rptr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_sd_ptr(System.Int32 Axis_ID, out IntPtr sptr);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_mlink_servo_alarm(System.Int32 Axis_ID, System.Int32 Alarm_No, ref System.Int32 Alarm_Detail);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_mlink_servo_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_mlink_pulse_per_rev(System.Int32 Axis_ID, System.Int32 PPR);

        //Apply smooth servo off [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_smooth_servo_off(System.Int32 Axis_ID, System.Double Decay_Rate);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_smooth_servo_off(System.Int32 Board_ID, System.Int32 Axis_ID, System.Int32 cnt_Max, ref System.Int32 cnt_Err);

        //ODM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_wait(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_wait(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_linear_move_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time, ref System.Int32 MotionSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_non_wait(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_non_wait(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_linear_move_non_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move_non_wait(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed, System.Int32 Time_Out, System.Int32 Delay_Time);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_wait_move_done(System.Int32 Axis_ID, ref System.Int32 MotionSts);

        //ODM functions [For MNET-4XMO-C]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_arc_move_ex(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32[] End_Pos_Array, System.Int32 CwOrCcw, System.Int32 Max_Arc_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_status_ex(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_motion_io_status_ex(System.Int32 Axis_ID);

        //Gantry functions [For PCI-8392(H), PCI-8253/56]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_gantry_param(System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_param(System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 ParaNum, ref System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_gantry_axis(System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 Master_Axis_ID, System.Int32 Slave_Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_axis(System.Int32 Board_ID, System.Int32 GroupNum, ref System.Int32 Master_Axis_ID, ref System.Int32 Slave_Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_error(System.Int32 Board_ID, System.Int32 GroupNum, ref System.Int32 GentryError);

        //Field bus master functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 BUS_Param_No, System.Int32 BUS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 BUS_Param_No, ref System.Int32 BUS_Param);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_field_bus(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 Start_Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_scan_field_bus(System.Int32 Board_ID, System.Int32 BUS_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_stop_field_bus(System.Int32 Board_ID, System.Int32 BUS_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_master_status(System.Int32 Board_ID, System.Int32 BUS_No, ref System.UInt32 Status);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_last_scan_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32[] Info_Array, System.Int32 Array_Size, ref System.Int32 Info_Count);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_master_type(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 BUS_Type);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_type(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 MOD_Type);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_name(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 MOD_Name);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_serialID(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int16 Serial_ID);

        //Field bus slave functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_slave_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, ref System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_slave_connect_quality(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 Sts_data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_slave_online_status(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 Live);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_slave_recovery(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No);


        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_ESC_register(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 RegOffset, System.Int32 DataSize, ref System.Int32 DataValue);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_ESC_register(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 RegOffset, System.Int32 DataSize, ref System.Int32 DataValue);


        //Field bus DIO slave functions [For PCI-8392(H)]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DI_Value);

        //Modules be 64 bits gpio
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_output_ex(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, DO_DATA_EX DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_output_ex(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref DO_DATA_EX DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_input_ex(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref DI_DATA_EX DI_Value);

        //Field bus AIO slave functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_a_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_a_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_a_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value);

        //ODM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_vao_by_mode(System.Int32 Board_ID, System.Int32 ChannelInBit, System.Int32 Mode, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_vao_pwm_burst_count(System.Int32 Board_ID, System.Int32 Table_No, System.Int32 Count);

        //PWM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pwm_width(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 Width);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pwm_width(System.Int32 Board_ID, System.Int32 PWM_Ch, ref System.Int32 Width);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pwm_frequency(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 Frequency);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pwm_frequency(System.Int32 Board_ID, System.Int32 PWM_Ch, ref System.Int32 Frequency);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pwm_on(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 PWM_On);

        // Comparing trigger functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_param(System.Int32 Board_ID, System.Int32 Param_No, System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_param(System.Int32 Board_ID, System.Int32 Param_No, ref System.Int32 Param_Val);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_linear(System.Int32 Board_ID, System.Int32 LCmpCh, System.Int32 StartPoint, System.Int32 RepeatTimes, System.Int32 Interval);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_table(System.Int32 Board_ID, System.Int32 TCmpCh, System.Int32[] DataArr, System.Int32 ArraySize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_manual(System.Int32 Board_ID, System.Int32 TrgCh);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_manual_s(System.Int32 Board_ID, System.Int32 TrgChInBit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_table_cmp(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_linear_cmp(System.Int32 Board_ID, System.Int32 LCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_count(System.Int32 Board_ID, System.Int32 TrgCh, ref System.Int32 TrgCnt);

        //Pulser counter functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pulser_counter(System.Int32 Board_ID, ref System.Int32 Counter);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pulser_counter(System.Int32 Board_ID, System.Int32 Counter);

        //Reserved functions [Legacy functions]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_slave_set_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, System.Int32 ParaDat);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_slave_get_param(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, ref System.Int32 ParaDat);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_d_set_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_d_get_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_d_get_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DI_Value);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_set_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_get_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_get_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_set_output_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value, System.Int16 RunStep);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_field_bus_A_get_input_plc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value, System.Int16 RunStep);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_eep_curr_drv_ctrl_mode(System.Int32 Board_ID, ref System.Int32 ModeInBit);

        //DPAC functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_rescan_CF(System.Int32 Board_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_battery_status(System.Int32 Board_ID, ref System.Int32 Battery_status);

        //DPAC display & Display button
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_display_data(System.Int32 Board_ID, System.Int32 displayDigit, ref System.Int32 displayIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_display_data(System.Int32 Board_ID, System.Int32 displayDigit, System.Int32 displayIndex);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_button_status(System.Int32 Board_ID, ref System.Int32 buttonstatus);

        //NV RAM functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_nv_ram(System.Int32 Board_ID, System.Int32 RamNo, System.Int32 DataWidth, System.Int32 Offset, System.Int32 Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_nv_ram(System.Int32 Board_ID, System.Int32 RamNo, System.Int32 DataWidth, System.Int32 Offset, ref System.Int32 Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_clear_nv_ram(System.Int32 Board_ID, System.Int32 RamNo);

        //Advanced single move & interpolation [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_ptp(System.Int32 Axis_ID, System.Int32 Option, System.Double Position, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_ptp_v(System.Int32 Axis_ID, System.Int32 Option, System.Double Position, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_ptp_all(System.Int32 Axis_ID, System.Int32 Option, System.Double Position, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_vel(System.Int32 Axis_ID, System.Int32 Option, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_vel_all(System.Int32 Axis_ID, System.Int32 Option, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_line(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_line_v(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_line_all(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc3_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_spiral_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);

        //Ring counter functions [For PCI-8154/8]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_ring_counter(System.Int32 Axis_ID, System.Int32 RingVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_ring_counter(System.Int32 Axis_ID, ref System.Int32 RingVal);



        //**********************************************
        // New header functions; 20151102
        //**********************************************

        //Pitch error compensation [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_pitch_table(System.Int32 Axis_ID, System.Int32 Comp_Type, System.Int32 Total_Points, System.Int32 MinPosition, System.UInt32 Interval, System.Int32[] Comp_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_pitch_table(System.Int32 Axis_ID, ref System.Int32 Comp_Type, ref System.Int32 Total_Points, ref System.Int32 MinPosition, ref System.UInt32 Interval, System.Int32[] Comp_Data);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_pitch_comp(System.Int32 Axis_ID, System.Int32 Enable);

        //2D compensation [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_2d_compensation_table(System.Int32[] AxisIdArray, System.UInt32 CompType, System.UInt32[] TotalPointArray, System.Double[] StartPosArray, System.Double[] IntervalArray, System.Double[] CompDataArrayX, System.Double[] CompDataArrayY);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_2d_compensation_table(System.Int32[] AxisIdArray, ref System.UInt32 CompType, System.UInt32[] TotalPointArray, System.Double[] StartPosArray, System.Double[] IntervalArray, System.Double[] CompDataArrayX, System.Double[] CompDataArrayY);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_start_2d_compensation(System.Int32 Axis_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_linear_move_2d_compensation(System.Int32[] Axis_ID_Array, System.Double[] Position_Array, System.Double Max_Linear_Speed);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_2d_compensation_command_position(System.Int32 Axis_ID, ref System.Double CommandX, ref System.Double CommandY, ref System.Double PositionX, ref System.Double PositionY);

        //20200120
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_trigger_table_data(System.Int32 Board_ID, System.Int32 TCmpCh, System.Int32[] DataArr, Int32 ArraySize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_table_status(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 FreeSpace, ref System.Int32 FifoSts);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_trigger_cmp_value(System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 CmpVal);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_enable_trigger_table(System.Int32 Board_ID, System.Int32 TCmpCh, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_trigger_table(System.Int32 Board_ID, System.Int32 TCmpCh);



        //Multi-dimension comparator functions [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_multi_trigger_table(System.Int32 Board_ID, System.Int32 Dimension, MCMP_POINT[] DataArr, System.Int32 ArraySize, System.Int32 Window);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_multi_trigger_table_cmp(System.Int32 Board_ID, System.Int32 Dimension, ref MCMP_POINT CmpVal);

        //Pulser functions
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_start(System.Int32 Board_ID, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_velocity_move(System.Int32 Axis_ID, System.Double SpeedLimit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_relative_move(System.Int32 Axis_ID, System.Double Distance, System.Double SpeedLimit);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_manual_pulser_home_move(System.Int32 Axis_ID);

        // [Wei-Li suggests to remove]
        //**********************************************
        // 2D arc-interpolation for 3-point
        [DllImport("APS168x64.dll")] public static extern Int32 APS_arc2_ct_all(System.Int32[] Axis_ID_Array, System.Int32 APS_Option, System.Double[] AnyArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        //**********************************************

        // [Reserved for unknown usage]
        //**********************************************
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_watch_timer(System.Int32 Board_ID, ref System.Int32 Timer);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_wdt(System.Int32 Board_ID, System.Int32 WDT_No);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_mapto_AxisID(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 AxisID);
        //**********************************************

        //for 8338 
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_module_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, [In, Out] EC_MODULE_INFO[] Module_info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_field_bus_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_alarm(System.Int32 Axis_ID, ref System.UInt32 AlarmCode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_pdo_offset(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, out IntPtr PPTx, ref System.UInt32 NumOfTx, out IntPtr PPRx, ref System.UInt32 NumOfRx);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_pdo(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt16 ByteOffset, System.UInt16 Size, ref System.UInt32 Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_pdo(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt16 ByteOffset, System.UInt16 Size, System.UInt32 Value);
        [DllImport("APS168x64.dll")]
        public static extern Int32 APS_get_field_bus_sdo(
                                                         System.Int32 Board_ID,
                                                         System.Int32 BUS_No,
                                                         System.Int32 MOD_No,
                                                         System.UInt16 ODIndex,
                                                         System.UInt16 ODSubIndex,
                                                         System.Byte[] Data,
                                                         System.UInt32 DataLen,
                                                         ref System.UInt32 OutDatalen,
                                                         System.UInt32 Timeout,
                                                         System.UInt32 Flags
                                                        );

        [DllImport("APS168x64.dll")]
        public static extern Int32 APS_set_field_bus_sdo(
                                                         System.Int32 Board_ID,
                                                         System.Int32 BUS_No,
                                                         System.Int32 MOD_No,
                                                         System.UInt16 ODIndex,
                                                         System.UInt16 ODSubIndex,
                                                         System.Byte[] Data,
                                                         System.UInt32 DataLen,
                                                         System.UInt32 Timeout,
                                                         System.UInt32 Flags
                                                        );

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_num(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.UInt16 Num, out IntPtr ODList);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_desc(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.UInt16 ODIndex, ref System.UInt16 MaxNumSubIndex, System.Byte[] Description, System.UInt32 Size);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_desc_entry(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.UInt16 ODIndex, System.UInt16 ODSubIndex, [In, Out] OD_DESC_ENTRY[] pOD_DESC_ENTRY);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_actual_torque(System.Int32 Axis_ID, ref System.Int32 Torque);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_d_port_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Port_No, System.UInt32 DO_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_port_input(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Port_No, ref System.UInt32 DI_Value);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_d_port_output(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Port_No, ref System.UInt32 DO_Value);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_circular_limit(System.Int32 Axis_A, System.Int32 Axis_B, System.Double Center_A, System.Double Center_B, System.Double Radius, System.Int32 Stop_Mode, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_circular_limit(System.Int32 Axis_A, System.Int32 Axis_B, ref System.Double Center_A, ref System.Double Center_B, ref System.Double Radius, ref System.Int32 Stop_Mode, ref System.Int32 Enable);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_loss_package(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Loss_Count);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_od_data(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 ODIndex, System.UInt32 RawData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_data(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 ODIndex, ref System.UInt32 RawData);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_module_info(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, [In, Out] EC_Sub_MODULE_INFO[] Sub_Module_info);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_number(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, ref System.Int32 TxODNum, ref System.Int32 RxODNum);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_tx(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 TxODIndex, [In, Out] EC_Sub_MODULE_OD_INFO[] Sub_MODULE_OD_INFO);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_od_rx(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 SubMOD_No, System.Int32 RxODIndex, [In, Out] EC_Sub_MODULE_OD_INFO[] Sub_MODULE_OD_INFO);

        // PVT function;
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_add_point(System.Int32 Axis_ID, System.Int32 ArraySize, System.Double[] PositionArray, System.Double[] VelocityArray, System.Double[] TimeArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_get_status(System.Int32 Axis_ID, ref System.Int32 FreeSize, ref System.Int32 PointCount, ref System.Int32 State);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_start(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pvt_reset(System.Int32 Axis_ID);

        // PT functions;
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_add_point(System.Int32 Axis_ID, System.Int32 ArraySize, System.Double[] PositionArray, System.Double[] TimeArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_get_status(System.Int32 Axis_ID, ref System.Int32 FreeSize, ref System.Int32 PointCount, ref System.Int32 State);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_start(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_pt_motion_reset(System.Int32 Axis_ID);

        //Get speed profile calculation
        [DllImport("APS168x64.dll")] public static extern Int32 APS_relative_move_profile(System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed, ref System.Int32 StrVel, ref System.Int32 MaxVel, ref System.Double Tacc, ref System.Double Tdec, ref System.Double Tconst);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_absolute_move_profile(System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed, ref System.Int32 StrVel, ref System.Int32 MaxVel, ref System.Double Tacc, ref System.Double Tdec, ref System.Double Tconst);

        //ASYNC mode
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_error_code(System.Int32 Axis_ID, System.UInt32 Index, ref System.Int32 ErrorCode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_cmd_fifo_usage(System.Int32 Axis_ID, ref System.UInt32 Number);

        //Get fpga latch value [For PCI-8254/58]
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_axis_latch_data(System.Int32 Axis_ID, System.Int32 latch_channel, ref System.Int32 latch_data);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_register_emx(System.Int32 emx_online, System.Int32 option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_deviceIP(System.Int32 Board_ID, ref string option);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_reset_emx_alarm(System.Int32 Axis_ID);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_check_motion_profile_emx(System.Int32 Axis_ID, ref Speed_profile profile_input, ref Speed_profile profile_output, ref System.Int32 MinDis);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_module_map(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt32[] MOD_No_Arr, System.UInt32 Size);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_module_map(System.Int32 Board_ID, System.Int32 BUS_No, System.UInt32[] MOD_No_Arr, System.UInt32 Size);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_analysis_topology(System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Error_Slave_No, [In, Out] EC_MODULE_INFO[] Current_slave_info, ref System.Int32 Current_slave_num, [In, Out] EC_MODULE_INFO[] Past_slave_info, ref System.Int32 Past_slave_num);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_number(System.Int32 Axis_ID, ref System.Int32 SlaveAxisIDSize);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_info(System.Int32 Axis_ID, System.Int32 SlaveAxisIDSize, System.Int32[] SlaveAxisIDArray);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_gantry_deviation(System.Int32 Axis_ID, System.Int32 SlaveAxisIDSize, System.Int32[] SlaveAxisIDArray, System.Double[] DeviationArray);

        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_field_bus_slave_state(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 State);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_field_bus_slave_state(System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 State);
        // Coordinate transform 20190624
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_coordTransform2D_config(System.Int32 Board_ID, System.Int32 AxisID_X, System.Int32 AxisID_Y, System.Double XYAngle, System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_coordTransform2D_config(System.Int32 Board_ID, ref System.Int32 AxisID_X, ref System.Int32 AxisID_Y, ref System.Double XYAngle, ref System.Int32 Enable);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_coordTransform2D_position(System.Int32 Board_ID, ref System.Double Cmd_transform_X, ref System.Int32 Cmd_transform_Y, ref System.Double Fbk_transform_X, ref System.Double Fbk_transform_Y);

        // Torque control
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_torque_command(System.Int32 Axis_ID, ref System.Int32 TorqueCmd);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_set_command_control_mode(System.Int32 Axis_ID, System.Byte Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_get_command_control_mode(System.Int32 Axis_ID, ref System.Byte Mode);
        [DllImport("APS168x64.dll")] public static extern Int32 APS_torque_move(System.Int32 Axis_ID, System.Int16 TorqueValue, System.UInt32 Slope, System.UInt16 Option, ref ASYNCALL Wait);

    }
}