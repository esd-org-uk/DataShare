using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.WebUI.Controllers;
using DS.WebUI.Controllers.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DS.Tests.ControllersTests
{
    [TestClass]
    public class ViewControllerTests
    {
        private ISystemConfigurationService _sysConfigService;
        private ICategoryService _categoryService;
        private IDataSetDetailService _datasetDetailService;
        private IDataSetSchemaService _dataSchemaService;

        [TestInitialize]
        public void TestInit()
        {
            var mock = new Mock<ISystemConfigurationService>();
            mock.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject());
            mock.Setup(x => x.AppSettingsInt("DefaultGraphPageSize")).Returns(10);
            mock.Setup(x => x.AppSettingsInt("DefaultViewPageSize")).Returns(103);

            _sysConfigService = mock.Object;
        }

        [TestMethod]
        public void Index_when_schema_is_not_null_and_schema_detail_is_not_null_and_schema_is_not_online_and_preview_is_not_true_returns_partial_view_offline()
        {
            //arrange
            var schema = "schemanotNull";
            var schemaDetail = new DataSetSchema();
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock.Object;
                var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action
            var filter = new List<FilterCriteria>();
            var result = (PartialViewResult) sut.Index("category", schema, 1, 1, "", "", filter, "", "previewNotTrue");
            //assert
            Assert.AreEqual("Offline", result.ViewName);
            //cleanup
            _dataSchemaService = null;


        }


        [TestMethod]
        public void Index_when_schema_is_not_null_and_schema_detail_is_not_null_and_schema_is_online_download_is_not_null_or_empty_and_returns_typeOf_csvResult()
        {
            //arrange
            var schema = "schemanotNull";
            var schemaDetail = new DataSetSchema() { IsDisabled = false, IsApproved = true, Category = new Category() { IsDisabled = false } };
            var filter = new List<FilterCriteria>();

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock.Object;

            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.SearchSchema(filter, schemaDetail, "", "")).Returns(new ViewControllerData());
            _datasetDetailService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action
            
            var result = sut.Index("category", schema, 1, 1, "", "", filter, "download", "");
            //assert
            Assert.AreEqual(typeof(CsvResult), result.GetType());
            //cleanup
            _dataSchemaService = null;
            _datasetDetailService = null;


        }

        [TestMethod]
        public void Index_when_schema_is_not_null_and_schema_detail_is_not_null_and_schema_is_online_and_is_external_data_returns_ViewResult_ExternalDataset()
        {
            //arrange
            var schema = "schemanotNull";
            var schemaDetail = new DataSetSchema() { IsDisabled = false, IsApproved = true, IsExternalData = true, Category = new Category() { IsDisabled = false } };
            var filter = new List<FilterCriteria>();

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock.Object;

            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.SearchSchema(filter, schemaDetail, "", "")).Returns(new ViewControllerData());
            _datasetDetailService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (ViewResult)sut.Index("category", schema, 1, 1, "", "", filter, "", "");
            //assert
            Assert.AreEqual("ExternalDataSet", result.ViewName);
            //cleanup
            _dataSchemaService = null;
            _datasetDetailService = null;


        }

        [TestMethod]
        public void Index_when_schema_is_not_null_and_schema_detail_is_not_null_schema_is_online_andNot_external_data_will_return_viewResult_dataset()
        {
            //arrange
            var schema = "schemanotNull";
            var schemaDetail = new DataSetSchema() { IsDisabled = false, IsApproved = true, IsExternalData = false, Category = new Category(){IsDisabled = false}};
            var filter = new List<FilterCriteria>();

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock.Object;

            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.SearchSchema(filter, schemaDetail, "", "")).Returns(new ViewControllerData());
            _datasetDetailService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (ViewResult)sut.Index("category", schema, 1, 1, "", "", filter, "", "");
            //assert
            Assert.AreEqual("DataSet", result.ViewName);
            //cleanup
            _dataSchemaService = null;
            _datasetDetailService = null;


        }


        [TestMethod]
        public void Index_when_schema_is_not_null_and_schema_detail_is_not_null_schema_is_online_Filter_has_value_and_Not_external_data_will_use_Search_schema_to_return_viewControllerData()
        {
            //arrange
            var schema = "schemanotNull";
            var schemaDetail = new DataSetSchema() { IsDisabled = false, IsApproved = true, IsExternalData = false, Category = new Category() { IsDisabled = false } };
            var filter = new List<FilterCriteria>(){new FilterCriteria(){}};

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock.Object;

            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.SearchSchema(filter, schemaDetail, 1, 1, "", "", true)).Returns(new ViewControllerData(){Count = 1234});
            _datasetDetailService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (ViewResult)sut.Index("category", schema, 1, 1, "", "", filter, "", "");
            var model = (ViewControllerData) result.Model;

            //assert
            Assert.AreEqual(1234, model.Count);
            //cleanup
            _dataSchemaService = null;
            _datasetDetailService = null;


        }
        [TestMethod]
        public void Index_when_schema_is_not_null_and_schema_detail_is_not_null_schema_is_online_Filter_has_NO_value_and_Not_external_data_will_use_GetLatest_to_return_viewControllerData()
        {
            //arrange
            var schema = "schemanotNull";
            var schemaDetail = new DataSetSchema() { IsDisabled = false, IsApproved = true, IsExternalData = false, Category = new Category() { IsDisabled = false } };
            var filter = new List<FilterCriteria>() {  };

            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock.Object;

            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.GetLatest(schemaDetail)).Returns(new ViewControllerData() { Count = 4321 });
            _datasetDetailService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (ViewResult)sut.Index("category", schema, 1, 1, "", "", filter, "", "");
            var model = (ViewControllerData)result.Model;

            //assert
            Assert.AreEqual(4321, model.Count);
            //cleanup
            _dataSchemaService = null;
            _datasetDetailService = null;


        }

        [TestMethod]
        public void
            Index_when_schema_is_null_and_schema_detail_is_null_and_category_is_not_null_and_category_is_all_returns_viewREsult_schema
            ()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.GetAll()).Returns(new List<DataSetSchema>());
            _dataSchemaService = mock.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action
            var result = (ViewResult)sut.Index("all", "", 1, 1, "", "", null, "", "");
            //assert
            Assert.AreEqual("Schema", result.ViewName);
            //cleanup
            _dataSchemaService = null;
            

        }

        [TestMethod]
        public void
            Index_when_category_is_not_null_and_category_is_not_all_ANd_categoryDetail_isNotNull_returns_viewREsult_schema
            ()
        {
            //arrange
            var category = "category";
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.GetAll()).Returns(new List<DataSetSchema>());
            _dataSchemaService = mock.Object;
            var mock2 = new Mock<ICategoryService>();
            mock2.Setup(x => x.Get(category)).Returns(new Category());
            _categoryService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action
            var result = (ViewResult)sut.Index(category, "", 1, 1, "", "", null, "", "");
            //assert
            Assert.AreEqual("Schema", result.ViewName);
            //cleanup
            _dataSchemaService = null;
            _categoryService = null;

        }

        [TestMethod]
        public void
            Index_when_category_is_not_null_and_category_is_not_all_ANd_categoryDetail_IS_Null_returns_viewREsult_Error
            ()
        {
            //arrange
            var category = "category";
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.GetAll()).Returns(new List<DataSetSchema>());
            _dataSchemaService = mock.Object;
            var mock2 = new Mock<ICategoryService>();
            _categoryService = mock2.Object;
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action
            var result = (ViewResult)sut.Index(category, "", 1, 1, "", "", null, "", "");
            //assert
            Assert.AreEqual("Error", result.ViewName);
            //cleanup
            _dataSchemaService = null;
            _categoryService = null;

        }

        [TestMethod]
        public void Index_when_schema_is_null_and_schema_detail_is_null_and_category_is_null_return_view_index()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            _dataSchemaService = mock.Object;
            _categoryService = new Mock<ICategoryService>().Object;
            var filter = new List<FilterCriteria>();
            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (ViewResult) sut.Index(null, null, 1, 1, "", "", filter, "", "");
            //assert
            Assert.AreEqual("Index", result.ViewName);
            //cleanup 

        }


        [TestMethod]
        public void
            Ajax_when_getVisualisationData_is_true_and_chart_type_is_map_and_schema_definition_hasLatLngColumns_returns_result_from_visualiseSchemaAsMap
            ()
        {
            //arrange
            string category = "";
            string schema = "";
            int? currentPage = null;
            int? numberToShow = null;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = true;
            string chartType = "map";
            string xAxis = "";
            string yAxis = "";
            int? chartCurrentPage = null;
            int? chartNumberToShow = null;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() { new DataSetSchemaColumn() { ColumnName = "Latitude" }, new DataSetSchemaColumn() { ColumnName = "Longitude" } } } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.VisualiseSchemaAsMap(filter, schemaDetail)).Returns(new ViewControllerData(){DataGraph = "fromVisualiseSchemaAsMap"});
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action
            
            var result = (JsonNetResult)sut.Ajax(category, schema, currentPage, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  chartNumberToShow, yAxisAggregate);
            var unboxedResult = (ViewControllerData) result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromVisualiseSchemaAsMap");
            //cleanup

        }
        [TestMethod]
        public void
            Ajax_when_getVisualisationData_is_true_and_chart_type_is_NOT_map_and_schema_definition_hasLatLngColumns_returns_result_from_visualiseSchema
            ()
        {
            //arrange
            string category = "";
            string schema = "";
            int? currentPage = null;
            int? numberToShow = null;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = true;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int chartCurrentPage = 1;
            int chartNumberToShow = 1;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() { new DataSetSchemaColumn() { ColumnName = "Latitude" }, new DataSetSchemaColumn() { ColumnName = "Longitude" } } } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.VisualiseSchema(filter, schemaDetail, chartCurrentPage, chartNumberToShow, chartType, xAxis, yAxis,yAxisAggregate )).Returns(new ViewControllerData() { DataGraph = "fromVisualiseSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonResult)sut.Ajax(category, schema, currentPage, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  chartNumberToShow, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromVisualiseSchema");
            //cleanup

        }

        [TestMethod]
        public void
            Ajax_when_getVisualisationData_is_true_and_chart_type_is_NOT_map_and_schema_definition_hasLatLngColumns_SETS_chartCurrentPageTo_1_if_null_andReturns_data_from_visualiseSchema
            ()
        {
            //arrange
            string category = "";
            string schema = "";
            int? currentPage = null;
            int? numberToShow = null;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = true;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int? chartCurrentPage = null;
            int chartNumberToShow = 1;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() { new DataSetSchemaColumn() { ColumnName = "Latitude" }, new DataSetSchemaColumn() { ColumnName = "Longitude" } } } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.VisualiseSchema(filter, schemaDetail, 1, chartNumberToShow, chartType, xAxis, yAxis, yAxisAggregate)).Returns(new ViewControllerData() { DataGraph = "fromVisualiseSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonResult)sut.Ajax(category, schema, currentPage, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  chartNumberToShow, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromVisualiseSchema");
            //cleanup

        }
        

        [TestMethod]
        public void
            Ajax_when_getVisualisationData_is_true_and_chart_type_is_NOT_map_and_schema_definition_hasLatLngColumns_SETS_chartNumberToShow_value_from_WebConfig_if_null_andReturns_data_from_visualiseSchema
            ()
        {
            //arrange
            string category = "";
            string schema = "";
            int? currentPage = null;
            int? numberToShow = null;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = true;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int chartCurrentPage = 1;
            int chartNumberToShow = 10;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() { new DataSetSchemaColumn() { ColumnName = "Latitude" }, new DataSetSchemaColumn() { ColumnName = "Longitude" } } } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.VisualiseSchema(filter, schemaDetail, 1, 10, chartType, xAxis, yAxis, yAxisAggregate)).Returns(new ViewControllerData() { DataGraph = "fromVisualiseSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonResult)sut.Ajax(category, schema, currentPage, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  null, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromVisualiseSchema");
            //cleanup

        }


        [TestMethod]
        public void
            Ajax_when_getVisualisationData_is_FALSE_returns_result_from_search_schema()
        {
            //arrange
            string category = "";
            string schema = "";
            int currentPage = 1;
            int numberToShow = 10;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = false;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int chartCurrentPage = 1;
            int chartNumberToShow = 10;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() { new DataSetSchemaColumn() { ColumnName = "Latitude" }, new DataSetSchemaColumn() { ColumnName = "Longitude" } } } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.SearchSchema(filter, schemaDetail, currentPage, numberToShow, orderByColumn, orderByDirection, true)).Returns(new ViewControllerData() { DataGraph = "fromSearchSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonNetResult)sut.Ajax(category, schema, currentPage, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  null, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromSearchSchema");
            //cleanup

        }
        [TestMethod]
        public void
            Ajax_when_columns_has_no_latitudelongitude_and_not_getvisualisationData_returns_result_from_search_schema()
        {
            //arrange
            string category = "";
            string schema = "";
            int currentPage = 1;
            int numberToShow = 10;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = false;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int? chartCurrentPage = null;
            int? chartNumberToShow = null;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.SearchSchema(filter, schemaDetail, currentPage, numberToShow, orderByColumn, orderByDirection, true)).Returns(new ViewControllerData() { DataGraph = "fromSearchSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonNetResult)sut.Ajax(category, schema, currentPage, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  chartNumberToShow, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromSearchSchema");
            //cleanup

        }

        [TestMethod]
        public void
            Ajax_SearchSchema_when_currentPage_is_null_use_1_returns_result_from_search_schema()
        {
            //arrange
            string category = "";
            string schema = "";
            int currentPage = 1;
            int numberToShow = 10;
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = false;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int? chartCurrentPage = null;
            int? chartNumberToShow = null;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.SearchSchema(filter, schemaDetail, currentPage, numberToShow, orderByColumn, orderByDirection, true)).Returns(new ViewControllerData() { DataGraph = "fromSearchSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonNetResult)sut.Ajax(category, schema, null, numberToShow, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  chartNumberToShow, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromSearchSchema");
            //cleanup

        }

        [TestMethod]
        public void
            Ajax_SearchSchema_when_numberToShow_is_null_use_settingsValue_from_config_file_returns_result_from_search_schema()
        {
            //arrange
            string category = "";
            string schema = "";
            int currentPage = 1;
            
            string orderByColumn = "";
            string orderByDirection = "";
            IList<FilterCriteria> filter = new List<FilterCriteria>();
            string download = "";
            bool getVisualisationData = false;
            string chartType = "";
            string xAxis = "";
            string yAxis = "";
            int? chartCurrentPage = null;
            int? chartNumberToShow = null;
            string yAxisAggregate = "";
            DataSetSchema schemaDetail = new DataSetSchema() { Definition = new DataSetSchemaDefinition() { } };
            var mock = new Mock<IDataSetDetailService>();
            mock.Setup(x => x.SearchSchema(filter, schemaDetail, currentPage, 103, orderByColumn, orderByDirection, true)).Returns(new ViewControllerData() { DataGraph = "fromSearchSchema" });
            _datasetDetailService = mock.Object;

            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.Get(schema)).Returns(schemaDetail);
            _dataSchemaService = mock2.Object;

            var sut = new ViewController(_dataSchemaService, _datasetDetailService, _categoryService, _sysConfigService);
            //action

            var result = (JsonNetResult)sut.Ajax(category, schema, currentPage, null, orderByColumn, orderByDirection, filter,
                                  download, getVisualisationData, chartType, xAxis, yAxis, chartCurrentPage,
                                  chartNumberToShow, yAxisAggregate);
            var unboxedResult = (ViewControllerData)result.Data;
            //assert
            Assert.AreEqual(unboxedResult.DataGraph, "fromSearchSchema");
            //cleanup

        }
    }
}
