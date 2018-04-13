using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
    public class DataSetDetailServiceTests
    {
        private IRepository<DataSetDetail> _repositoryDataSetDetail;
        private ISystemConfigurationService _systemConfigurationService;
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetDetailSqlRepo _sqlRepo;
        private ISqlOrderByColumnAndDirectionFormatter _sqlColumnTextFormatter;

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
            _sqlColumnTextFormatter = new SqlOrderByColumnAndDirectionFormatter();
            
        }

        [TestMethod]
        public void Get_will_return_dataset_detail_based_on_selected_id()
        {
            //arrange
            var datasetdetail = new DataSetDetail() {Id = 1, Title = "datasetdetail1"};
            _repositoryDataSetDetail.Add(datasetdetail);
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Get(1);
            //assert
            Assert.AreEqual("datasetdetail1", result.Title);
            //cleanup
            _repositoryDataSetDetail.Delete(datasetdetail);
        }

        [TestMethod]
        public void GetTemplateCsv_will_return_csv_from_schema_id()
        {
            //arrange
            var schema = new DataSetSchema()
                {
                    Id=1,
                    Definition = new DataSetSchemaDefinition()
                        {
                            Columns = new List<DataSetSchemaColumn>()
                                {
                                    new DataSetSchemaColumn(){ColumnName = "col1"}
                                    ,new DataSetSchemaColumn(){ColumnName = "col2"}
                                    ,new DataSetSchemaColumn(){ColumnName = "col3"}
                                }
                        }
                };
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(1)).Returns(schema);
            _dataSetSchemaService = mock.Object;

            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            
            //act
            var result = sut.GetTemplateCsv(1);
            
            //assert
            Assert.AreEqual("\"col1\",\"col2\",\"col3\"", result);
            //cleanup

        }

        [TestMethod]
        public void GetData_when_datasetdetail_is_null_returns_null()
        {
            //arrange
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.GetData("emptyurl", "emptyschemaurl");
            //assert
            Assert.AreEqual(null, result);
            //cleanup
        }

        [TestMethod]
        public void GetData_when_datasetdetail_is_not_null_returns_datatable()
        {
            //arrange
            var datasetdetail = new DataSetDetail(){Title = "datasetdetail title here", Schema = new DataSetSchema(){Title="schema title here", Definition = new DataSetSchemaDefinition(){TableName="table1", Columns = new List<DataSetSchemaColumn>(){new DataSetSchemaColumn(){Title="col1"}}}}};
            _repositoryDataSetDetail.Add(datasetdetail);
            var ds = new DataSet();
            ds.Tables.Add(new DataTable());
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryDatasetDetailId(_repositoryDataSetDetail.DbConnectionString, datasetdetail.Schema.Definition, datasetdetail.Id)).Returns(ds);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.GetData("datasetdetail-title-here", "schema-title-here");
            //assert
            Assert.AreEqual(typeof(DataTable), result.GetType());
            //cleanup
        }

        [TestMethod]
        public void SearchSchema_will_return_view_controller_data_object_with_value_of_first_column_in_first_row_in_first_datatable()
        {
            //arrange
            
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int currentPage = 0;
            int pageSize = 10;
            string orderByColumn = "";
            string orderDirection = "desc";
            bool getTotals = false;
            var mock = new Mock<IDataSetDetailSqlRepo>();
            var ds = new DataSet();
            
            ds.Tables.Add(ReturnDataTable(null, 2));
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            mock.Setup(
                x =>
                x.ExecuteQuerySearchSchema(_repositoryDataSetDetail.DbConnectionString, filter, schema, currentPage,
                                           pageSize, orderByColumn, orderDirection)).Returns(ds);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.SearchSchema(filter, schema, currentPage, pageSize, orderByColumn, orderDirection, getTotals);
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
        }

        [TestMethod]
        public void SearchSchema_when_orderdirection_is_null_empty_will_set_order_direction_to_asc()
        {
            //arrange

            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int currentPage = 0;
            int pageSize = 10;
            string orderByColumn = "";
            string orderDirection = "ASC";
            bool getTotals = false;
            var mock = new Mock<IDataSetDetailSqlRepo>();
            var ds = new DataSet();

            ds.Tables.Add(ReturnDataTable(null, 2));
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            mock.Setup(
                x =>
                x.ExecuteQuerySearchSchema(_repositoryDataSetDetail.DbConnectionString, filter, schema, currentPage,
                                           pageSize, orderByColumn, orderDirection)).Returns(ds);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.SearchSchema(filter, schema, currentPage, pageSize, orderByColumn, "", getTotals);
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
        }

        [TestMethod]
        public void SearchSchema_will_return_view_controller_data_object_with_total_pages_if_zero_willreturn_total_of_1()
        {
            //arrange

            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int currentPage = 0;
            int pageSize = 10;
            string orderByColumn = "";
            string orderDirection = "desc";
            bool getTotals = false;
            var mock = new Mock<IDataSetDetailSqlRepo>();
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 0));
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            mock.Setup(
                x =>
                x.ExecuteQuerySearchSchema(_repositoryDataSetDetail.DbConnectionString, filter, schema, currentPage,
                                           pageSize, orderByColumn, orderDirection)).Returns(ds);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.SearchSchema(filter, schema, currentPage, pageSize, orderByColumn, orderDirection, getTotals);
            //assert
            Assert.AreEqual(1, result.TotalPages);
            //cleanup
        }
        [TestMethod]
        public void SearchSchema_will_return_view_controller_data_object_with_total_pages_if_not_zero_and_count_mod_page_size_more_than_zero_will_return_value_of_total_pages_plus_1()
        {
            //arrange

            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int currentPage = 0;
            int pageSize = 10;
            string orderByColumn = "";
            string orderDirection = "DESC";
            bool getTotals = false;
            var mock = new Mock<IDataSetDetailSqlRepo>();
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 51));
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            mock.Setup(
                x =>
                x.ExecuteQuerySearchSchema(_repositoryDataSetDetail.DbConnectionString, filter, schema, currentPage,
                                           pageSize, orderByColumn, orderDirection)).Returns(ds);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.SearchSchema(filter, schema, currentPage, pageSize, orderByColumn, orderDirection, getTotals);
            //assert
            Assert.AreEqual(6, result.TotalPages);
            //cleanup
        }

        [TestMethod]
        public void SearchSchema_will_return_view_controller_data_object_with_total_pages_if_not_zero_will_return_value_of_total_pages()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int currentPage = 0;
            int pageSize = 10;
            string orderByColumn = "";
            string orderDirection = "DESC";
            bool getTotals = false;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(
                x =>
                x.ExecuteQuerySearchSchema(_repositoryDataSetDetail.DbConnectionString, filter, schema, currentPage,
                                           pageSize, orderByColumn, orderDirection)).Returns(ds);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.SearchSchema(filter, schema, currentPage, pageSize, orderByColumn, orderDirection, getTotals);
            //assert
            Assert.AreEqual(5, result.TotalPages);
            //cleanup
        }


        //[TestMethod]
        //public void SearchSchema_when_orderByColumn_is_not_null_or_empty_use_orderbyColumn_parameter()
        //{
        //    //arrange
        //    var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
        //    //action
        //    var result = sut.SearchSchema(filter, schema, "orderbycolumn", "");

        //    //assert
        //    Assert.AreEqual();
        //    //cleanup
        //}

        [TestMethod]
        public void
            VisualiseSchema_will_return_type_of_view_controller_data()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int pageNumber = 0;
            int pageSize = 0;
            string chartType = null;
            string xAxis = null;
            string yAxis = null;
            string yAxisAggregate = null;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchema(filter, schema, pageNumber, pageSize, chartType, xAxis, yAxis, yAxisAggregate);
            //assert
            Assert.AreEqual(typeof(ViewControllerData), result.GetType());
        }
        [TestMethod]
        public void
            VisualiseSchema_when_datatable_2_content_has_int_more_than_zero_will_view_controller_data_has_negative_values_true()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int pageNumber = 0;
            int pageSize = 1;
            string chartType = null;
            string xAxis = "123";
            string yAxis = "11";
            string yAxisAggregate = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 1)); //this is tested ( count  of negative values) 
            var vsCriteria = new VisualSchemaCriteria("", null, null, "", "", "", "", "", "", 0, 0);
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchema(vsCriteria)).Returns(ds);
            mock.Setup(
                x =>
                x.ConvertToVisualSchemaCriteria(_repositoryDataSetDetail.DbConnectionString, filter, schema, chartType,
                                                xAxis, yAxis, yAxisAggregate, pageNumber, pageSize)).Returns(vsCriteria);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchema(filter, schema, pageNumber, pageSize, chartType, xAxis, yAxis, yAxisAggregate);
            //assert
            Assert.AreEqual(true, result.HasNegativeValues);
            //cleanup
            _sqlRepo = null;
        }


        [TestMethod]
        public void
            VisualiseSchema_returns_viewcontrollerdata_currentpage_is_equals_to_pagenumber()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int pageNumber = 3;
            int pageSize = 1;
            string chartType = null;
            string xAxis = "123";
            string yAxis = "11";
            string yAxisAggregate = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 1)); //this is tested ( count  of negative values) 
            var vsCriteria = new VisualSchemaCriteria("", null, null, "", "", "", "", "", "", 0,0);
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchema(vsCriteria)).Returns(ds);
            mock.Setup(
                x =>
                x.ConvertToVisualSchemaCriteria(_repositoryDataSetDetail.DbConnectionString, filter, schema, chartType,
                                                xAxis, yAxis, yAxisAggregate, pageNumber, pageSize)).Returns(vsCriteria);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchema(filter, schema, pageNumber, pageSize, chartType, xAxis, yAxis, yAxisAggregate);
            //assert
            Assert.AreEqual(3, result.CurrentPage);
            //cleanup
            _sqlRepo = null;
        }


        [TestMethod]
        public void
            VisualiseSchema_when_total_pages_is_0_returns_viewcontrollerdata_totalpages_is_1()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int pageNumber = 3;
            int pageSize = 1;
            string chartType = null;
            string xAxis = "123";
            string yAxis = "11";
            string yAxisAggregate = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 0));
            ds.Tables.Add(ReturnDataTable(null, 1)); //this is tested ( count  of negative values) 
            var vsCriteria = new VisualSchemaCriteria("", null, null, "", "", "", "", "", "", 0, 0);
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchema(vsCriteria)).Returns(ds);
            mock.Setup(
                x =>
                x.ConvertToVisualSchemaCriteria(_repositoryDataSetDetail.DbConnectionString, filter, schema, chartType,
                                                xAxis, yAxis, yAxisAggregate, pageNumber, pageSize)).Returns(vsCriteria);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchema(filter, schema, pageNumber, pageSize, chartType, xAxis, yAxis, yAxisAggregate);
            //assert
            Assert.AreEqual(3, result.CurrentPage);
            //cleanup
            _sqlRepo = null;
        }
        [TestMethod]
        public void
            VisualiseSchema_when_total_pages_is_morethan_zero_returns_viewcontrollerdata_totalpages_is_equal_to_total_pages()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int pageNumber = 1;
            int pageSize = 3;
            string chartType = null;
            string xAxis = "123";
            string yAxis = "11";
            string yAxisAggregate = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 9));
            ds.Tables.Add(ReturnDataTable(null, 1)); //this is tested ( count  of negative values) 
            var vsCriteria = new VisualSchemaCriteria("", null, null, "", "", "", "", "", "", 0, 0);
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchema(vsCriteria)).Returns(ds);
            mock.Setup(
                x =>
                x.ConvertToVisualSchemaCriteria(_repositoryDataSetDetail.DbConnectionString, filter, schema, chartType,
                                                xAxis, yAxis, yAxisAggregate, pageNumber, pageSize)).Returns(vsCriteria);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchema(filter, schema, pageNumber, pageSize, chartType, xAxis, yAxis, yAxisAggregate);
            //assert
            Assert.AreEqual(3, result.TotalPages);
            //cleanup
            _sqlRepo = null;
        }

        [TestMethod]
        public void
            VisualiseSchema_when_count_mod_page_size_more_than_zero_total_pages_returns_viewcontrollerdata_totalpages_is_equal_to_total_pages_plus_1()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            int pageNumber = 1;
            int pageSize = 3;
            string chartType = null;
            string xAxis = "123";
            string yAxis = "11";
            string yAxisAggregate = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 50));
            ds.Tables.Add(ReturnDataTable(null, 10));//10 mod 3  > 0
            ds.Tables.Add(ReturnDataTable(null, 1)); //this is tested ( count  of negative values) 
            var vsCriteria = new VisualSchemaCriteria("", null, null, "", "", "", "", "", "", 0, 0);
            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchema(vsCriteria)).Returns(ds);
            mock.Setup(
                x =>
                x.ConvertToVisualSchemaCriteria(_repositoryDataSetDetail.DbConnectionString, filter, schema, chartType,
                                                xAxis, yAxis, yAxisAggregate, pageNumber, pageSize)).Returns(vsCriteria);
            _sqlRepo = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchema(filter, schema, pageNumber, pageSize, chartType, xAxis, yAxis, yAxisAggregate);
            //assert
            Assert.AreEqual(4, result.TotalPages);
            //cleanup
            _sqlRepo = null;
        }



        [TestMethod]
        public void VisualiseSchemaAsMap_returns_type_of_viewcontroller_type()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 0));
            ds.Tables.Add(ReturnDataTable(null, 0));
            ds.Tables.Add(ReturnDataTable(null, 0)); 

            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchemaMap(_repositoryDataSetDetail.DbConnectionString, null, null)).Returns(ds);
            _sqlRepo = mock.Object;
            var mock2 = new Mock<ISystemConfigurationService>();
            mock2.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject());
            _systemConfigurationService = mock2.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchemaAsMap(filter, schema);
            //assert
            Assert.AreEqual(typeof(ViewControllerData), result.GetType());
            //cleanup
            _sqlRepo = null;
            _systemConfigurationService = null;

        }
        [TestMethod]
        public void VisualiseSchemaAsMap_returns_viewcontroller_with_count_as_content_of_datatable_1_first_row_col_value()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 0));//table1
            ds.Tables.Add(ReturnDataTable(null, 1));//table2
            ds.Tables.Add(ReturnDataTable(null, 0));//table3

            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchemaMap(_repositoryDataSetDetail.DbConnectionString, null, null)).Returns(ds);
            _sqlRepo = mock.Object;
            var mock2 = new Mock<ISystemConfigurationService>();
            mock2.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject());
            _systemConfigurationService = mock2.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchemaAsMap(filter, schema);
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _sqlRepo = null;
            _systemConfigurationService = null;

        }
        [TestMethod]
        public void VisualiseSchemaAsMap_returns_viewcontroller_with_mapcentrelatitude_as_from_systemconfiguration_service()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 0));//table1
            ds.Tables.Add(ReturnDataTable(null, 1));//table2
            ds.Tables.Add(ReturnDataTable(null, 0));//table3

            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchemaMap(_repositoryDataSetDetail.DbConnectionString, null, null)).Returns(ds);
            _sqlRepo = mock.Object;
            var mock2 = new Mock<ISystemConfigurationService>();
            mock2.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject(){MapCentreLatitude = "123"});
            _systemConfigurationService = mock2.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchemaAsMap(filter, schema);
            //assert
            Assert.AreEqual(123.00, result.MapCentreLatitude);
            //cleanup
            _sqlRepo = null;
            _systemConfigurationService = null;

        }
        [TestMethod]
        public void VisualiseSchemaAsMap_returns_viewcontroller_with_mapcentrelongitude_as_from_systemconfiguration_service()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 0));//table1
            ds.Tables.Add(ReturnDataTable(null, 1));//table2
            ds.Tables.Add(ReturnDataTable(null, 0));//table3

            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchemaMap(_repositoryDataSetDetail.DbConnectionString, null, null)).Returns(ds);
            _sqlRepo = mock.Object;
            var mock2 = new Mock<ISystemConfigurationService>();
            mock2.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject() { MapCentreLongitude = "321" });
            _systemConfigurationService = mock2.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchemaAsMap(filter, schema);
            //assert
            Assert.AreEqual(321.00, result.MapCentreLongitude);
            //cleanup
            _sqlRepo = null;
            _systemConfigurationService = null;

        }

        [TestMethod]
        public void VisualiseSchemaAsMap_returns_viewcontroller_with_mapdefaultzoom_as_from_systemconfiguration_service()
        {
            //arrange
            IList<FilterCriteria> filter = null;
            DataSetSchema schema = null;
            var ds = new DataSet();
            ds.Tables.Add(ReturnDataTable(null, 0));//table1
            ds.Tables.Add(ReturnDataTable(null, 1));//table2
            ds.Tables.Add(ReturnDataTable(null, 0));//table3

            var mock = new Mock<IDataSetDetailSqlRepo>();
            mock.Setup(x => x.ExecuteQueryVisualiseSchemaMap(_repositoryDataSetDetail.DbConnectionString, null, null)).Returns(ds);
            _sqlRepo = mock.Object;
            var mock2 = new Mock<ISystemConfigurationService>();
            mock2.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject() { MapDefaultZoom = "3" });
            _systemConfigurationService = mock2.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.VisualiseSchemaAsMap(filter, schema);
            //assert
            Assert.AreEqual(3, result.MapDefaultZoom);
            //cleanup
            _sqlRepo = null;
            _systemConfigurationService = null;

        }

        [TestMethod]
        public void Search_returns_type_of_ListDataSetDetail()
        {
            //arrange
            string to = null;
            string from = null;
            var searchText = "";
            int schemaId = 0;
            var mock = new Mock<IRepository<DataSetDetail>>();
            _repositoryDataSetDetail = mock.Object;
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Search(schemaId, searchText, from, to);
            //assert
            Assert.AreEqual(typeof(List<DataSetDetail>), result.GetType());
            //cleanup
        }

        [TestMethod]
        public void Search_when_from_date_and_to_date_is_not_null_and_both_dates_are_more_than_min_datevalue_returns_datasetdetail_with_correct_filtered_dates_list_of_DataSetDetail()
        {
            //arrange
            string to = "2010-1-1";//new DateTime(2010, 1, 1);
            string from = "01/12/2009";//new DateTime(2009, 12, 1);
            var searchText = "dstitle";
            int schemaId = 0;
            var schema = new DataSetSchema() {Title = "dstitle", Id = 0};
            var dsd1 = new DataSetDetail() {DateUpdated = new DateTime(2009, 11, 1), Schema = schema};
            var dsd2 = new DataSetDetail() {DateUpdated = new DateTime(2009, 12, 2), Schema = schema};
            _repositoryDataSetDetail.Add( dsd1);
            _repositoryDataSetDetail.Add(dsd2);
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Search(schemaId, searchText, from, to);
            //assert
            Assert.AreEqual(new DateTime(2009,12,2), result[0].DateUpdated);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
            _repositoryDataSetDetail.Delete(dsd2);
        }

        [TestMethod]
        public void Search_when_fromdate_is_not_null_and_date_is_more_than_min_datevalue_returns_datasetdetail_with_correct_filtered_larger_than_fromdate_list_of_DataSetDetail()
        {
            //arrange
            string to = null;
            string from = "2009-12-1";//new DateTime(2009, 12, 1);
            var searchText = "dstitle";
            int schemaId = 0;
            var schema = new DataSetSchema() { Title = "dstitle", Id = 0 };
            var dsd1 = new DataSetDetail() { DateUpdated = new DateTime(2009, 11, 1), Schema = schema };
            var dsd2 = new DataSetDetail() { DateUpdated = new DateTime(2009, 12, 2), Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);
            _repositoryDataSetDetail.Add(dsd2);
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Search(schemaId, searchText, from, to);
            //assert
            Assert.AreEqual(new DateTime(2009, 12, 2), result[0].DateUpdated);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
            _repositoryDataSetDetail.Delete(dsd2);
        }

        [TestMethod]
        public void Search_when_todate_is_not_null_and_date_is_more_than_min_datevalue_returns_datasetdetail_with_correct_filtered_smaller_than_todate_list_of_DataSetDetail()
        {
            //arrange
            string to = "01/01/2010";// new DateTime(2010, 1, 1);
            string from = null;
            var searchText = "dstitle";
            int schemaId = 0;
            var schema = new DataSetSchema() { Title = "dstitle", Id = 0 };
            var dsd1 = new DataSetDetail() { DateUpdated = new DateTime(2010, 11, 1), Schema = schema };
            var dsd2 = new DataSetDetail() { DateUpdated = new DateTime(2009, 12, 2), Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);
            _repositoryDataSetDetail.Add(dsd2);
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Search(schemaId, searchText, from, to);
            //assert
            Assert.AreEqual(new DateTime(2009, 12, 2), result[0].DateUpdated);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
            _repositoryDataSetDetail.Delete(dsd2);
        }


        [TestMethod]
        public void Search_when_fromdate_and_todate_isnull_returns_all_datasetdetail()
        {
            //arrange
            string to = null;
            string from = null;
            var searchText = "dstitle";
            int schemaId = 0;
            var schema = new DataSetSchema() { Title = "dstitle", Id = 0 };
            var dsd1 = new DataSetDetail() { DateUpdated = new DateTime(2010, 11, 1), Schema = schema };
            var dsd2 = new DataSetDetail() { DateUpdated = new DateTime(2009, 12, 2), Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);
            _repositoryDataSetDetail.Add(dsd2);
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Search(schemaId, searchText, from, to);
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
            _repositoryDataSetDetail.Delete(dsd2);
        }


        [TestMethod]
        public void Search_when_fromdate_and_todate_isemptyString_returns_all_datasetdetail()
        {
            //arrange
            
            var searchText = "dstitle";
            int schemaId = 0;
            var schema = new DataSetSchema() { Title = "dstitle", Id = 0 };
            var dsd1 = new DataSetDetail() { DateUpdated = new DateTime(2010, 11, 1), Schema = schema };
            var dsd2 = new DataSetDetail() { DateUpdated = new DateTime(2009, 12, 2), Schema = schema };
            _repositoryDataSetDetail.Add(dsd1);
            _repositoryDataSetDetail.Add(dsd2);
            var sut = new DataSetDetailService(_systemConfigurationService, _repositoryDataSetDetail, _dataSetSchemaService, _sqlRepo, _sqlColumnTextFormatter);
            //act
            var result = sut.Search(schemaId, searchText, "", "");
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
            _repositoryDataSetDetail.Delete(dsd1);
            _repositoryDataSetDetail.Delete(dsd2);
        }


        private DataTable ReturnDataTable(string strRow1Col1Value, int? intRow1Col1Value)
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn());
            var row = dt.NewRow();
            if (strRow1Col1Value == null)
                row[0] = Convert.ToInt32(intRow1Col1Value);
            else
                row[0] = strRow1Col1Value;

            dt.Rows.Add(row);
            return dt;
        }

    }
}
