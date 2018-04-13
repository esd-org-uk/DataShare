namespace DS.Domain
{
    public class ImportDataSetSchemaResult
    {
        public ImportDataSetSchemaResult()
        {
            ErrorMessage = "";
        }
        public DataSetSchema DataSetSchema { get; set; }
        public string ErrorMessage { get; set; }
    }
}