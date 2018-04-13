using Microsoft.SqlServer.Management.Smo;

namespace DS.Domain.Interface
{
    public interface IDataSetSchemaColumnSqlRepo
    {
        ///// <summary>
        ///// add column to db table
        ///// </summary>
        ///// <param name="column"></param>
        ///// <param name="sqlDataType"></param>
        //void AddColumn(DataSetSchemaColumn column, DataType sqlDataType);

        /// <summary>
        /// add column to db table
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sqlDataType"></param>
        /// <param name="maxLength"></param>
        void AddColumn(DataSetSchemaColumn column, string sqlDataType, int maxLength);


        bool CheckSqlColumnExists(string tableName, string columnName);
        
        /// <summary>
        /// deletes column from db table
        /// </summary>
        /// <param name="column"></param>
        void DeleteColumn(DataSetSchemaColumn column);
    }
}