using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DS.Domain;
using DS.Domain.Interface;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DS.Service
{
    public class SqlTableUtility : ISqlTableUtility
    {
        public void DropTable(string tablename)
        {
            using (
                var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                if (db.Tables.Contains(tablename))
                {
                    var dbtable = db.Tables[tablename];
                    dbtable.Drop();
                }
            }
        }

        public void DropTables(List<string> tableNames)
        {
            if (tableNames.Count == 0) return;

            using (
                var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                foreach (var table in tableNames)
                {
                    if (db.Tables.Contains(table))
                        db.Tables[table].Drop();
                }
            }
        }

        public List<string> GetUnusedDSTables()
        {
            using (
                var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var sb = new System.Text.StringBuilder();
                sb.Append("SELECT * FROM information_schema.tables where Table_name like 'DS_%' ");
                sb.Append("and Table_name <> 'DS_DataSetDetails'");
                sb.Append("and Table_name <> 'DS_DataSetSchema'");
                sb.Append("and Table_name <> 'DS_DataSetSchemaColumns'");
                sb.Append("and Table_name <> 'DS_DataSetSchemaDefinition'");
                sb.Append("and Table_name <> 'DS_DebugInfo'");
                sb.Append("and Table_name <> 'DS_Group'");
                sb.Append("and Table_name <> 'DS_GroupUsers'");
                sb.Append("and Table_name <> 'DS_Theme'");
                sb.Append("and Table_name <> 'DS_WebsiteUsage'");
                sb.Append("and Table_name <> 'DS_Contact'");
                sb.Append("and Table_name <> 'DS_Category'");
                sb.Append("and Table_name <> 'DS_CategoryGroups'");
                sb.Append("and Table_name <> 'DS_SchemaESDFunctionServiceLink'");
                sb.Append("and Table_name <> 'DS_SchemaGroups'");
                sb.Append("and Table_name not in (SELECT  [TableName] FROM [dbo].[DS_DataSetSchemaDefinition])");
                var cmd = new SqlCommand(sb.ToString(), cn);
                var datatable = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(datatable);
                da.Dispose();
                cmd.Dispose();
                var result = new List<string>();

                foreach (var x in datatable.AsEnumerable())
                {
                 result.Add(x["Table_Name"].ToString());   
                }
                return result;
            }
        }
    }
}
