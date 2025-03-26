using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static MvCamCtrl.NET.MyCamera;


namespace SharpFrame.Common
{
    public class Haikang
    {
        private bool[] _mvsopen;

        public bool[] MvsOpen
        {
            get { return _mvsopen; }
            set { _mvsopen = value; }
        }
        public WriteableBitmap[] writeableBitmaps;
        private object[] m_BufForSaveImageLock;
        private MyCamera.cbOutputExdelegate cbImage;
        private MyCamera[] mycamera;
        private List<StringBuilder> devicelist = new List<StringBuilder>();
        private MyCamera.MV_DISPLAY_FRAME_INFO[] stDisplayInfo;
        /// <summary>
        /// WriteableBitmaps初始化完成通知
        /// </summary>
        public event Action<int, WriteableBitmap> WriteableBitmaps_Initialize;
        /// <summary>
        /// 设备列表
        /// </summary>
        private MyCamera.MV_CC_DEVICE_INFO_LIST deviceinfo = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        /// <summary>
        /// 设备信息
        /// </summary>
        private MyCamera.MV_CC_DEVICE_INFO[] devicemessage;

        /// <summary>
        /// 输出帧信息
        /// </summary>
        private MyCamera.MV_FRAME_OUT_INFO_EX[] frameinfo;
        /// <summary>
        /// 指令回调
        /// </summary>
        /// <param name="a"></param>
        public delegate bool EventCallbackfunction(int a);

        private IntPtr[] m_BufForDriver;

        private UInt32[] sizefordriver;

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        public IntPtr[] Pictorialhandle { get; set; }//获取外部图像控件的句柄

        public enum FetchModel
        {
            /// <summary>
            /// 连续取流
            /// </summary>
            Continuous,
            /// <summary>
            /// 单次
            /// </summary>
            Einmal
        }

        public enum Trigger
        {
            /// <summary>
            /// 外部触发
            /// </summary>
            Line0,
            /// <summary>
            /// 软触发
            /// </summary>
            Software
        }

        public enum Trigger_Model
        {
            上升沿,
            下降沿,
            高电平,
            低电平,
            上升或下降沿
        }

        private bool Mistake(int outint)
        {
            if (outint != 0 && outint != -1)
            {

                //MessageBox.Show($"相机api错误:{outint:x8}", "错误");
            }
            else if (outint < 0)
            {
                switch (outint)
                {
                    case -1:
                        //MessageBox.Show($"未查找到相机！", "错误");
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取在线设备信息
        /// </summary>
        /// <returns>List设备信息</returns>
        public List<StringBuilder> GetDevice()
        {
            deviceinfo.nDeviceNum = 0;//在线设备数量重置
            //ecallback = new EventCallbackfunction(Mistake);
            cbImage = new MyCamera.cbOutputExdelegate(ImageCallBack);
            MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE, ref deviceinfo);
            writeableBitmaps = new WriteableBitmap[deviceinfo.nDeviceNum];
            mycamera = new MyCamera[deviceinfo.nDeviceNum];
            Pictorialhandle = new IntPtr[deviceinfo.nDeviceNum];
            devicemessage = new MyCamera.MV_CC_DEVICE_INFO[deviceinfo.nDeviceNum];
            MvsOpen = new bool[deviceinfo.nDeviceNum];
            m_BufForDriver = new IntPtr[deviceinfo.nDeviceNum];
            sizefordriver = new uint[deviceinfo.nDeviceNum];
            stDisplayInfo = new MV_DISPLAY_FRAME_INFO[deviceinfo.nDeviceNum];
            frameinfo = new MyCamera.MV_FRAME_OUT_INFO_EX[deviceinfo.nDeviceNum];
            m_BufForSaveImageLock = new object[deviceinfo.nDeviceNum];
            for (int i = 0; i < deviceinfo.nDeviceNum; i++)
            {
                m_BufForSaveImageLock[i] = new object();
                sizefordriver[i] = 0;
                m_BufForDriver[i] = IntPtr.Zero;
                mycamera[i] = new MyCamera();
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceinfo.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));//获取设备信息
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO GetDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    if (GetDeviceInfo.chUserDefinedName != "")
                    {
                        devicelist.Add(new StringBuilder("GEV: " + GetDeviceInfo.chUserDefinedName + " (" + GetDeviceInfo.chSerialNumber + ")"));
                    }
                    else
                    {
                        devicelist.Add(new StringBuilder("GEV: " + GetDeviceInfo.chManufacturerName + " " + GetDeviceInfo.chModelName + " (" + GetDeviceInfo.chSerialNumber + ")"));
                    }
                }
                else
                {
                    Mistake(-1);
                }
            }
            if (devicelist.Count == 0)
            {
                Mistake(-1);
            }
            return devicelist;
        }

        private void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            int nIndex = (int)pUser;
            lock (m_BufForSaveImageLock[nIndex])
            {
                if (m_BufForDriver[nIndex] == IntPtr.Zero || pFrameInfo.nFrameLen > sizefordriver[nIndex])
                {
                    if (m_BufForDriver[nIndex] != IntPtr.Zero)
                    {
                        Marshal.Release(m_BufForDriver[nIndex]);
                        m_BufForDriver[nIndex] = IntPtr.Zero;
                    }

                    m_BufForDriver[nIndex] = Marshal.AllocHGlobal((Int32)pFrameInfo.nFrameLen);
                    if (m_BufForDriver[nIndex] == IntPtr.Zero)
                    {
                        return;
                    }
                    sizefordriver[nIndex] = pFrameInfo.nFrameLen;
                }
                frameinfo[nIndex] = pFrameInfo;
                CopyMemory(m_BufForDriver[nIndex], pData, pFrameInfo.nFrameLen);
            }
            Callback_conversion(nIndex, pFrameInfo);
            stDisplayInfo[nIndex] = new MyCamera.MV_DISPLAY_FRAME_INFO();
            stDisplayInfo[nIndex].hWnd = Pictorialhandle[nIndex];
            stDisplayInfo[nIndex].pData = pData;
            stDisplayInfo[nIndex].nDataLen = pFrameInfo.nFrameLen;
            stDisplayInfo[nIndex].nWidth = pFrameInfo.nWidth;
            stDisplayInfo[nIndex].nHeight = pFrameInfo.nHeight;
            stDisplayInfo[nIndex].enPixelType = pFrameInfo.enPixelType;
            //ConvertToWriteableBitmap(nIndex, pFrameInfo);
            mycamera[nIndex].MV_CC_DisplayOneFrame_NET(ref stDisplayInfo[nIndex]);

        }

        private Bitmap Callback_conversion(int indexes, MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            byte[] imageData = new byte[pFrameInfo.nFrameLen];
            Marshal.Copy(m_BufForDriver[indexes], imageData, 0, (int)pFrameInfo.nFrameLen);
            Bitmap bitmap = new Bitmap(pFrameInfo.nWidth, pFrameInfo.nHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(imageData, 0, bitmapData.Scan0, imageData.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        /// <summary>
        /// 打开指定相机
        /// </summary>
        /// <param name="indexes"></param>
        public void OpenDevice(int indexes)
        {
            try
            {
                devicemessage[indexes] = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceinfo.pDeviceInfo[indexes], typeof(MyCamera.MV_CC_DEVICE_INFO));
                Mistake(mycamera[indexes].MV_CC_CreateDevice_NET(ref devicemessage[indexes]));//创建设备
                Mistake(mycamera[indexes].MV_CC_OpenDevice_NET());//打开设备
                int packetsize = mycamera[indexes].MV_CC_GetOptimalPacketSize_NET();//获得网口相机的最佳packet size
                Mistake(mycamera[indexes].MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)packetsize));//设置packetsize
                Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS));
                mycamera[indexes].MV_CC_RegisterImageCallBackEx_NET(cbImage, (IntPtr)indexes);
                MvsOpen[indexes] = true;
            }
            catch (Exception)
            {
                MvsOpen[indexes] = false;
            }
        }

        /// <summary>
        /// 打开所有相机
        /// </summary>
        public void OpenDevice()
        {
            for (int i = 0; i < mycamera.Length; i++)
            {
                try
                {
                    devicemessage[i] = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceinfo.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    Mistake(mycamera[i].MV_CC_CreateDevice_NET(ref devicemessage[i]));//创建设备
                    Mistake(mycamera[i].MV_CC_OpenDevice_NET());//打开设备
                    int packetsize = mycamera[i].MV_CC_GetOptimalPacketSize_NET();//获得网口相机的最佳packet size
                    Mistake(mycamera[i].MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)packetsize));//设置packetsize
                    Mistake(mycamera[i].MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS));
                    mycamera[i].MV_CC_RegisterImageCallBackEx_NET(cbImage, (IntPtr)i);
                    MvsOpen[i] = true;
                }
                catch (Exception)
                {
                    MvsOpen[i] = false;
                }
            }
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        public void Gathercamera(int indexes, FetchModel fetch, Trigger trigger)
        {
            if (frameinfo != null && indexes < frameinfo.Length && indexes >= 0)
            {
                switch (fetch)
                {
                    case FetchModel.Continuous:
                        Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF));
                        break;
                    case FetchModel.Einmal:
                        Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON));
                        break;
                }
                switch (trigger)
                {
                    case Trigger.Line0:
                        Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0));
                        break;
                    case Trigger.Software:
                        Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE));
                        break;
                }
                frameinfo[indexes].nFrameLen = 0;//取流之前先清除帧长度
                frameinfo[indexes].enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                Mistake(mycamera[indexes].MV_CC_StartGrabbing_NET());
            }
        }

        /// <summary>
        /// 处理WPF中使用WriteableBitmap格式取流
        /// </summary>
        /// <param name="nIndex">相机id</param>
        /// <param name="pFrameInfo">图像回调指针</param>
        private void ConvertToWriteableBitmap(int nIndex, MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            if (writeableBitmaps[nIndex] == null)
            {
                System.Windows.Media.PixelFormat pixelFormat;
                switch (pFrameInfo.enPixelType)
                {
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                        // 对应 BGR 24 位格式
                        pixelFormat = PixelFormats.Bgr24;
                        break;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                        // 对应 8 位灰度格式
                        pixelFormat = PixelFormats.Gray8;
                        break;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                        // 对应 RGB 24 位格式
                        pixelFormat = PixelFormats.Rgb24;
                        break;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                        pixelFormat = PixelFormats.Bgr24;
                        break;
                    default:
                        pixelFormat = PixelFormats.Bgr24;
                        break;
                }
                writeableBitmaps[nIndex] = new WriteableBitmap((int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight, 96, 96, pixelFormat, null);
                //WriteableBitmaps_Initialize?.Invoke(nIndex, writeableBitmaps[nIndex]);
            }
            WriteableBitmap bitmap = writeableBitmaps[nIndex];
            try
            {
                bitmap.Lock();
                IntPtr backBuffer = bitmap.BackBuffer;
                int stride = bitmap.BackBufferStride;
                switch (pFrameInfo.enPixelType)
                {
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                        CopyMemory(backBuffer, m_BufForDriver[nIndex], (uint)pFrameInfo.nFrameLen);
                        break;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                        byte[] monoData = new byte[pFrameInfo.nFrameLen];
                        Marshal.Copy(m_BufForDriver[nIndex], monoData, 0, (int)pFrameInfo.nFrameLen);
                        if (bitmap.Format == PixelFormats.Bgr24)
                        {
                            byte[] bgrData = new byte[pFrameInfo.nWidth * pFrameInfo.nHeight * 3];
                            for (int i = 0; i < monoData.Length; i++)
                            {
                                byte value = monoData[i];
                                int index = i * 3;
                                bgrData[index] = value;
                                bgrData[index + 1] = value;
                                bgrData[index + 2] = value;
                            }
                            Marshal.Copy(bgrData, 0, backBuffer, bgrData.Length);
                        }
                        else if (bitmap.Format == PixelFormats.Gray8)
                        {
                            Marshal.Copy(monoData, 0, backBuffer, monoData.Length);
                        }
                        break;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                        CopyMemory(backBuffer, m_BufForDriver[nIndex], (uint)pFrameInfo.nFrameLen);
                        break;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                        CopyMemory(backBuffer, m_BufForDriver[nIndex], (uint)pFrameInfo.nFrameLen);
                        break;
                    default:
                        break;
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight));
            }
            finally
            {
                writeableBitmaps[nIndex].Unlock();
            }
        }

        public BitmapImage GetBitmapImage(int Index)
        {
            ConvertToWriteableBitmap(Index, frameinfo[Index]);
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(writeableBitmaps[Index]));
                encoder.Save(stream);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }


        private void SaveWriteableBitmapAsPng(WriteableBitmap bitmap, string filePath)
        {
            try
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
                Console.WriteLine($"Image saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="fetch"></param>
        /// <param name="trigger"></param>
        public void SetTriggerModel(int indexes, FetchModel fetch, Trigger trigger, Trigger_Model model)
        {
            switch (fetch)
            {
                case FetchModel.Continuous:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF));
                    break;
                case FetchModel.Einmal:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON));
                    break;
            }
            switch (trigger)
            {
                case Trigger.Line0:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0));
                    break;
                case Trigger.Software:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE));
                    break;
            }
            switch (model)
            {
                case Trigger_Model.上升沿:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerActivation", 0));
                    break;
                case Trigger_Model.下降沿:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerActivation", 1));
                    break;
                case Trigger_Model.高电平:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerActivation", 2));
                    break;
                case Trigger_Model.低电平:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerActivation", 3));
                    break;
                case Trigger_Model.上升或下降沿:
                    Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("TriggerActivation", 4));
                    break;
            }
        }

        /// <summary>
        /// 曝光设置
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="time"></param>
        public void SetExposure(int indexes, UInt32 time)
        {
            Mistake(mycamera[indexes].MV_CC_SetEnumValue_NET("ExposureTime", time));
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        public void StopGathercamera()
        {
            for (int i = 0; i < mycamera.Length; ++i)
            {
                mycamera[i].MV_CC_StopGrabbing_NET();
            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public void Close()
        {
            for (int i = 0; i < mycamera.Length; i++)
            {
                if (m_BufForDriver[i] != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver[i]);
                }
                mycamera[i].MV_CC_CloseDevice_NET();
                mycamera[i].MV_CC_DestroyDevice_NET();
                MvsOpen[i] = false;
            }
        }

        ///// <summary>
        ///// 采集彩色图像
        ///// </summary>
        ///// <param name="indexes">相机号</param>
        ///// <returns></returns>
        ///// <exception cref="AbandonedMutexException"></exception>
        //public Mat GetMatRGB(int indexes)
        //{
        //    try
        //    {
        //        var height = stDisplayInfo[indexes].nHeight;
        //        var width = stDisplayInfo[indexes].nWidth;
        //        Mat imat = new Mat(height, width, MatType.CV_8UC3, stDisplayInfo[indexes].pData);//三通道
        //        //Cv2.CvtColor(imat, imat, ColorConversionCodes.BGR2RGB);//将颜色通道BGR 8转换为RGB
        //        return imat;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //        throw new AbandonedMutexException("未将相机Image Format Control.Pixel Format中格式设置为BGR8");
        //    }
        //}

        ///// <summary>
        ///// 采集单通道图像
        ///// </summary>
        ///// <param name="indexes"></param>
        ///// <returns></returns>
        ///// <exception cref="AbandonedMutexException"></exception>
        //public Mat GetMat8UCV1(int indexes)
        //{
        //    try
        //    {
        //        var height = stDisplayInfo[indexes].nHeight;
        //        var width = stDisplayInfo[indexes].nWidth;
        //        Mat imat = new Mat(height, width, MatType.CV_8UC1, stDisplayInfo[indexes].pData);//单通道

        //        Cv2.CvtColor(imat, imat, ColorConversionCodes.BGR2RGB);//将颜色通道BGR 8转换为RGB
        //        GC.Collect();
        //        return imat;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //        throw new AbandonedMutexException("未将相机Image Format Control.Pixel Format中格式设置为BGR8");
        //    }
        //}

        //public void Arguments(int indexes, float arg)//曝光参数设置
        //{
        //    mycamera[indexes].MV_CC_SetEnumValue_NET("ExposureAuto", 0);
        //    Mistake(mycamera[indexes].MV_CC_SetFloatValue_NET("ExposureTime", arg));
        //}

        public BitmapImage bitmapToimage(Bitmap bitimg)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitimg.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
