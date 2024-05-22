using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace SharpFrame.Common
{
    public class SQL_Server
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString_Default = "Data Source=CHINAMI-QUD6HKC;Initial Catalog=HuaDe;Persist Security Info=True;User ID=sa;Password=ksrhck";

        private static SqlConnection sql_con;

        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());




        public static void WriteError(int id, string str, bool state)
        {
            string s = "";
            if (state)
                s = "报警中";
            else
                s = "报警复位";
            SQL_Server.ExecteNonQuery(CommandType.Text, $"insert into Error values('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',{id},'{str}','{s}')");
        }

        public static DataSet ErrorSpecialQuery(string table_name, DateTime TimeMax, DateTime TimeMin)
        {
            return SQL_Server.ExecuteDataSet(CommandType.Text, $"SELECT * FROM {table_name} WHERE Error_Time BETWEEN '{TimeMax.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{TimeMin.ToString("yyyy-MM-dd HH:mm:ss")}'");
        }

        /// <summary>
        /// 时间和结果查询
        /// </summary>
        /// <param name="TimeMax"></param>
        /// <param name="TimeMin"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static DataSet SpecialQuery(string table_name, DateTime TimeMax, DateTime TimeMin, bool res)
        {
            string sql = "";
            if (res)
                sql = "Pass";
            else
                sql = "NG";
            return SQL_Server.ExecuteDataSet(CommandType.Text, $"SELECT * FROM {table_name} WHERE test_time BETWEEN '{TimeMax.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND '{TimeMin.ToString("yyyy-MM-dd HH:mm:ss.sss")}' AND [total_result]  = '{sql}'");
        }

        /// <summary>
        /// 时间查询
        /// </summary>
        /// <param name="TimeMax"></param>
        /// <param name="TimeMin"></param>
        /// <returns></returns>
        public static List<T> SpecialQuery<T>(string table_name, DateTime TimeMax, DateTime TimeMin) where T : class, new()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = table_name ?? type.Name;
            string columns = string.Join(", ", properties.Select(p => p.Name));
            string sql = $"SELECT * FROM {table_name} WHERE Time BETWEEN '{TimeMax.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{TimeMin.ToString("yyyy-MM-dd HH:mm:ss")}' ORDER BY Time";

            DataTable dataTable = SQL_Server.ExecuteQuery(ConnectionString_Default, CommandType.Text, sql);
            return MapDataTableToObjects<T>(dataTable);
        }

        /// <summary>
        /// 数据库表建立
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename">表名</param>
        /// <param name="t">表字段类</param>
        public static void NewTablename<T>(string tablename, T t) where T : class
        {
            var type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();
            string createTableSql = $"CREATE TABLE {tablename} (";
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                Type propertyType = property.PropertyType;
                string columnType = GetColumnType(propertyType);
                createTableSql += $"{propertyName} {columnType}, ";
            }
            createTableSql = createTableSql.TrimEnd(',', ' ');
            createTableSql += ")";
            SQL_Server.ExecteNonQuery(CommandType.Text, createTableSql);
        }

        private static string GetColumnType(Type propertyType)
        {
            if (propertyType == typeof(int))
                return "INT";
            if (propertyType == typeof(string))
                return "VARCHAR(100)";
            if (propertyType == typeof(DateTime))
                return "DATETIME";
            if (propertyType == typeof(float) || propertyType == typeof(double))
                return "FLOAT";
            throw new ArgumentException($"Unsupported property type: {propertyType.Name}");
        }

        /// <summary>
        /// 数据库写入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">表类</param>
        /// <param name="table_name">表名称</param>
        public static void Write<T>(T model, string table_name = null) where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string tableName = table_name == null ? type.Name : table_name;
            string columns = string.Join(", ", properties.Select(p => p.Name));
            string values = string.Join(", ", properties.Select(p => GetValueString(p, model)));
            string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            SQL_Server.ExecteNonQuery(CommandType.Text, sql);
        }

        private static string GetValueString(PropertyInfo property, object model)
        {
            object value = property.GetValue(model);
            if (property.PropertyType == typeof(string))
                return $"'{value}'";
            else
                return value.ToString();
        }

        public static string[] GetMax()
        {
            var data = SQL_Server.ExecuteDataSet(CommandType.Text, "SELECT test_time,serial_number  FROM Test WHERE test_time = (SELECT MAX(test_time) FROM Test)");
            string serial_number = "";
            string time = "";
            string[] server_query = new string[2];
            if (data.Tables[0].Rows.Count == 1)
            {
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                    for (int j = 0; j < data.Tables[0].Columns.Count; j++)
                        server_query[j] = data.Tables[0].Rows[i][j].ToString();
                DateTime start = Convert.ToDateTime(server_query[0]).Date;
                DateTime end = Convert.ToDateTime(DateTime.Now.ToString()).Date;
                TimeSpan sp = end.Subtract(start);
                if (sp.Days > 0)
                {
                    time = DateTime.Now.ToString("yyyyMMdd");
                    serial_number = "0000000";
                }
                else
                {
                    time = end.ToString("yyyyMMdd");
                    serial_number = String.Format("{0:D7}", Convert.ToInt64(server_query[1]) + 1);
                }
            }
            else
            {
                time = DateTime.Now.ToString("yyyyMMdd");
                serial_number = "0000000";
            }
            return new string[] { time, serial_number };
        }

        private static List<T> MapDataTableToObjects<T>(DataTable dataTable) where T : class, new()
        {
            List<T> result = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();

                foreach (DataColumn column in dataTable.Columns)
                {
                    PropertyInfo property = typeof(T).GetProperty(column.ColumnName);
                    if (property != null && row[column] != DBNull.Value)
                    {
                        property.SetValue(obj, Convert.ChangeType(row[column], property.PropertyType));
                    }
                }

                result.Add(obj);
            }

            return result;
        }


        public static DataSet SpecialQuery(string qr)
        {
            //SQL_Server.ExecuteDataSet(CommandType.Text, $"SELECT * FROM Test WHERE Time BETWEEN '{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")}' AND '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' AND [Result]  = 'Pass'");       
            return SQL_Server.ExecuteDataSet(CommandType.Text, $"SELECT * FROM Test WHERE QR='{qr}' ORDER BY TestTime");
        }
        #region 基方法
        #region //ExecteNonQuery方法
        /// <summary>
        ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecteNonQuery(ConnectionString_Default, cmdType, cmdText, commandParameters);
        }

        /// <summary>
        ///存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQueryProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecteNonQuery(CommandType.StoredProcedure, cmdText, commandParameters);
        }

        /// <summary>
        ///Sql语句专用
        /// </summary>
        /// <param name="cmdText">T_Sql语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQueryText(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecteNonQuery(CommandType.Text, cmdText, commandParameters);
        }

        #endregion

        #region//GetTable方法

        /// <summary>
        /// 执行一条返回结果集的SqlCommand，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="connecttionString">一个现有的数据库连接</param>
        /// <param name="cmdTye">SqlCommand命令类型</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTable(string connecttionString, CommandType cmdTye, string cmdText, SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connecttionString))
            {
                PrepareCommand(cmd, conn, null, cmdTye, cmdText, commandParameters);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);
            }
            DataTableCollection table = ds.Tables;
            return table;
        }

        /// <summary>
        /// 执行一条返回结果集的SqlCommand，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="cmdTye">SqlCommand命令类型</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTable(CommandType cmdTye, string cmdText, SqlParameter[] commandParameters)
        {
            return GetTable(ConnectionString_Default, cmdTye, cmdText, commandParameters);
        }

        /// <summary>
        /// 存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTableProducts(string cmdText, SqlParameter[] commandParameters)
        {
            return GetTable(CommandType.StoredProcedure, cmdText, commandParameters);
        }

        /// <summary>
        /// Sql语句专用
        /// </summary>
        /// <param name="cmdText"> T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTableText(string cmdText, SqlParameter[] commandParameters)
        {
            return GetTable(CommandType.Text, cmdText, commandParameters);
        }

        #endregion

        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">SqlCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);

                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch
            {
                throw;
            }
        }

        #region//ExecuteDataSet方法

        /// <summary>
        /// return a dataset
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    return ds;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(ConnectionString_Default, cmdType, cmdText, commandParameters);
        }

        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSetProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(ConnectionString_Default, CommandType.StoredProcedure, cmdText, commandParameters);
        }

        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="cmdText">T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSetText(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(ConnectionString_Default, CommandType.Text, cmdText, commandParameters);
        }

        public static DataView ExecuteDataSet(string sortExpression, string direction, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString_Default))
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    DataView dv = ds.Tables[0].DefaultView;
                    dv.Sort = sortExpression + " " + direction;
                    return dv;
                }
            }
            catch
            {

                throw;
            }
        }
        #endregion

        #region // ExecuteScalar方法

        /// <summary>
        /// 返回第一行的第一列
        /// </summary>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(ConnectionString_Default, cmdType, cmdText, commandParameters);
        }

        /// <summary>
        /// 返回第一行的第一列存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalarProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(ConnectionString_Default, CommandType.StoredProcedure, cmdText, commandParameters);
        }

        /// <summary>
        /// 返回第一行的第一列Sql语句专用
        /// </summary>
        /// <param name="cmdText">者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalarText(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(ConnectionString_Default, CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        #endregion

        public static SqlDataReader ExecuteDataReader(string connectionString, CommandType commandType, string commandText)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;

            connection.Open();

            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
                return null;
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
            return clonedParms;
        }

        public static DataTable ExecuteQuery(string connectionString, CommandType commandType, string sql)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 可以根据需要处理异常
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return dataTable;
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>bool结果</returns>
        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(ConnectionString_Default, CommandType.Text, strSql, cmdParms));
            if (cmdresult == 0)
                return false;
            else
                return true;
        }
        #endregion
    }
}
