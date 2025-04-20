using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;
using static SharpFrame.Views.SharpStyle.HandelEventArgs;

namespace SharpFrame.Views.SharpStyle
{
    public class OpenVision : Control, IDisposable
    {
        private Canvas ImageCanvas;
        private Rectangle SelectionRectangle;
        // private BitmapSource originalImage;
        private Point startPoint;
        private bool isSelecting;
        private TransformGroup transformGroup = new TransformGroup();
        private ScaleTransform scaleTransform = new ScaleTransform();
        private TranslateTransform translateTransform = new TranslateTransform();
        private const double ZoomFactor = 1.1;
        private bool isDragging;
        private Point lastMousePosition;
        private const double MaxScale = 10;

        static OpenVision()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OpenVision), new FrameworkPropertyMetadata(typeof(OpenVision)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ImageCanvas = GetTemplateChild("ImageCanvas") as Canvas;
            SelectionRectangle = GetTemplateChild("SelectionRectangle") as Rectangle;
            Image originalImage = GetTemplateChild("OriginalImageControl") as Image;
            originalImage.RenderTransform = transformGroup;
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            if (ImageCanvas != null)
            {
                ImageCanvas.MouseLeftButtonDown += MouseLDown;
                ImageCanvas.MouseLeftButtonUp += MouseLUp;
                ImageCanvas.MouseMove += Mouse_Move;
                ImageCanvas.MouseWheel += Mouse_Wheel;
                ImageCanvas.MouseRightButtonDown += MouseRDown;
                ImageCanvas.MouseRightButtonUp += MouseRUp;
            }
            this.Loaded += (o, x) =>
            {
                var image = GetTemplateChild("OriginalImageControl") as Image;
                PresentationSource presentationSource = PresentationSource.FromVisual(image);
                HwndSource hwndSource = presentationSource as HwndSource;
                if (hwndSource != null)
                {
                    SetValue(HandlePropertyKey, hwndSource?.Handle ?? IntPtr.Zero);
                    RaiseIntercepHandelValueEvent(Handle);
                }
            };
        }

        private void MouseRUp(object sender, MouseButtonEventArgs e)
        {
            //isDragging = false;
            //ImageCanvas.ReleaseMouseCapture();
        }

        /// <summary>
        /// 右键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseRDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageCanvas == null || SelectionRectangle == null || InputImage == null)
                return;
            if (isSelecting)
                return;
            double controlWidth = ImageCanvas.ActualWidth;
            double controlHeight = ImageCanvas.ActualHeight;
            double imageWidth = ((BitmapSource)InputImage).PixelWidth;
            double imageHeight = ((BitmapSource)InputImage).PixelHeight;
            double scaleX = controlWidth / imageWidth;
            double scaleY = controlHeight / imageHeight;
            double scale = Math.Min(scaleX, scaleY);
            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
            double scaledImageWidth = imageWidth * scale;
            double scaledImageHeight = imageHeight * scale;
            double offsetX = (controlWidth - scaledImageWidth) / 2;
            double offsetY = (controlHeight - scaledImageHeight) / 2;
            translateTransform.X = offsetX;
            translateTransform.Y = offsetY;
            scaleTransform.CenterX = 0;
            scaleTransform.CenterY = 0;
            if (SelectionRectangle.Visibility == Visibility.Visible)
            {
                SelectionRectangle.Visibility = Visibility.Collapsed;
            }
            //lastMousePosition = e.GetPosition(ImageCanvas); // 记录平移开始点
            //isDragging = true;
            //ImageCanvas.CaptureMouse();
        }

        /// <summary>
        /// 滚轮移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mouse_Wheel(object sender, MouseWheelEventArgs e)
        {
            if (ImageCanvas == null || SelectionRectangle == null || InputImage == null)
                return;
            if (isSelecting)
                return;
            Point mousePosition = e.GetPosition(ImageCanvas);
            double zoom = e.Delta > 0 ? ZoomFactor : 1 / ZoomFactor;
            double newScaleX = scaleTransform.ScaleX * zoom;
            double newScaleY = scaleTransform.ScaleY * zoom;
            if (newScaleX > MaxScale || newScaleY > MaxScale)
            {
                return;
            }
            Point originalPoint = transformGroup.Inverse.Transform(mousePosition);
            scaleTransform.CenterX = originalPoint.X;
            scaleTransform.CenterY = originalPoint.Y;
            scaleTransform.ScaleX = newScaleX;
            scaleTransform.ScaleY = newScaleY;
            Point newPosition = transformGroup.Transform(originalPoint);
            translateTransform.X += mousePosition.X - newPosition.X;
            translateTransform.Y += mousePosition.Y - newPosition.Y;
            if (SelectionRectangle.Visibility == Visibility.Visible)
            {
                Point rectStart = new Point(Canvas.GetLeft(SelectionRectangle), Canvas.GetTop(SelectionRectangle));
                Point rectEnd = rectStart + new Vector(SelectionRectangle.Width, SelectionRectangle.Height);
                rectStart = transformGroup.Inverse.Transform(rectStart);
                rectEnd = transformGroup.Inverse.Transform(rectEnd);
                rectStart = transformGroup.Transform(rectStart);
                rectEnd = transformGroup.Transform(rectEnd);
                Canvas.SetLeft(SelectionRectangle, rectStart.X);
                Canvas.SetTop(SelectionRectangle, rectStart.Y);
                SelectionRectangle.Width = rectEnd.X - rectStart.X;
                SelectionRectangle.Height = rectEnd.Y - rectStart.Y;
            }
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (ImageCanvas == null || SelectionRectangle == null || InputImage == null)
                return;
            if (isSelecting)
            {
                Point currentPoint = e.GetPosition(ImageCanvas);
                double x = Math.Min(startPoint.X, currentPoint.X);
                double y = Math.Min(startPoint.Y, currentPoint.Y);
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);
                Canvas.SetLeft(SelectionRectangle, x);
                Canvas.SetTop(SelectionRectangle, y);
                SelectionRectangle.Width = width;
                SelectionRectangle.Height = height;
            }
            else if (isDragging)
            {
                Point currentMousePosition = e.GetPosition(ImageCanvas);
                Vector offset = currentMousePosition - lastMousePosition;
                translateTransform.X += offset.X;
                translateTransform.Y += offset.Y;
                lastMousePosition = currentMousePosition;
                if (SelectionRectangle.Visibility == Visibility.Visible)
                {
                    Canvas.SetLeft(SelectionRectangle, Canvas.GetLeft(SelectionRectangle) + offset.X);
                    Canvas.SetTop(SelectionRectangle, Canvas.GetTop(SelectionRectangle) + offset.Y);
                }
            }
            Point mousePosition = e.GetPosition(ImageCanvas);

            // 计算图像的缩放因子
            double scaleX = (double)((BitmapSource)InputImage).PixelWidth / ImageCanvas.ActualWidth;
            double scaleY = (double)((BitmapSource)InputImage).PixelHeight / ImageCanvas.ActualHeight;

            // 根据缩放因子转换鼠标坐标
            int pixelx = (int)(mousePosition.X * scaleX);
            int pixely = (int)(mousePosition.Y * scaleY);
            if (pixelx >= 0 && pixelx < ((BitmapSource)InputImage).PixelWidth && pixely >= 0 && pixely < ((BitmapSource)InputImage).PixelHeight)
            {
                byte[] pixelData = new byte[4];
                ((BitmapSource)InputImage).CopyPixels(new Int32Rect(pixelx, pixely, 1, 1), pixelData, 4, 0);
                byte blue = pixelData[0];
                byte green = pixelData[1];
                byte red = pixelData[2];
                byte alpha = pixelData[3];

                // 计算灰度值
                double grayValue = 0.299 * red + 0.587 * green + 0.114 * blue;
                PixelInfo pixel = new PixelInfo()
                {
                    X = pixelx,
                    Y = pixely,
                    R = red,
                    G = green,
                    B = blue,
                    Gray = (int)grayValue
                };
                // 处理像素信息

                RaiseMovePixelInfoEvent(pixel);
                //Console.WriteLine($"Pixel at ({pixelx}, {pixely}): R={red}, G={green}, Gray={grayValue}");
            }
        }


        /// <summary>
        /// 左键松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLUp(object sender, MouseButtonEventArgs e)
        {
            if (ImageCanvas == null || SelectionRectangle == null || InputImage == null)
                return;
            if (isSelecting)
            {
                isSelecting = false;
                ImageCanvas.ReleaseMouseCapture();
                if (SelectionRectangle.Width > 0 && SelectionRectangle.Height > 0)
                {
                    Point rectStart = new Point(Canvas.GetLeft(SelectionRectangle), Canvas.GetTop(SelectionRectangle));
                    Point rectEnd = rectStart + new Vector(SelectionRectangle.Width, SelectionRectangle.Height);
                    rectStart = transformGroup.Inverse.Transform(rectStart);
                    rectEnd = transformGroup.Inverse.Transform(rectEnd);
                    int cropX = (int)Math.Round(rectStart.X);
                    int cropY = (int)Math.Round(rectStart.Y);
                    int cropWidth = (int)Math.Round(rectEnd.X - rectStart.X);
                    int cropHeight = (int)Math.Round(rectEnd.Y - rectStart.Y);
                    cropX = Math.Max(cropX, 0);
                    cropY = Math.Max(cropY, 0);
                    cropWidth = Math.Min(cropWidth, ((BitmapSource)InputImage).PixelWidth - cropX);
                    cropHeight = Math.Min(cropHeight, ((BitmapSource)InputImage).PixelHeight - cropY);
                    if (cropWidth > 0 && cropHeight > 0)
                    {
                        Int32Rect cropRect = new Int32Rect(cropX, cropY, cropWidth, cropHeight);
                        CroppedBitmap croppedBitmap = new CroppedBitmap(((BitmapSource)InputImage), cropRect);
                        ROI roi = new ROI()
                        {
                            X = cropX,
                            Y = cropY,
                            Width = cropWidth,
                            Height = cropHeight,
                            RoiBitmap = croppedBitmap
                        };
                        RaiseInterceptROIEvent(roi);
                        SelectionRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// 左键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLDown(object sender, MouseButtonEventArgs e)
        {

            if (ImageCanvas == null || SelectionRectangle == null || InputImage == null)
                return;
            startPoint = e.GetPosition(ImageCanvas);
            isSelecting = true;
            SelectionRectangle.Visibility = Visibility.Visible;
            Canvas.SetLeft(SelectionRectangle, startPoint.X);
            Canvas.SetTop(SelectionRectangle, startPoint.Y);
            SelectionRectangle.Width = 0;
            SelectionRectangle.Height = 0;
            ImageCanvas.CaptureMouse();
        }

        ///// <summary>
        ///// 注册鼠标选取路由事件
        ///// </summary>
        public static readonly RoutedEvent InterceptROIEvent = EventManager.RegisterRoutedEvent(
            "InterceptROI", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ROI>), typeof(OpenVision));

        // Roi区域Command
        public event RoutedEventHandler<ROI> InterceptROI
        {
            add { AddHandler(InterceptROIEvent, value); }
            remove { RemoveHandler(InterceptROIEvent, value); }
        }

        /// <summary>
        /// 鼠标截取区域路由触发
        /// </summary>
        /// <param name="roi">X,Y,W,H,image</param>
        protected void RaiseInterceptROIEvent(ROI roi)
        {
            RoutedEventArgs<ROI> args = new RoutedEventArgs<ROI>(InterceptROIEvent, roi);
            RaiseEvent(args);
        }

        // 注册鼠标移动路由事件
        public static readonly RoutedEvent MovePixelInfoEvent = EventManager.RegisterRoutedEvent(
            "MovePixelInfo", RoutingStrategy.Bubble, typeof(PixelInfoRoutedEventHandler), typeof(OpenVision));

        // 鼠标位移像素Command
        public event PixelInfoRoutedEventHandler MovePixelInfo
        {
            add { AddHandler(MovePixelInfoEvent, value); }
            remove { RemoveHandler(MovePixelInfoEvent, value); }
        }


        protected void RaiseIntercepHandelValueEvent(IntPtr handel)
        {
            HandelEventArgs args = new HandelEventArgs(HandelEvent, handel);
            RaiseEvent(args);
        }

        public static readonly RoutedEvent HandelEvent = EventManager.RegisterRoutedEvent(
            "HandelValue", RoutingStrategy.Bubble, typeof(HandelValueEventHandler), typeof(OpenVision));

        // 控件HandelCommand
        public event HandelValueEventHandler HandelValue
        {
            add { AddHandler(HandelEvent, value); }
            remove { RemoveHandler(HandelEvent, value); }
        }

        /// <summary>
        /// 鼠标像素信息路由触发
        /// </summary>
        /// <param name="roi">X,Y,R,G,B</param>
        protected void RaiseMovePixelInfoEvent(PixelInfo pixelInfo)
        {
            PixelInfoRoutedEventArgs args = new PixelInfoRoutedEventArgs(MovePixelInfoEvent, pixelInfo);
            RaiseEvent(args);
        }

        public static readonly DependencyProperty OpenVisionHeight = DependencyProperty.Register(
            "VisionHeight",
            typeof(double),
            typeof(OpenVision),
            new PropertyMetadata(100.0));

        /// <summary>
        /// 控件高度
        /// </summary>
        public double VisionHeight
        {
            get { return (double)GetValue(OpenVisionHeight); }
            set { SetValue(OpenVisionHeight, value); }
        }

        public static readonly DependencyProperty OpenVisionWidth = DependencyProperty.Register(
            "VisionWidth",
            typeof(double),
            typeof(OpenVision),
            new PropertyMetadata(100.0));

        /// <summary>
        /// 控件宽度
        /// </summary>
        public double VisionWidth
        {
            get { return (double)GetValue(OpenVisionWidth); }
            set { SetValue(OpenVisionWidth, value); }
        }

        public static readonly DependencyProperty OpenVisionImage = DependencyProperty.Register(
           "InputImage",
           typeof(ImageSource),
           typeof(OpenVision),
           new PropertyMetadata(null, OnImageChanged));

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var openVision = (OpenVision)d;
            ImageSource newImageSource = e.NewValue as ImageSource;
            if (newImageSource == null)
            {
                return;
            }

            if (e.OldValue is ImageSource oldImageSource)
            {
                if (oldImageSource is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                oldImageSource = null; // 断开引用
            }
            openVision.MouseRDown(null, null);
        }

        /// <summary>
        /// 图像源
        /// </summary>
        public ImageSource InputImage
        {
            get { return (ImageSource)GetValue(OpenVisionImage); }
            set { SetValue(OpenVisionImage, value); ; }
        }

        // 注册Handle
        private static readonly DependencyPropertyKey HandlePropertyKey =
        DependencyProperty.RegisterReadOnly(
            "Handle",
            typeof(IntPtr),
            typeof(OpenVision),
            new PropertyMetadata(IntPtr.Zero));

        public IntPtr Handle =>
            (IntPtr)GetValue(HandlePropertyKey.DependencyProperty);

        public void Dispose()
        {
            if (ImageCanvas != null)
            {
                ImageCanvas.MouseLeftButtonDown -= MouseLDown;
                ImageCanvas.MouseLeftButtonUp -= MouseLUp;
                ImageCanvas.MouseMove -= Mouse_Move;
                ImageCanvas.MouseWheel -= Mouse_Wheel;
                ImageCanvas.MouseRightButtonDown -= MouseRDown;
                ImageCanvas.MouseRightButtonUp -= MouseRUp;
                ImageCanvas = null;
            }
            if (SelectionRectangle != null)
            {
                SelectionRectangle = null;
            }
            transformGroup.Children.Clear();
            transformGroup = null;
            scaleTransform = null;
            translateTransform = null;
            InputImage = null;
            GC.SuppressFinalize(this);
        }
    }

    public class HandelEventArgs : RoutedEventArgs
    {
        public IntPtr ImageHandel { get; set; }
        public HandelEventArgs(RoutedEvent routedEvent, IntPtr handel) : base(routedEvent)
        {
            ImageHandel = handel;
        }
        public delegate void HandelValueEventHandler(object sender, HandelEventArgs e);
    }

    #region 鼠标跟随像素信息路由事件

    public class PixelInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }
        public int Gray { get; set; }
    }
    // 自定义的 RoutedEventArgs 类，用于传递 PixelInfo 对象
    public class PixelInfoRoutedEventArgs : RoutedEventArgs
    {
        public PixelInfo PixelInfo { get; private set; }

        public PixelInfoRoutedEventArgs(RoutedEvent routedEvent, PixelInfo pixelInfo) : base(routedEvent)
        {
            PixelInfo = pixelInfo;
        }
    }
    // 自定义的 RoutedEventHandler 委托
    public delegate void PixelInfoRoutedEventHandler(object sender, PixelInfoRoutedEventArgs e);
    #endregion

    #region 鼠标截取功能路由事件

    public class ROI : IDisposable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public CroppedBitmap RoiBitmap { get; set; }

        public void Dispose()
        {
            RoiBitmap = null;
            GC.SuppressFinalize(this);
        }
    }

    public class RoutedEventArgs<ROI> : RoutedEventArgs
    {
        public ROI MousetROI { get; private set; }

        public RoutedEventArgs(RoutedEvent routedEvent, ROI roi) : base(routedEvent)
        {
            MousetROI = roi;
        }
    }
    // 自定义的 RoutedEventHandler 委托
    public delegate void RoutedEventHandler<ROI>(object sender, RoutedEventArgs<ROI> e);

    #endregion
}
