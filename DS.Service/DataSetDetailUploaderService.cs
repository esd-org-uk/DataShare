using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS.Domain;
using DS.Domain.Interface;
using Elmah;

namespace DS.Service
{
    public class DataSetDetailUploaderService : IDataSetDetailUploaderService
    {
        private IDataSetSchemaService _dataSetSchemaService;
        private IRepository<DataSetDetail> _repositoryDataSetDetails;
        private IDataSetDetailCsvProcessor _csvProcessor;
        private IDataSetDetailSqlRepo _dataSetDetailSqlRepo;

        public DataSetDetailUploaderService(IDataSetSchemaService dataSetSchemaService
            , IRepository<DataSetDetail> repositoryDataSetDetails
            ,IDataSetDetailCsvProcessor csvProcessor
            , IDataSetDetailSqlRepo dataSetDetailSqlRepo
            )
        {
            _dataSetSchemaService = dataSetSchemaService;
            _repositoryDataSetDetails = repositoryDataSetDetails;
            _csvProcessor = csvProcessor;
            _dataSetDetailSqlRepo = dataSetDetailSqlRepo;
        }
        public bool DatasetExistsAlready(int schemaId, string title)
        {
            var checkIfTitleExistsInDataSetDetail = _repositoryDataSetDetails.GetQuery().FirstOrDefault(s => s.Schema.Id == schemaId && s.Title.ToLower().Trim() == title.ToLower().Trim());
            var schemaDef = _dataSetSchemaService.Get(schemaId);
            return !schemaDef.IsAllDataOverwittenOnUpload && checkIfTitleExistsInDataSetDetail != null;
        }

        public UploadResult SaveCsv(int schemaId, string title, string filePath)
        {

            var schemaDef = _dataSetSchemaService.Get(schemaId);
            if (schemaDef == null) return UploadResultError("Unable to find schema " + schemaId.ToString());
            try
            {
                var result = _csvProcessor.ProcessCsv(filePath, schemaDef, title);
                if (result == null) return UploadResultError("Unable to process csv from " + filePath);

                _dataSetDetailSqlRepo.SaveToDatabase(schemaDef.Definition, result, _repositoryDataSetDetails.DbConnectionString);

                //if there was no errors remove all previous data if the schema is setup to
                if (result.Errors.Count == 0 && schemaDef.IsAllDataOverwittenOnUpload)
                    RemovePreviouslyUploadedData(schemaDef.Definition.TableName, schemaDef.Id, result.Id);
                
                _csvProcessor.DeleteCsv(filePath);

                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                _csvProcessor.DeleteCsv(filePath);
                return UploadResultError(String.Format("Error Validating: {0}", ex.Message));
            }
        }

        public DataSetDetail AddExternalDataSet(int schemaId, string title, string url, string type)
        {
            var datasetDetail = new DataSetDetail
            {
                Schema = _dataSetSchemaService.Repository.Query(s => s.Id == schemaId).FirstOrDefault(),
                Title = title,
                VersionNumber = 1,
                FileUrl = url,
                FileType = type.ToUpper(),
                CsvFileSize = 0,
                XmlFileSize = 0
            };

            _repositoryDataSetDetails.Add(datasetDetail);
            _repositoryDataSetDetails.SaveChanges();
            return datasetDetail;
        }

        private void RemovePreviouslyUploadedData(string schemaTable, int schemaId, int newDataSetDetailId)
        {
            //remove previous uploaded data
            _repositoryDataSetDetails.ExecuteRawSql(String.Format("DELETE FROM DS_DataSetDetails WHERE Schema_Id = {0} and Id != {1}", schemaId, newDataSetDetailId));
            _repositoryDataSetDetails.ExecuteRawSql(String.Format("IF OBJECT_ID('dbo.{0}', 'U') IS NOT NULL  DELETE FROM {0} WHERE DataSetDetailId != {1}", schemaTable, newDataSetDetailId));
        }
        public void DeleteDataSetDetail(DataSetDetail dataSetDetail)
        {
            _repositoryDataSetDetails.Delete(dataSetDetail);
            _repositoryDataSetDetails.SaveChanges();
        }
        public void DeleteExternalDataSetDetail(DataSetDetail dataSetDetail)
        {
            var deleteSql = String.Format(@"exec DS_DataSet_Delete ""{0}"", {1}", dataSetDetail.Schema.Definition.TableName, dataSetDetail.Id);
            _dataSetDetailSqlRepo.ExecuteQuery(_repositoryDataSetDetails.DbConnectionString, deleteSql, null);
        }
        private UploadResult UploadResultError(string s)
        {
            return new UploadResult { Errors = new List<string> { s } };
        }
    }
}
