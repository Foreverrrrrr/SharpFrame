using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpFrame.Common
{
    public class OpencvHandle
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Bitmap转ImageSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
        {
            // 获取 Bitmap 的句柄
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource imageSource;
            try
            {
                //创建 ImageSource 对象
                imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                // 释放 Bitmap 的句柄
                DeleteObject(hBitmap);
            }
            return imageSource;
        }
    }
}
