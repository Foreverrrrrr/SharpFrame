using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Common
{
    internal class SQL_Sqlite
    {
        private static SQLiteConnection _sqliteConn = null;
        public static string _SQLpath = @"D:\项目文件\Csharp\SQLite\SQLite\Data.db";
        public static string into = "insert into person (Time,Value) values(@Time,@Value)";
        public static string ConnectionString = ";Pooling=true;FailIfMissing=false";

        public static SQLiteConnection SQLiteConn
        {
            get
            {
                if (_sqliteConn == null && _SQLpath != null) _sqliteConn = new SQLiteConnection(_SQLpath);
                return _sqliteConn;
            }
            set { _sqliteConn = value; }
        }

        /// <summary>
        /// 建立数据库
        /// </summary>
        /// <param name="path">数据库路径</param>
        public static void NewSql(string sqlitepath)
        {
            SQLiteConnection.CreateFile(sqlitepath);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="varchar">列名称</param>
        public static void NewTable(string sqlitepath, string tableName, string[] varchar)
        {
            SQLiteConnection sqliteConn = new SQLiteConnection("Data Source=" + sqlitepath);
            if (sqliteConn.State != System.Data.ConnectionState.Open)
            {
                sqliteConn.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = sqliteConn,
                    CommandText = "CREATE TABLE " + tableName + ($"({varchar[0]} varchar,{varchar[1]} varchar,{varchar[2]} varchar,{varchar[3]} varchar)")
                };
                cmd.ExecuteNonQuery();
            }
            sqliteConn.Close();
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="varchar">列名称</param>
        public static void NewTable(string sqlitepath, string tableName, string commandText)
        {
            SQLiteConnection sqliteConn = new SQLiteConnection("Data Source=" + sqlitepath);
            if (sqliteConn.State != System.Data.ConnectionState.Open)
            {
                sqliteConn.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = sqliteConn,
                    CommandText = commandText
                };
                cmd.ExecuteNonQuery();
            }
            sqliteConn.Close();
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <returns></returns>
        public static bool OpenDb(string sqlitepath)
        {
            try
            {

                SQLiteConn = new SQLiteConnection(sqlitepath);
                SQLiteConn.Open();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库：" + _SQLpath + "的连接失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public static void CloseSql()
        {
            if (SQLiteConn != null && _sqliteConn.State != ConnectionState.Closed)
            {
                SQLiteConn.Close();
                SQLiteConn = null;
            }
        }

        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string sqlStr, params SQLiteParameter[] p)
        {

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = sqlStr;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            if (p != null)
            {
                foreach (SQLiteParameter parm in p)
                {
                    cmd.Parameters.AddWithValue(parm.ParameterName, parm.Value);
                }
            }

        }
        /// <summary>
        /// 查询dataSet
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string sqlitepath, string sqlStr, params SQLiteParameter[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + sqlitepath))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {

                    DataSet ds = new DataSet();
                    try
                    {
                        PrepareCommand(command, conn, sqlStr, p);
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                        return ds;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }


        /// <summary>
        /// 事务处理
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="p"></param>
        public static bool ExecSQL(string sqlitepath, string sqlStr, params SQLiteParameter[] p)
        {
            bool result = true;
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + sqlitepath))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, sqlStr, p);
                    SQLiteTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        result = true;
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = false;
                        ex.Message.ToString();
                    }
                }
            }
            return result;
        }

        public static int ExecuteNonQuery(string sqlitepath, string sqlStr, params SQLiteParameter[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + sqlitepath))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(command, conn, sqlStr, p);

                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return -99;
                    }
                }
            }
        }

        public static object ExecuteScalar(string sqlitepath, string sqlStr, params SQLiteParameter[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + sqlitepath))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(command, conn, sqlStr, p);
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        return -99;
                    }
                }
            }
        }
    }

}

