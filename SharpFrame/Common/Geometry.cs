using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpFrame.Common
{
    /// <summary>
    /// 几何算法
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// 倾斜角基准
        /// </summary>
        public enum AngleCriterion
        {
            /// <summary>
            /// 水平基准
            /// </summary>
            HorizontalLine,
            /// <summary>
            /// 垂直基准
            /// </summary>
            PerpendiCularLine
        }

        /// <summary>
        /// 平面直线坐标
        /// </summary>
        public struct Line2D
        {
            /// <summary>
            /// 平面直线坐标
            /// </summary>
            /// <param name="x1">线段起始X坐标</param>
            /// <param name="y1">线段起始Y坐标</param>
            /// <param name="x2">线段结束X坐标</param>
            /// <param name="y2">线段结束Y坐标</param>
            public Line2D(double x1, double y1, double x2, double y2)
            {
                _startx = 0;
                _starty = 0;
                _endx = 0;
                _endy = 0;
                StartX = x1;
                StartY = y1;
                EndX = x2;
                EndY = y2;
            }

            private double _startx;
            /// <summary>
            /// 直线起始X坐标
            /// </summary>
            public double StartX
            {
                get { return _startx; }
                set { _startx = value; }
            }

            private double _starty;
            /// <summary>
            /// 直线起始Y坐标
            /// </summary>
            public double StartY
            {
                get { return _starty; }
                set { _starty = value; }
            }

            private double _endx;
            /// <summary>
            /// 直线结束X坐标
            /// </summary>
            public double EndX
            {
                get { return _endx; }
                set { _endx = value; }
            }

            private double _endy;
            /// <summary>
            /// 直线结束Y坐标
            /// </summary>
            public double EndY
            {
                get { return _endy; }
                set { _endy = value; }
            }
        }

        /// <summary>
        /// 平面坐标
        /// </summary>
        public struct Point2D
        {
            /// <summary>
            /// 平面点位坐标
            /// </summary>
            /// <param name="x">X坐标值</param>
            /// <param name="y">Y坐标值</param>
            public Point2D(double x, double y)
            {
                _x = 0;
                _y = 0;
                this.X = x;
                this.Y = y;
            }

            private double _x;
            /// <summary>
            /// X坐标
            /// </summary>
            public double X
            {
                get { return _x; }
                set { _x = value; }
            }

            private double _y;
            /// <summary>
            /// Y坐标
            /// </summary>
            public double Y
            {
                get { return _y; }
                set { _y = value; }
            }
        }

        /// <summary>
        /// 空间坐标
        /// </summary>
        public struct Point3D
        {
            /// <summary>
            /// 空间坐标
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            public Point3D(double x, double y, double z)
            {
                _x = 0;
                _y = 0;
                _z = 0;
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            private double _x;
            /// <summary>
            /// 空间X坐标
            /// </summary>
            public double X
            {
                get { return _x; }
                set { _x = value; }
            }

            private double _y;
            /// <summary>
            /// 空间Y坐标
            /// </summary>
            public double Y
            {
                get { return _y; }
                set { _y = value; }
            }

            private double _z;
            /// <summary>
            /// 空间Z坐标
            /// </summary>
            public double Z
            {
                get { return _z; }
                set { _z = value; }
            }
        }

        /// <summary>
        /// PID参数结构体
        /// </summary>
        public struct PIDHelper
        {
            /// <summary>
            /// PID参数初始化
            /// </summary>
            /// <param name="Kp">P比例值</param>
            /// <param name="Ki">I积分值</param>
            /// <param name="Kd">D微分值</param>
            /// <param name="DeadBand">死区宽度</param>
            /// <param name="MaxLimit">最大值限制</param>
            /// <param name="MinLimit">最小值限制</param>
            /// <param name="setvalue">目标值</param>
            public PIDHelper(double Kp, double Ki, double Kd, double DeadBand, double MaxLimit, double MinLimit, double setvalue)
            {
                prakp = Kp;
                praki = Ki;
                prakd = Kd;
                deadband = DeadBand;
                MAXLIM = MaxLimit;
                MINLIM = MinLimit;
                setValue = setvalue;
                err = 0;
                err_last = 0;
                err_next = 0;
            }

            private double prakp;
            /// <summary>比例的参数信息</summary>
            public double Kp
            {
                set => this.prakp = value;
                get => this.prakp;
            }

            private double praki;
            /// <summary>积分的参数信息</summary>
            public double Ki
            {
                set => this.praki = value;
                get => this.praki;
            }

            private double prakd;
            /// <summary>微分的参数信息</summary>
            public double Kd
            {
                set => this.prakd = value;
                get => this.prakd;
            }

            private double deadband;
            /// <summary>获取或设置死区的值</summary>
            public double DeadBand
            {
                set => this.deadband = value;
                get => this.deadband;
            }

            private double MAXLIM;
            /// <summary>获取或设置输出的上限，默认为没有设置</summary>
            public double MaxLimit
            {
                set => this.MAXLIM = value;
                get => this.MAXLIM;
            }

            private double MINLIM;
            /// <summary>获取或设置输出的下限，默认为没有设置</summary>
            public double MinLimit
            {
                set => this.MINLIM = value;
                get => this.MINLIM;
            }

            private double setValue;
            /// <summary>获取或设置目标值</summary>
            public double SetValue
            {
                set => this.setValue = value;
                get => this.setValue;
            }

            private double err;
            /// <summary>偏差值</summary>
            public double Err
            {
                get => this.err;
                set => this.err = value;
            }

            private double err_last;
            /// <summary>上一个偏差值</summary>
            public double Err_Last
            {
                get => this.err_last;
                set => this.err_last = value;
            }

            private double err_next;
            /// <summary>下一个偏差</summary>
            public double Err_Next
            {
                get { return err_next; }
                set { err_next = value; }
            }

        }

        public struct Kalman
        {
            public Kalman(double R, double Q, double A = 1, double B = 0, double C = 1, double x = 0)
            {
                _a = A;
                _b = B;
                _c = C;
                _r = R;
                _q = Q;
                _x = x;
                _cov = double.NaN;
            }

            private double _a;
            /// <summary>
            /// 状态转移方程
            /// </summary>
            public double A
            {
                get { return _a; }
                set { _a = value; }
            }

            private double _b;
            /// <summary>
            /// 控制输入参数
            /// </summary>
            public double B
            {
                get { return _b; }
                set { _b = value; }
            }

            private double _c;
            /// <summary>
            /// 观测方程
            /// </summary>
            public double C
            {
                get { return _c; }
                set { _c = value; }
            }

            private double _r;
            /// <summary>
            /// 过程噪声
            /// </summary>
            public double R
            {
                get { return _r; }
                set { _r = value; }
            }

            private double _q;
            /// <summary>
            /// 测量噪声
            /// </summary>
            public double Q
            {
                get { return _q; }
                set { _q = value; }
            }

            private double _cov;
            /// <summary>
            /// 
            /// </summary>
            public double Cov
            {
                get { return _cov; }
                set { _cov = value; }
            }

            private double _x;
            /// <summary>
            /// 初始值
            /// </summary>
            public double X
            {
                get { return _x; }
                set { _x = value; }
            }

        }

        /// <summary>
        /// 寻峰类型
        /// </summary>
        public enum Wave
        {
            /// <summary>
            /// 波峰
            /// </summary>
            Wavecrest,
            /// <summary>
            /// 波谷
            /// </summary>
            Trough
        }

        /// <summary>
        /// 位置矩阵
        /// </summary>
        public struct Matrix
        {
            /// <summary>
            /// 位置矩阵
            /// </summary>
            /// <param name="column">列</param>
            /// <param name="row">行</param>
            /// <param name="p0">点一</param>
            /// <param name="p1">点二</param>
            /// <param name="p2">点三</param>
            /// <param name="p3">点三</param>
            public Matrix(int column, int row, Point2D p0, Point2D p1, Point2D p2, Point2D p3)
            {
                _row = row;
                _column = column;
                _total = row * column;
                _p0 = p0;
                _p1 = p1;
                _p2 = p2;
                _p3 = p3;
                _poslist = new Point2D[_column, _row];
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
                this.P3 = p3;
            }

            private int _row;
            /// <summary>
            /// 行
            /// </summary>
            public int Row
            {
                get { return _row; }
                set { _row = value; _total = (_row) * (_column); }
            }

            private int _column;
            /// <summary>
            /// 列
            /// </summary>
            public int Column
            {
                get { return _column; }
                set { _column = value; _total = (_row) * (_column); }
            }

            private int _total;
            /// <summary>
            /// 总数量
            /// </summary>
            public int Total
            {
                get { return _total; }
            }

            private Point2D _p0;
            /// <summary>
            /// 行、列坐标（0，0）点
            /// </summary>
            public Point2D P0
            {
                get { return _p0; }
                set { _p0 = value; }
            }

            private Point2D _p1;
            /// <summary>
            /// 行、列坐标（Row，0）点
            /// </summary>
            public Point2D P1
            {
                get { return _p1; }
                set { _p1 = value; }
            }

            private Point2D _p2;
            /// <summary>
            /// 行、列坐标（0，Column）点
            /// </summary>
            public Point2D P2
            {
                get { return _p2; }
                set { _p2 = value; }
            }

            private Point2D _p3;
            /// <summary>
            /// 行、列坐标（0，Column）点
            /// </summary>
            public Point2D P3
            {
                get { return _p3; }
                set { _p3 = value; }
            }


            private Point2D[,] _poslist;
            /// <summary>
            /// 矩阵点位坐标数组
            /// </summary>
            public Point2D[,] PosList
            {
                get { return _poslist; }
                set { _poslist = value; }
            }
        }

        /// <summary>
        /// 上料方式
        /// </summary>
        public enum Load
        {
            /// <summary>
            /// 正常上料
            /// </summary>
            Normal,
            /// <summary>
            /// 蛇形上料
            /// </summary>
            Snake
        }

        /// <summary>
        /// 插补误差结构体
        /// </summary>
        public struct Interpolator
        {
            /// <summary>
            /// 最大误差出现索引
            /// </summary>
            public int MaxErrItem { get; set; }

            /// <summary>
            /// 最大误差值
            /// </summary>
            public double MaxErrValue { get; set; }

            /// <summary>
            /// 平均误差
            /// </summary>
            public double AverageErrValue { get; set; }

            /// <summary>
            /// 计算次数
            /// </summary>
            public int Countimes { get; set; }
        }

        /// <summary>
        /// 点线测量
        /// </summary>
        /// <param name="linex1">线起始X</param>
        /// <param name="liney1">线起始Y</param>
        /// <param name="linex2">线终点X</param>
        /// <param name="liney2">线终点Y</param>
        /// <param name="ptx">点X</param>
        /// <param name="pty">点Y</param>
        ///  <param name="conversion">像素单位</param>
        /// <returns>[0]距离 [1]垂足点X [2]垂直点Y</returns>
        public static double[] PointgoLine(double linex1, double liney1, double linex2, double liney2, double ptx, double pty, double conversion = 1)
        {
            double A = liney2 - liney1;
            double B = linex1 - linex2;
            double C = (linex2 * liney1) - (linex1 * liney2);
            double outpta = Math.Abs((A * ptx) + (B * pty) + C) / Math.Sqrt(Math.Pow(A, 2.0) + Math.Pow(B, 2.0));
            double distance = outpta * conversion;
            double Ptlinex = ((B * B * ptx) - (A * B * pty) - (A * C)) / ((A * A) + (B * B));
            double Ptliney = ((A * -1 * B * ptx) + (A * A * pty) - (B * C)) / ((A * A) + (B * B));
            return new double[] { distance, Ptlinex, Ptliney };
        }

        /// <summary>
        /// 点线测量
        /// </summary>
        /// <param name="line">直线结构</param>
        /// <param name="point1">点结构</param>
        /// <param name="conversion">像素单位</param>
        /// <returns>[0]距离 [1]垂足点X [2]垂直点Y</returns>
        public static double[] PointgoLine(Line2D line, Point2D point1, double conversion = 1)
        {
            double A = line.EndY - line.StartY;
            double B = line.StartX - line.EndX;
            double C = (line.EndX * line.StartY) - (line.StartX * line.EndY);
            double outpta = Math.Abs((A * point1.X) + (B * point1.Y) + C) / Math.Sqrt(Math.Pow(A, 2.0) + Math.Pow(B, 2.0));
            double distance = outpta * conversion;
            double Ptlinex = ((B * B * point1.X) - (A * B * point1.Y) - (A * C)) / ((A * A) + (B * B));
            double Ptliney = ((A * -1 * B * point1.X) + (A * A * point1.Y) - (B * C)) / ((A * A) + (B * B));
            return new double[] { distance, Ptlinex, Ptliney };
        }

        /// <summary>
        /// 点点测量
        /// </summary>
        /// <param name="pointx1">起始点X</param>
        /// <param name="pointy1">起始点Y</param>
        /// <param name="pointx2">结束点X</param>
        /// <param name="pointy2">结束点Y</param>
        /// <param name="conversion">像素单位</param>
        /// <returns>距离</returns>
        public static double PointMeasure(double pointx1, double pointy1, double pointx2, double pointy2, double conversion = 1)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(pointx1 - pointx2), 2.0) + Math.Pow(Math.Abs(pointy1 - pointy2), 2.0)) * conversion;
        }

        /// <summary>
        /// 倾斜角计算
        /// </summary>
        /// <param name="line">线段结构体</param>
        /// <param name="criterion">倾斜基准</param>
        /// <returns>角度</returns>
        public static double Angle(Line2D line, AngleCriterion criterion)
        {
            double k = 0;
            if (criterion == AngleCriterion.PerpendiCularLine)
            {
                k = (line.StartX - line.EndX) / (line.StartY - line.EndY);
            }
            else
            {
                k = (line.StartY - line.EndY) / (line.StartX - line.EndX);
            }
            return Math.Atan(k) * 180 / Math.PI;
        }

        /// <summary>
        /// 几何平面一般式
        /// </summary>
        /// <param name="p1">基准3D点1</param>
        /// <param name="p2">基准3D点2</param>
        /// <param name="p3">基准3D点3</param>
        /// <returns>几何平面一般方程式Ax +By +Cz + D = 0，[0]A,[1]B,[2]C,[3]D</returns>
        public static double[] PlanarGeneralForm(Point3D p1, Point3D p2, Point3D p3)
        {
            double a1 = (p2.Y - p1.Y) * (p3.Z - p1.Z);
            double a2 = (p2.Z - p1.Z) * (p3.Y - p1.Y);
            double a = a1 - a2;
            double b1 = (p2.Z - p1.Z) * (p3.X - p1.X);
            double b2 = (p2.X - p1.X) * (p3.Z - p1.Z);
            double b = b1 - b2;
            double c1 = (p2.X - p1.X) * (p3.Y - p1.Y);
            double c2 = (p2.Y - p1.Y) * (p3.X - p1.X);
            double c = c1 - c2;
            double d1 = (a * p1.X) + (b * p1.Y) + (c * p1.Z);
            double d = 0 - d1;
            return new double[] { a, b, c, d };
        }

        /// <summary>
        /// 空间点到几何平面的距离
        /// </summary>
        /// <param name="p">3D坐标</param>
        /// <param name="a">平面一般式A变量</param>
        /// <param name="b">平面一般式B变量</param>
        /// <param name="c">平面一般式C变量</param>
        /// <param name="d">平面一般式D变量</param>
        /// <returns>垂直距离</returns>
        public static double DistanceFromPointPlane(Point3D p, double a, double b, double c, double d)
        {
            double a1 = Math.Abs((a * p.X) + (b * p.Y) + (c * p.Z) + d);
            double b1 = Math.Sqrt((a * a) + (b * b) + (c * c));
            return a1 / b1;
        }

        /// <summary>
        /// 增量式PID算法
        /// </summary>
        /// <param name="helper">PID参数结构</param>
        /// <param name="prvalue">实际值</param>
        /// <returns></returns>
        public static double PIDCalculate(PIDHelper helper, double prvalue)
        {
            double value = prvalue;
            helper.Err_Next = helper.Err_Last;
            helper.Err_Last = helper.Err;
            helper.Err = helper.SetValue - value;
            value += helper.Kp * (helper.Err - helper.Err_Last + (helper.Ki * helper.Err) + (helper.Kd * (helper.Err - (2.0 * helper.Err_Last) + helper.Err_Next)));
            if (value > helper.MaxLimit)
                value = helper.MaxLimit;
            if (value < helper.MinLimit)
                value = helper.MinLimit;
            return value;
        }

        /// <summary>
        /// 一阶卡尔曼滤波算法
        /// </summary>
        /// <param name="kalman">卡尔曼参数</param>
        /// <param name="value">输入值</param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        public static double AlittleKalman(Kalman kalman, double value, double filtered)
        {
            if (double.IsNaN(kalman.X))
            {
                kalman.X = 1 / kalman.C * value;
                kalman.Cov = 1 / kalman.C * kalman.Q * (1 / kalman.C);
            }
            else
            {
                double predX = (kalman.A * kalman.X) + (kalman.B * filtered);
                double predCov = (kalman.A * kalman.Cov * kalman.A) + kalman.R;

                // Kalman gain
                double K = predCov * kalman.C * (1 / ((kalman.C * predCov * kalman.C) + kalman.Q));

                // Correction
                kalman.X = predX + (K * (value - (kalman.C * predX)));
                kalman.Cov = predCov - (K * kalman.C * predCov);
            }
            return kalman.X;
        }

        /// <summary>
        /// 数据寻峰算法
        /// </summary>
        /// <param name="data">数据集</param>
        /// <param name="PeakStyle">寻峰类型</param>
        /// <returns>数据集下标索引</returns>
        public static int[] FindPeaks(double[] data, Wave PeakStyle)
        {
            double[] diff = new double[data.Length - 1];
            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = data[i + 1] - data[i];
            }
            int[] sign = new int[diff.Length];
            for (int i = 0; i < sign.Length; i++)//波峰波谷区分
            {
                if (diff[i] > 3)
                    sign[i] = 1;
                else if (diff[i] < -1)
                    sign[i] = -1;
                else
                    sign[i] = 0;
            }
            for (int i = sign.Length - 1; i >= 0; i--)
            {
                if (sign[i] == 0 && i == sign.Length - 1)
                    sign[i] = 1;
                else if (sign[i] == 0)
                {
                    if (sign[i + 1] >= 0)
                        sign[i] = 1;
                    else
                        sign[i] = -1;
                }
            }
            List<int> result = new List<int>();
            for (int i = 0; i != sign.Length - 1; i++)
            {
                if (PeakStyle == Wave.Wavecrest)
                {
                    if (sign[i + 1] - sign[i] == -2)
                        result.Add(i + 1);
                }
                else if (PeakStyle == Wave.Trough)
                {
                    if (sign[i + 1] - sign[i] == 2)
                        result.Add(i + 1);
                }
            }
            return result.ToArray();//相当于原数组的下标
        }

        /// <summary>
        /// 矩阵上料
        /// </summary>
        /// <param name="matrix">矩阵结构</param>
        /// <param name="model">类型</param>
        /// <param name="loadnumber">取料个数</param>
        /// <param name="serialnumber">起始取料序列</param>
        public static Point2D[] MatrixLoadMaterial(Matrix matrix, Load model, int loadnumber, int serialnumber)
        {
            Point2D[] sitepoint = new Point2D[loadnumber];
            int count = 0;
            if (model == Load.Snake)
            {
                int j = serialnumber % matrix.Row;
                for (int i = serialnumber / matrix.Column; i < matrix.Column; i++)
                {
                    for (; j < matrix.Row; j++)
                    {
                        if (i % 2 == 0)
                            sitepoint[count] = matrix.PosList[i, j];
                        else if (i % 2 != 0)
                            sitepoint[count] = matrix.PosList[i, matrix.Row - j - 1];
                        count++;
                        if (count == loadnumber)
                            break;
                    }
                    j = 0;
                    if (count == loadnumber)
                        break;
                }
            }
            return sitepoint;
        }

        /// <summary>
        /// 单线定比分
        /// </summary>
        /// <param name="startpoint">起始点坐标</param>
        /// <param name="endpoint">结束点坐标</param>
        /// <param name="quantile">等分数</param>
        /// <returns>等分坐标</returns>
        public static Point2D[] DefiniteProportionSetPoint(Point2D startpoint, Point2D endpoint, int quantile)
        {
            Point2D[] doubles = new Point2D[quantile + 1];
            for (double i = 0; i <= quantile; i++)
            {
                doubles[(int)i].X = Math.Round(((endpoint.X - startpoint.X) * i) / quantile + startpoint.X, 5);
                doubles[(int)i].Y = Math.Round(((endpoint.Y - startpoint.Y) * i) / quantile + startpoint.Y, 5);
            }
            return doubles;
        }

        /// <summary>
        /// 四点定比分
        /// </summary>
        /// <param name="point0">点一</param>
        /// <param name="point1">点二</param>
        /// <param name="point2">点三</param>
        /// <param name="point3">点四</param>
        /// <param name="column">列数</param>
        /// <param name="row">行数</param>
        /// <returns>行列内所有等分坐标</returns>
        public static Matrix DefiniteProportionSetPoint(Point2D point0, Point2D point1, Point2D point2, Point2D point3, int column, int row)
        {
            Point2D[,] doubles = new Point2D[column, row];
            Matrix matrix = new Matrix();
            var left = DefiniteProportionSetPoint(point0, point1, column - 1);
            var right = DefiniteProportionSetPoint(point2, point3, column - 1);
            if (left != null && right != null)
            {
                for (int i = 0; i < left.Length; i++)
                {
                    doubles[i, 0] = left[i];
                    doubles[i, row - 1] = right[i];
                }
                for (int i = 0; i < column; i++)
                {
                    var retpoint = DefiniteProportionSetPoint(doubles[i, 0], doubles[i, row - 1], row - 1);
                    for (int j = 0; j < row; j++)
                        doubles[i, j] = retpoint[j];
                }
                matrix = new Matrix
                {
                    Column = column,
                    Row = row,
                    P0 = point0,
                    P1 = point1,
                    P2 = point2,
                    P3 = point3,
                    PosList = doubles,
                };
            }
            return matrix;
        }

        /// <summary>
        /// 计算数组内相差值为一的众数的平均值
        /// </summary>
        /// <param name="doubles"></param>
        /// <returns></returns>
        public static double SampleMode(double[] doubles)
        {
            if (doubles.Length == 0) return 0;
            double[] data = doubles;
            Dictionary<int, int> frequencies = new Dictionary<int, int>();
            foreach (double d in data)
            {
                int rounded = (int)Math.Round(d, 3);
                if (!frequencies.ContainsKey(rounded))
                    frequencies[rounded] = 0;
                frequencies[rounded]++;
            }
            int mode = frequencies.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            List<double> result = new List<double>();
            foreach (double d in data)
            {
                if (d - mode < 1 && d - mode > 0)
                    result.Add(d);
            }
            result.Remove(result.Min());
            result.Remove(result.Max());
            return result.Average();
        }

        /// <summary>
        /// DDA直线插补算法
        /// </summary>
        /// <param name="start">直线起始点</param>
        /// <param name="end">直线结束点</param>
        /// <returns></returns>
        public static Point2D[] DDA_Interpolation(Point2D start, Point2D end)
        {
            // 计算起点和终点的坐标差
            int dx = (int)(end.X - start.X);
            int dy = (int)(end.Y - start.Y);
            // 计算插补次数
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            Point2D[] result = new Point2D[steps + 1];
            result[0] = new Point2D(start.X, start.Y);
            // 计算每一步的增量
            double xIncrement = (double)dx / steps;
            double yIncrement = (double)dy / steps;
            // 对每一步进行插值，并将结果添加到结果列表中
            double x = start.X;
            double y = start.Y;
            for (int i = 0; i <= steps; i++)
            {
                result[i] = new Point2D((int)Math.Round(x), (int)Math.Round(y));
                x += xIncrement;
                y += yIncrement;
            }
            return result;
        }

        /// <summary>
        /// 逐点直线插补
        /// </summary>
        /// <param name="starting">起始点</param>
        /// <param name="terminus">结束点</param>
        /// <returns></returns>
        public static Point2D[] PointwiseLine(Point2D starting, Point2D terminus)
        {
            double x = terminus.X - starting.X;
            double y = terminus.Y - starting.Y;
            double tan = Math.Abs(y / x);
            double moving = 0;
            Point2D[] points = new Point2D[(int)(Math.Abs(x) + Math.Abs(y)) + 1];
            Point2D point = new Point2D(starting.X, starting.Y);
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = point.X;
                points[i].Y = point.Y;
                moving = Math.Abs((terminus.Y - point.Y) / (terminus.X - point.X));
                if (moving >= tan)
                {
                    if (y >= 0)
                        point.Y++;
                    else
                        point.Y--;
                }
                else if (moving < tan)
                {
                    if (x >= 0)
                        point.X++;
                    else
                        point.X--;
                }
            }
            return points;
        }


        /// <summary>
        /// 直线插补误差计算
        /// </summary>
        /// <param name="starting">直线起始点</param>
        /// <param name="terminus">直线结束点</param>
        /// <param name="points">插补算法返回值</param>
        /// <returns></returns>
        public static Interpolator Interpolation_Error(Point2D starting, Point2D terminus, Point2D[] points)
        {
            Interpolator interpolator = new Interpolator();
            double k = (terminus.Y - starting.Y) / (terminus.X - starting.X);
            double b = terminus.Y - (k * terminus.X);
            for (int i = 0; i < points.Length; i++)
            {
                double y = k * points[i].X + b;
                double x = (points[i].Y - b) / k;
                double t = Math.Abs(points[i].Y - y);
                t = PointMeasure(points[i].X, y, points[i].X, points[i].Y);
                //var t = PointgoLine(new Line2D(starting.X, starting.Y, terminus.X, terminus.Y), points[i]);
                interpolator.AverageErrValue += t;
                if (t > interpolator.MaxErrValue)
                {
                    interpolator.MaxErrValue = t;
                    interpolator.MaxErrItem = i;
                }
            }
            interpolator.Countimes = points.Length;
            interpolator.AverageErrValue = interpolator.AverageErrValue / points.Length;
            return interpolator;
        }

        /// <summary>
        /// 求直线方程K和b变量
        /// </summary>
        /// <param name="p1">点一</param>
        /// <param name="p2">点二</param>
        /// <param name="k"></param>
        /// <param name="b"></param>
        public static void CalculateLineEquation(Point2D p1, Point2D p2, out double k, out double b)
        {
            k = (p2.Y - p1.Y) / (p2.X - p1.X);
            b = p1.Y - k * p1.X;
        }

        /// <summary>
        /// 数据平滑
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="windowSize">平滑窗口大小</param>
        /// <returns></returns>
        public static double[] MovingAverage(double[] data, int windowSize)
        {
            double[] smoothedData = new double[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                int start = Math.Max(0, i - windowSize / 2);
                int end = Math.Min(data.Length - 1, i + windowSize / 2);

                double sum = 0;
                for (int j = start; j <= end; j++)
                {
                    sum += data[j];
                }

                smoothedData[i] = sum / (end - start + 1);
            }

            return smoothedData;
        }

        /// <summary>
        /// 对一组点通过最小二乘法进行线性回归
        /// </summary>
        /// <param name="parray"></param>
        public static Point2D LinearRegression(double[] x, double[] y)
        {
            Point2D tempPoint = new Point2D();
            // 点数不能小于2
            if (x.Length < 2 || x.Length != y.Length)
            {
                Console.WriteLine("点的数量小于2或x和y数组长度不匹配，无法进行线性回归");
                return tempPoint;
            }
            // 求出横纵坐标的平均值
            double averagex = x.Sum() / x.Length;
            double averagey = y.Sum() / y.Length;
            // 经验回归系数的分子与分母
            double numerator = 0;
            double denominator = 0;
            for (int i = 0; i < x.Length; i++)
            {
                numerator += (x[i] - averagex) * (y[i] - averagey);
                denominator += (x[i] - averagex) * (x[i] - averagex);
            }
            // 回归系数b
            double RCB = numerator / denominator;
            // 回归系数a
            double RCA = averagey - RCB * averagex;

            tempPoint.X = RCB; // 斜率
            tempPoint.Y = RCA; // 截距
            return tempPoint;
        }

        /// <summary>
        /// 线性回归斜率计算
        /// </summary>
        /// <param name="array">原始数组</param>
        /// <param name="currentIndex">求导长度</param>
        /// <param name="min_to_max">true=求最大斜率 false=求最小斜率</param>
        /// <returns></returns>
        public static Point2D CalculateSlope(double[] array, int currentIndex, bool min_to_max)
        {
            Point2D point2 = new Point2D();
            double max = 0;
            if (min_to_max)
                max = double.MinValue;
            else
                max = double.MaxValue;
            int previousIndex = currentIndex - 50;
            for (int i = 0; i < array.Length - currentIndex; i++)
            {
                double x1 = i;
                double y1 = array[i];
                double x2 = i + currentIndex;
                double y2 = array[i + currentIndex];
                double slope = (y2 - y1) / (x2 - x1);
                if (slope > max)
                {
                    max = slope;
                    point2.X = i;
                }

            }
            point2.Y = array[(int)point2.X];
            return point2;
        }

        /// <summary>
        /// 平均值计算
        /// </summary>
        /// <param name="array">数组原始数据</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="endIndex">结束索引</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double CalculateAverage(double[] array, int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= array.Length || startIndex > endIndex)
            {
                // 索引无效，无法计算平均值
                throw new ArgumentException("Invalid indices");
            }

            double sum = 0;
            int count = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                sum += array[i];
                count++;
            }

            double average = sum / count;
            return average;
        }
    }
}
