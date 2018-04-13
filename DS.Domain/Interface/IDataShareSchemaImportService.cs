namespace DS.Domain.Interface
{
    public interface IDataShareSchemaImportService
    {

        ImportDataSetSchemaResult ImportFromUrl(DataSetSchema schema);
        
    }
}