namespace DS.Domain.Interface
{
    public interface IDataSetDetailUploaderService
    {
        bool DatasetExistsAlready(int schemaId, string title);
        UploadResult SaveCsv(int schemaId, string title, string filePath);

        DataSetDetail AddExternalDataSet(int schemaId, string title, string url, string type);
        void DeleteDataSetDetail(DataSetDetail dataSetDetail);
        void DeleteExternalDataSetDetail(DataSetDetail dataSetDetail);
    }
}