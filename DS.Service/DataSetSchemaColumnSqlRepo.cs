
namespace DS.Service
{
    using System.Configuration;
    using System.Data.SqlClient;
    using DS.Domain;
    using DS.Domain.Interface;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;

    public class DataSetSchemaColumnSqlRepo : IDataSetSchemaColumnSqlRepo
    {
        //public void AddColumn(DataSetSchemaColumn column, DataType sqlDataType)
        //{
        //    using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
        //    {
        //        cn.Open();
        //        var server = new Server(new ServerConnection(cn));
        //        var db = server.Databases[cn.Database];
        //        var dbTable = db.Tables[column.SchemaDefinition.TableName];
        //        var newColumn = new Column(dbTable, column.ColumnName)
        //        {
        //            DataType = sqlDataType
        //        };
        //        dbTable.Columns.Add(newColumn);
        //        dbTable.Alter();
        //    }
        //}

        public void AddColumn(DataSetSchemaColumn column, string sqlDataType, int maxLength)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                var dbTable = db.Tables[column.SchemaDefinition.TableName];
                var newColumn = new Column(dbTable, column.ColumnName)
                {
                    DataType = GetDataType(sqlDataType, maxLength)
                };
                dbTable.Columns.Add(newColumn);
                dbTable.Alter();
            }
        }

        private DataType GetDataType(string sqlDataType, int maxLength)
        {
            var sqlDataTypeObj = new DataType();
            switch (sqlDataType)
            {
                case "NVarChar":
                    sqlDataTypeObj = DataType.NVarChar(maxLength);
                    break;
                case "Float":
                    sqlDataTypeObj = DataType.Float;
                    break;
                case "DateTime":
                    sqlDataTypeObj = DataType.DateTime;
                    break;
            }
            return sqlDataTypeObj;
        }

        public bool CheckSqlColumnExists(string tableName, string columnName)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                if (db.Tables.Contains(tableName))
                {
                    var table = db.Tables[tableName];
                    return table.Columns.Contains(columnName);
                }
                return false;
            }
        }

        public void DeleteColumn(DataSetSchemaColumn column)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                if (db.Tables.Contains(column.SchemaDefinition.TableName))
                {
                    var dbtable = db.Tables[column.SchemaDefinition.TableName];
                    var dbColumn = dbtable.Columns[column.ColumnName];
                    dbColumn.Drop();
                }
            }
        }
    }
}
