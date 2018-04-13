namespace DS.Domain.Interface
{
    public interface IDataSetSchemaDefinitionService
    {
        bool SqlTableExists(string tableName);
        void CreateSqlTable(DataSetSchemaDefinition schema);
        void Delete(int definitionId);
    }
}