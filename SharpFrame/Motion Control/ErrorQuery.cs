using SharpFrame.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFrame.Motion_Control
{
    public class ErrorQuery
    {
        public static string Query(string tablename, int id)
        {
            string sqlitePath = "ErrorSql.db";
            string sqlQuery = $"SELECT * FROM {tablename} WHERE ID = {id}";
            DataSet resultDataSet = SQL_Sqlite.ExecuteQuery(sqlitePath, sqlQuery);
            var t = ConvertDataSetToDataStructure(resultDataSet);
            return t[0].Message;
        }

        public static List<DataStructure> ConvertDataSetToDataStructure(DataSet dataSet)
        {
            List<DataStructure> dataStructures = new List<DataStructure>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    DataStructure dataStructure = new DataStructure
                    {
                        ID = row["ID"]?.ToString(),
                        Message = row["Message"]?.ToString()
                    };
                    dataStructures.Add(dataStructure);
                }
            }
            return dataStructures;
        }
    }

    public class DataStructure
    {
        public string ID { get; set; }
        public string Message { get; set; }
    }
}
