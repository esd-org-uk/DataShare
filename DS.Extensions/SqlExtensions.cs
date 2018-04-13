using System;
using System.Data;
using System.Data.SqlClient;

namespace DS.Extensions
{
    public static class SqlExtensions
    {
        public static DataSet ExecuteQuery(string connectionString, string sql)
        {
            return ExecuteQuery(connectionString, sql, null);
        }

        public static DataSet ExecuteQuery(string connectionString, string sql, Array parameters)
        {
            var list = new DataSet();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    var mySqlDataAdapter = new SqlDataAdapter(cmd);
                    mySqlDataAdapter.Fill(list);
                }
            }

            return list;
        }
    }
}
