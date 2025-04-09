using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace SharpFrame.Common
{
    public class OpencvHandle
    {
        /// <summary>
        /// 棋盘格标定结构体
        /// </summary>
        public struct Matrix_returns
        {
            /// <summary>
            /// 重投影误差
            /// </summary>
            public double Reprojection { get; set; }
            /// <summary>
            /// 像素/mm尺寸
            /// </summary>
            public double Pixel { get; set; }
            /// <summary>
            /// 相机内参矩阵
            /// </summary>
            public double[,] CameraMatrix { get; set; }
            /// <summary>
            /// 畸变系数向量
            /// </summary>
            public double[] DistCoeffs { get; set; }
        }

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

        private static FileInfo[] Get_CalibrationPicture(string calibration_picture_path)
        {
            FileInfo[] imageFiles = new FileInfo[0];
            if (Directory.Exists(calibration_picture_path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(calibration_picture_path);
                FileInfo[] allFiles = directoryInfo.GetFiles();
                string[] imageExtensions = { ".jpg", ".png", ".bmp" };
                imageFiles = allFiles.Where(file => imageExtensions.Contains(file.Extension.ToLower())).ToArray();
                return imageFiles;
            }
            else
            {
                throw new Exception($"{calibration_picture_path}文件夹不存在");
            }

        }




        /// <summary>
        /// 相机棋盘格标定
        /// </summary>
        /// <param name="calibration_picture_path">标定图片文件夹路径</param>
        /// <param name="angular_point_width">棋盘格横向角点数</param>
        /// <param name="angular_point_height">棋盘格竖向角点数</param>
        /// <param name="imagewidth">图片宽度像素</param>
        /// <param name="imageheigth">图片高度像素</param>
        /// <param name="pixelsize">像素/mm</param>
        /// <param name="cameramatrix">相机内参矩阵</param>
        /// <returns>返回一个 Matrix_returns 类型的对象，该对象包含了相机标定的相关结果：
        /// <list type="bullet">
        ///     <item><description>Reprojection：重投影误差，标定精度。</description></item>
        ///     <item><description>CameraMatrix：输入的相机内参矩阵。</description></item>
        ///     <item><description>DistCoeffs：包含 8 个元素的双精度数组，表示相机的畸变系数，用于描述相机成像过程中产生的畸变情况。</description></item>
        ///     <item><description>Pixel：通过对所有标定图片进行处理后计算得到的平均像素尺寸（单位：mm/像素），是根据棋盘格角点间距和图像上的角点距离计算得出的。</description></item>
        /// </list>
        /// 如果在标定过程中没有成功检测到任何棋盘格角点（即 imagePoints.Count 为 0），则会抛出一个 "角点查找失败！" 的异常。</returns>
        /// <exception cref="Exception">当角点查找失败时（即没有在任何一张标定图片中找到棋盘格角点），会抛出此异常，异常信息为 "角点查找失败！"</exception>
        public static Matrix_returns Calibration_Matrix(string calibration_picture_path, int angular_point_width, int angular_point_height, int imagewidth, int imageheigth, float pixelsize, double[,] cameramatrix)
        {
            Matrix_returns returns = new Matrix_returns();
            var imagefile = Get_CalibrationPicture(calibration_picture_path);
            int imagecount = imagefile.Length;
            // 存储所有图像的角点
            List<List<Point2f>> imagePoints = new List<List<Point2f>>();
            // 存储所有图像对应的世界坐标系中的点
            List<List<Point3f>> objectPoints = new List<List<Point3f>>();
            // 定义棋盘格在世界坐标系中的坐标
            List<Point3f> obj = new List<Point3f>();
            for (int i = 0; i < angular_point_height; i++)
            {
                for (int j = 0; j < angular_point_width; j++)
                {
                    obj.Add(new Point3f(j * pixelsize, i * pixelsize, 0));
                }
            }
            for (int i = 0; i < imagecount; i++)
            {
                string imapath = imagefile[i].FullName;
                using (Mat img = new Mat(imapath))
                {
                    using (Mat gray = new Mat())
                    {
                        Point2f[] corners;
                        Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
                        bool found = Cv2.FindChessboardCorners(gray, new OpenCvSharp.Size(angular_point_width, angular_point_height), out corners);
                        if (found)
                        {
                            // 亚像素级角点检测
                            var subpix = Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1), new TermCriteria(CriteriaTypes.MaxIter, 50, 0.0001));
                            //将检测到的角点添加到 imagePoints 列表中
                            imagePoints.Add(subpix.ToList());
                            //将对应的世界坐标系中的点添加到 objectPoints 列表中
                            objectPoints.Add(new List<Point3f>(obj));
                            Cv2.DrawChessboardCorners(img, new OpenCvSharp.Size(angular_point_width, angular_point_height), corners, true);
                            //Cv2.ImWrite(i.ToString() + ".bmp", img);
                        }
                    }
                }
            }
            var image_angular_point = ConvertToList(imagePoints);
            var object_point = ConvertToList(objectPoints);
            if (imagePoints.Count > 0)
            {
                double[] distCoeffs = new double[8]; // 完整形式
                Vec3d[] rvecs;
                Vec3d[] tvecs;
                var Reprojection = Cv2.CalibrateCamera(object_point, image_angular_point, new OpenCvSharp.Size(imagewidth, imageheigth), cameramatrix, distCoeffs, out rvecs, out tvecs);
                returns.Reprojection = Reprojection;
                returns.CameraMatrix = cameramatrix;
                returns.DistCoeffs = distCoeffs;
                double pixel = 0;
                for (int i = 0; i < imagefile.Length; i++)
                {
                    using (Mat imagetest = new Mat(imagefile[i].FullName))
                    {
                        using (Mat resulimg = new Mat())
                        {
                            var camera = ConvertToMat(cameramatrix);
                            var dist = ConvertToMat(distCoeffs);
                            Cv2.Undistort(imagetest, resulimg, camera, dist);//畸变矫正
                            using (Mat gratimg = new Mat())
                            {
                                Cv2.CvtColor(resulimg, gratimg, ColorConversionCodes.BGR2GRAY);
                                Point2f[] cornersdsit;
                                bool found = Cv2.FindChessboardCorners(gratimg, new OpenCvSharp.Size(angular_point_width, angular_point_height), out cornersdsit);
                                if (found)
                                {
                                    double sumdist = 0.0;
                                    for (int j = 0; j < angular_point_width - 1; j++)
                                    {
                                        double distance = Math.Sqrt(
                                            Math.Pow(cornersdsit[i + 1].X - cornersdsit[i].X, 2) +
                                            Math.Pow(cornersdsit[i + 1].Y - cornersdsit[i].Y, 2));
                                        sumdist += distance;
                                    }
                                    pixel += pixelsize / (sumdist / (angular_point_width - 1));
                                }
                            }
                        }
                    }
                }
                returns.Pixel = pixel / imagefile.Length;
                return returns;
            }
            else
            {
                throw new Exception("角点查找失败！");
            }
        }

        /// <summary>
        /// 相机内参矩阵计算
        /// </summary>
        /// <param name="focal_length">镜头焦距mm</param>
        /// <param name="pixel_size_width">相机像元尺寸x mm</param>
        /// <param name="pixel_size_heigth">相机像元尺寸y mm</param>
        /// <param name="imagewidth">图像尺寸w</param>
        /// <param name="imageheigth">图像尺寸h</param>
        /// <returns>返回一个 3x3 的二维数组，表示计算得到的相机内参矩阵。矩阵的形式为：
        /// <code>
        /// [
        ///     [fx, 0, imagewidth/2],
        ///     [0, fy, imageheigth/2],
        ///     [0, 0, 1]
        /// ]
        /// </code>
        /// 其中，fx 和 fy 分别是根据输入参数计算得到的相机在 x 轴和 y 轴方向上的焦距，
        /// imagewidth/2 和 imageheigth/2 分别是基于图像宽度和高度计算得到的主点在 x 和 y 方向上的坐标。
        /// </returns>
        public static double[,] Camera_Intrinsic_Matrix(int focal_length, double pixel_size_width, double pixel_size_heigth, int imagewidth, int imageheigth)
        {
            double fx = focal_length / pixel_size_width;
            double fy = focal_length / pixel_size_heigth;
            double[,] cameraMatrix = new double[3, 3]
                {
                 { fx, 0, imagewidth/2 },
                 { 0, fy, imageheigth/2 },
                 { 0, 0, 1 }
                };
            return cameraMatrix;
        }

        private static Point3f[][] ConvertToList(List<List<Point3f>> objectPoints)
        {
            Point3f[][] array = new Point3f[objectPoints.Count][];
            for (int i = 0; i < objectPoints.Count; i++)
            {
                array[i] = objectPoints[i].ToArray();
            }

            return array;
        }
        private static Point2f[][] ConvertToList(List<List<Point2f>> objectPoints)
        {
            Point2f[][] array = new Point2f[objectPoints.Count][];
            for (int i = 0; i < objectPoints.Count; i++)
            {
                array[i] = objectPoints[i].ToArray();
            }
            return array;
        }

        private static Mat ConvertToMat(double[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            Mat mat = new Mat(rows, cols, MatType.CV_64F);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    mat.Set(i, j, array[i, j]);
                }
            }
            return mat;
        }

        static Mat ConvertToMat(double[] array)
        {
            int rows = 1;
            int cols = array.Length;
            Mat mat = new Mat(rows, cols, MatType.CV_64F);

            for (int i = 0; i < cols; i++)
            {
                mat.Set(0, i, array[i]);
            }

            return mat;
        }
    }
}
