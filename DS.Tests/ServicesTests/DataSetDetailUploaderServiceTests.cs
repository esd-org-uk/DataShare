using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class DataSetDetailUploaderServiceTests
    {
        private IRepository<DataSetDetail> _repositoryDataSetDetail;
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetDetailSqlRepo _sqlRepo;
        private IDataSetDetailCsvProcessor _csvProcessor;

        [TestInitialize]
        public void TestInit()
        {
            _repositoryDataSetDetail = new MemoryRepository<DataSetDetail>();
            //_fakecacheprovider = new FakeCacheProvider();
            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();

              }

           );


        }
        [TestMethod]
        public void DatasetExistsAlready_when_title_is_found_and_alldataisoverwrittenonupload_is_false_returns_true()
        {
            //arrange
            var schema = new DataSetSchema() { Id = 1, IsAllDataOverwittenOnUpload = false };
            var dsd1 = new DataSetDetail() { Title = "title", Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(1)).Returns(schema);
            _dataSetSchemaService = mock.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.DatasetExistsAlready(1, "title");
            //assert
            Assert.AreEqual(true, result);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
        }

        [TestMethod]
        public void DatasetExistsAlready_when_title_is_found_and_alldataisoverwrittenonupload_true_returns_false()
        {
            //arrange
            var schema = new DataSetSchema() { Id = 1, IsAllDataOverwittenOnUpload = true };
            var dsd1 = new DataSetDetail() { Title = "title", Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(1)).Returns(schema);
            _dataSetSchemaService = mock.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.DatasetExistsAlready(1, "title");
            //assert
            Assert.AreEqual(false, result);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
        }

        [TestMethod]
        public void DatasetExistsAlready_when_title_isnot_found_and_alldataisoverwrittenonupload_true_returns_false()
        {
            //arrange
            var schema = new DataSetSchema() { Id = 1, IsAllDataOverwittenOnUpload = true };
            var dsd1 = new DataSetDetail() { Title = "title", Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(1)).Returns(schema);
            _dataSetSchemaService = mock.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.DatasetExistsAlready(1, "tidsatle");
            //assert
            Assert.AreEqual(false, result);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
        }

        [TestMethod]
        public void SaveCsv_returns_type_of_upload_result()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            _dataSetSchemaService = mock.Object;
            var mock2 = new Mock<IDataSetDetailCsvProcessor>();
            _csvProcessor = mock2.Object;
            var mock3 = new Mock<IDataSetDetailSqlRepo>();
            _sqlRepo = mock3.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.SaveCsv(1, "title", "filepathhere");
            //assert
            Assert.AreEqual(typeof(UploadResult), result.GetType());
            //cleanup
            _dataSetSchemaService = null;
            _csvProcessor = null;
            _sqlRepo = null;
        }
        [TestMethod]
        public void SaveCsv_when_schema_cant_be_found_from_id_returns_null_will_return_uploadResult_with_error_message_unable_to_find_schema()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            _dataSetSchemaService = mock.Object;
            var mock2 = new Mock<IDataSetDetailCsvProcessor>();
            _csvProcessor = mock2.Object;
            var mock3 = new Mock<IDataSetDetailSqlRepo>();
            _sqlRepo = mock3.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.SaveCsv(1, "title", "filepathhere");
            //assert
            Assert.AreEqual("Unable to find schema 1", result.Errors[0]);
            //cleanup
            _dataSetSchemaService = null;
            _csvProcessor = null;
            _sqlRepo = null;
        }
        [TestMethod]
        public void SaveCsv_when_processCsv_returns_null_will_return_uploadResult_with_error_message_unable_to_process_from_filepath()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();

            mock.Setup(x => x.Get(1)).Returns(new DataSetSchema());
            _dataSetSchemaService = mock.Object;
            var mock2 = new Mock<IDataSetDetailCsvProcessor>();
            _csvProcessor = mock2.Object;
            var mock3 = new Mock<IDataSetDetailSqlRepo>();
            _sqlRepo = mock3.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.SaveCsv(1, "title", "filepathhere");
            //assert
            Assert.AreEqual("Unable to process csv from filepathhere", result.Errors[0]);
            //cleanup
            _dataSetSchemaService = null;
            _csvProcessor = null;
            _sqlRepo = null;
        }

        [TestMethod]
        public void SaveCsv_when_exception_is_thrown_returns_uploadresult_with_error_message_contains_error_validating()
        {
            //arrange
            var schema = new DataSetSchema();
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(1)).Returns(schema);
            _dataSetSchemaService = mock.Object;
            var uploadResult = new UploadResult();
            var mock2 = new Mock<IDataSetDetailCsvProcessor>();
            mock2.Setup(x => x.ProcessCsv("filepathhere", schema, "title")).Returns(uploadResult);
            _csvProcessor = mock2.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.SaveCsv(1, "title", "filepathhere");
            //assert
            Assert.AreEqual(true, result.Errors[0].Contains("Error Validating"));
            //cleanup
            _dataSetSchemaService = null;
            _csvProcessor = null;
            _sqlRepo = null;
        }

        [TestMethod]
        public void AddExternalDataSet_returns_datasetdetail_with_title()
        {
            //arrange

            var mock = new Mock<IDataSetSchemaService>();
            var repositoryDataSetSchema = new MemoryRepository<DataSetSchema>();
            mock.Setup(x => x.Repository).Returns(repositoryDataSetSchema);
            _dataSetSchemaService = mock.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.AddExternalDataSet(1, "title", "url", "type");
            //assert
            Assert.AreEqual("title", result.Title);
            //cleanup
            _dataSetSchemaService = null;
        }

        [TestMethod]
        public void AddExternalDataSet_returns_datasetdetail_fileurl_with_url()
        {
            //arrange

            var mock = new Mock<IDataSetSchemaService>();
            var repositoryDataSetSchema = new MemoryRepository<DataSetSchema>();
            mock.Setup(x => x.Repository).Returns(repositoryDataSetSchema);
            _dataSetSchemaService = mock.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.AddExternalDataSet(1, "title", "url", "type");
            //assert
            Assert.AreEqual("url", result.FileUrl);
            //cleanup
            _dataSetSchemaService = null;
        }


        [TestMethod]
        public void AddExternalDataSet_add_dataset_into_repository()
        {
            //arrange

            var mock = new Mock<IDataSetSchemaService>();
            var repositoryDataSetSchema = new MemoryRepository<DataSetSchema>();
            mock.Setup(x => x.Repository).Returns(repositoryDataSetSchema);
            _dataSetSchemaService = mock.Object;
            var sut = new DataSetDetailUploaderService(_dataSetSchemaService, _repositoryDataSetDetail, _csvProcessor, _sqlRepo);
            //act
            var result = sut.AddExternalDataSet(1, "titleinserted", "url", "type");
            var resultfromrepo = _repositoryDataSetDetail.GetQuery().FirstOrDefault(x => x.Title == "titleinserted");
            //assert
            Assert.AreEqual("titleinserted", resultfromrepo.Title);
            //cleanup
            _dataSetSchemaService = null;
        }

    }
}
