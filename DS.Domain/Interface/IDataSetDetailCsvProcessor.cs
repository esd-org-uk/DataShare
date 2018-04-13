namespace DS.Domain.Interface
{
    public interface IDataSetDetailCsvProcessor
    {
        UploadResult ProcessCsv(string filePath , DataSetSchema schema, string title);
        void DeleteCsv(string filepath);

    }
}