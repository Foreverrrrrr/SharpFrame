using OpenCvSharp.Internal.Vectors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Common
{
    /// <summary>
    /// 生产信息类
    /// </summary>
    public class ProductionInformation
    {
        private static Stopwatch stopwatch;
        private static TimeSpan totalElapsedTime = TimeSpan.Zero;

        /// <summary>
        /// 开始生产计时
        /// </summary>
        public static void StartProduction()
        {
            StartTiming();
            Console.WriteLine("生产已开始。");
        }

        /// <summary>
        /// 结束生产计时
        /// </summary>
        /// <returns>单次ct 累计ct</returns>
        public static (TimeSpan, TimeSpan) EndProduction()
        {
            var elapsedTime = EndTiming();
            totalElapsedTime += elapsedTime;
            Console.WriteLine($"生产已结束。本次生产耗时: {elapsedTime.TotalMilliseconds} 毫秒。");
            Console.WriteLine($"累计生产耗时: {totalElapsedTime.TotalMilliseconds} 毫秒。");
            return (elapsedTime, totalElapsedTime);
        }

        /// <summary>
        /// 获得累计生产时间
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetTotalProductionTime()
        {
            return totalElapsedTime;
        }

        private static void StartTiming()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        private static TimeSpan EndTiming()
        {
            if (stopwatch != null && stopwatch.IsRunning)
            {
                stopwatch.Stop();
                return stopwatch.Elapsed;
            }
            return TimeSpan.Zero;
        }

        /// <summary>
        /// 读取生产信息
        /// </summary>
        public static void ReadProductionInfo(ref InfoStructure info)
        {
            string tableName = "ProductionInformation";
            string databasePath = tableName + ".db";
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string selectQuery = $"SELECT * FROM {tableName} WHERE date(SetTime) = @CurrentDate";
            SQLiteParameter parameter = new SQLiteParameter("@CurrentDate", currentDate);
            DataSet resultDataSet = SQL_Sqlite.ExecuteQuery(databasePath, selectQuery, parameter);
            if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = resultDataSet.Tables[0].Rows[0];
                // 填充 InfoStructure 对象
                info.SetTime = DateTime.Parse(row["SetTime"].ToString());
                info.ProductionTotal = Convert.ToInt32(row["ProductionTotal"]);
                info.QualifiedCount = Convert.ToInt32(row["QualifiedCount"]);
                info.NGCount = Convert.ToInt32(row["NGCount"]);
                info.YieldRate = Convert.ToDouble(row["YieldRate"]);
                info.ProductionTime = DateTime.Parse(row["ProductionTime"].ToString());
                info.ActualProductionTime = DateTime.Parse(row["ActualProductionTime"].ToString());
            }
            else
            {
                info = new InfoStructure();
            }
        }

        /// <summary>
        /// 保存生产信息
        /// </summary>
        public static void SaveProductionInfo(InfoStructure info)
        {
            string tableName = "ProductionInformation";
            string databasePath = tableName + ".db";
            string datePart = info.SetTime.ToString("yyyy-MM-dd");
            string selectQuery = $"SELECT COUNT(*) FROM {tableName} WHERE date(SetTime) = @DatePart";
            SQLiteParameter selectParam = new SQLiteParameter("@DatePart", datePart);
            object result = SQL_Sqlite.ExecuteScalar(databasePath, selectQuery, selectParam);
            int count = Convert.ToInt32(result);
            if (count > 0)
            {
                string deleteQuery = $"DELETE FROM {tableName} WHERE date(SetTime) = @DatePart";
                SQLiteParameter deleteParam = new SQLiteParameter("@DatePart", datePart);
                SQL_Sqlite.ExecuteNonQuery(databasePath, deleteQuery, deleteParam);
            }
            string insertQuery = $@"
            INSERT INTO {tableName} (SetTime, ProductionTotal, QualifiedCount, NGCount, YieldRate, ProductionTime, ActualProductionTime)
            VALUES (@SetTime, @ProductionTotal, @QualifiedCount, @NGCount, @YieldRate, @ProductionTime, @ActualProductionTime);";
            SQLiteParameter[] parameters = new SQLiteParameter[]
            {
            new SQLiteParameter("@SetTime", info.SetTime.ToString("yyyy-MM-dd HH:mm:ss")),
            new SQLiteParameter("@ProductionTotal", info.ProductionTotal),
            new SQLiteParameter("@QualifiedCount", info.QualifiedCount),
            new SQLiteParameter("@NGCount", info.NGCount),
            new SQLiteParameter("@YieldRate", info.YieldRate),
            new SQLiteParameter("@ProductionTime", info.ProductionTime.ToString("yyyy-MM-dd HH:mm:ss")),
            new SQLiteParameter("@ActualProductionTime", info.ActualProductionTime.ToString("yyyy-MM-dd HH:mm:ss"))
            };
            SQL_Sqlite.ExecuteNonQuery(databasePath, insertQuery, parameters);
        }

        /// <summary>
        /// 生成生产数据数据库
        /// </summary>
        public static void SetDataDB()
        {
            string tableName = "ProductionInformation";
            SQL_Sqlite.NewSql(tableName + ".db");
            string createTableQuery = $@"
                    CREATE TABLE IF NOT EXISTS {tableName} (
                        SetTime TEXT,
                        ProductionTotal INTEGER,
                        QualifiedCount INTEGER,
                        NGCount INTEGER,
                        YieldRate REAL,
                        ProductionTime TEXT,
                        ActualProductionTime TEXT
                    );";
            SQL_Sqlite.NewTable(tableName + ".db", tableName, createTableQuery);
        }
    }

    public class InfoStructure
    {
        /// <summary>
        /// 设置时间
        /// </summary>
        public DateTime SetTime { get; set; }

        /// <summary>
        /// 生产总数
        /// </summary>
        public int ProductionTotal { get; set; }

        /// <summary>
        /// 合格数
        /// </summary>
        public int QualifiedCount { get; set; }

        /// <summary>
        /// NG 数（不合格数）
        /// </summary>
        public int NGCount { get; set; }

        /// <summary>
        /// 良率
        /// </summary>
        public double YieldRate { get; set; }

        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime ProductionTime { get; set; }

        /// <summary>
        /// 实际生产时间
        /// </summary>
        public DateTime ActualProductionTime { get; set; }
    }
}
