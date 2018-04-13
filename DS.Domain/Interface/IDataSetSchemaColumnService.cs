
namespace DS.Domain.Interface
{
    public interface IDataSetSchemaColumnService
    {
        //IRepository<DataSetSchemaColumn> Repository { get; }
        DataSetSchemaColumn Get(string categoryName, string schemaName, string columnName);
        void Create(DataSetSchemaColumn schemaCol);
        void Save(DataSetSchemaColumn columnData);
        void Delete(DataSetSchemaColumn column);
        bool SqlColumnExists(string tableName, string columnName);

        string SaveSorting(string[] columnsAndIndexes, string categoryName, string schemaName);
        //void UpdateColumnMaxSize(DataSetSchemaColumn column, int newSize);
    }
}