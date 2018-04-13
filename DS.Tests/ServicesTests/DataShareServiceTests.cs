using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Service.WcfRestService;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class DataShareServiceTests
    {
        private IDataSetSchemaService _datasetschemaservice;
        private IFileSystem _mockFileSystem;
        private IEsdFunctionService _esdFunctionService;
        private MemoryRepository<DataSetSchema> _repositoryDataSetSchema;
        private IDataSetDetailService _datasetdetailservice;

        [TestInitialize]
        public void TestInit()
        {


            _repositoryDataSetSchema = new MemoryRepository<DataSetSchema>();
            _datasetschemaservice = GetMockDataSetSchemaService();
            _esdFunctionService = GetMockEsdFunctionService();
            _mockFileSystem = GetMockFileSystem();
            _datasetdetailservice = Getdatasetdetailservice();
            
            ObjectFactory.Initialize(
            x =>
            {
                x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();
                x.For<IDataSetSchemaService>().Use(_datasetschemaservice);
                x.For<IEsdFunctionService>().Use(_esdFunctionService);
                x.For<IFileSystem>().Use(_mockFileSystem);
                x.For<IDataTableSerializer>().Use<CustomDataTableToJsonSerializer>().Named("json");
                x.For<IDataTableSerializer>().Use<CustomDataTableToXmlSerializer>().Named("xml");
                x.For<IDataSetDetailService>().Use(_datasetdetailservice);
            }

         );
 
        }
        #region Mock Services Setup
        private IDataSetSchemaService GetMockDataSetSchemaService()
        {
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Repository).Returns(_repositoryDataSetSchema);

            DataSetSchema nullObj = null;
            var dsObj = new DataSetSchema() { IsDisabled = true };

            //GetSchemaDefinitionv2_when_schema_is_null_returns_Schema_rest_definition_Error_message_this_schema_does_not_exist
            //GetSchemaDefinitionv2_when_schema_is_null_returns_Schema_rest_definition_with_esdlinks_as_null
            mock.Setup(x => x.Get("categoryurl", "schemaurl")).Returns(nullObj);
            //GetSchemaDefinitionv2_when_schema_is_disabled_returns_Schema_rest_definition_Error_message_this_schema_has_been_disabled
            //GetSchemaDefinitionv2_when_schema_is_disabled_returns_Schema_rest_definition_with_esdlinks_as_null
            mock.Setup(x => x.Get("categoryurl", "schemaurl_disabled_true")).Returns(dsObj);
            
            var dsObj2 = new DataSetSchema() { IsDisabled = false, IsApproved = true, Category = new Category() { IsDisabled = false } };
            mock.Setup(x => x.Get("categoryurl", "schemaurl_disabled_false_online_true")).Returns(dsObj2);
            //schemaurl_disabled_false_online_true_with_columns
            var dsObj3 = new DataSetSchema()
            {
                IsDisabled = false,
                IsApproved = true,
                Category = new Category() { IsDisabled = false },
                Definition = new DataSetSchemaDefinition()
                {
                    Columns = new List<DataSetSchemaColumn>()
                        {
                            new DataSetSchemaColumn(){MaxSize = 30, Type="Text"},
                            new DataSetSchemaColumn(){MinDate = new DateTime(1900,1,2), Type = "DateTime", MaxDate = new DateTime(2200,1,2)},
                            new DataSetSchemaColumn(){MinCurrency = 30, Type = "Currency", MaxCurrency = 100},
                            new DataSetSchemaColumn(){MinNumber = 1, Type = "Number", MaxNumber = 100}
                        }
                }
            };
            mock.Setup(x => x.Get("categoryurl", "schemaurl_return_dsObj3")).Returns(dsObj3);
            var dsObj4 = new DataSetSchema()
            {
                IsDisabled = false,
                IsApproved = true,
                Category = new Category() { IsDisabled = false },
                Definition = new DataSetSchemaDefinition()
                {
                    Columns = new List<DataSetSchemaColumn>()
                        {
                            new DataSetSchemaColumn(){MaxSize = 0, },
                            new DataSetSchemaColumn(){Type = "DateTime"},
                            new DataSetSchemaColumn(){Type = "Currency"},
                            new DataSetSchemaColumn(){Type = "Number", LinkedDataUri = null},
                        }
                }
            };
            mock.Setup(x => x.Get("categoryurl", "schemaurl_return_dsObj4")).Returns(dsObj4);

            return mock.Object;
        }

        private IFileSystem GetMockFileSystem()
        {
            var mock = new Mock<IFileSystem>();
            return mock.Object;
        }
        private IEsdFunctionService GetMockEsdFunctionService()
        {
            var emptyObj = new List<EsdFunctionServiceEntity>();
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(0)).Returns(emptyObj);
            return mock.Object;
        }

        private IDataSetDetailService Getdatasetdetailservice()
        {
                        


            
            var mock = new Mock<IDataSetDetailService>();
            var filter = new FilterCriteria();
            var ds = new DataSetSchema();
            DataTable dt = new DataTable("table10000");
            mock.Setup(
                x =>
                x.SearchSchema(new List<FilterCriteria>() {filter}, ds, -1, -1, filter.ColumnToSearch, "ASC", false))
                .Returns(new ViewControllerData() {Data = dt});
            mock.SetReturnsDefault(new ViewControllerData(){Data = new DataTable("table")});
            return mock.Object;
        }
        #endregion


 
        [TestMethod]
        public void GetSchemaDefinitionv2_when_schema_is_null_returns_Schema_rest_definition_Error_message_this_schema_does_not_exist()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl");
            //assert
            Assert.AreEqual("This schema does not exist!", result.ErrorMessage);
            //cleanup
            

        }

        [TestMethod]
        public void GetSchemaDefinitionv2_when_schema_is_null_returns_Schema_rest_definition_with_esdlinks_as_null()
        {
            //arrange
            
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl");
            //assert
            Assert.IsNull(result.EsdLinks);
            //cleanup
            

        }

        [TestMethod]
        public void GetSchemaDefinitionv2_when_schema_is_disabled_returns_Schema_rest_definition_Error_message_this_schema_has_been_disabled()
        {
            //arrange

            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_disabled_true");
            //assert
            Assert.AreEqual("This schema has been disabled", result.ErrorMessage);
            //cleanup
            

        }

        [TestMethod]
        public void GetSchemaDefinitionv2_when_schema_is_disabled_returns_Schema_rest_definition_with_esdlinks_as_null()
        {
            //arrange

            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_disabled_true");
            //assert
            Assert.IsNull(result.EsdLinks);
            //cleanup


        }

        [TestMethod]
        public void GetSchemaDefinitionv2_when_schema_is_not_disabled_and_isOnline_and_Schema_DataSetSchemaDefinition_is_null_returns_schemarestdefinitionobject_type()
        {
            //arrange

            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_disabled_false_online_true");
            //assert
            Assert.AreEqual(typeof(SchemaRestDefinition), result.GetType());
            //cleanup
            

        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_type_is_text_returns_col_maxsize_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual("30", result.RestColumnDefinitions.ColumnDefinitions[0].MaxSize);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxsize_less_than_zero_returns_col_n_a_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[0].MaxSize);
        }



        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_minDate_is_datetime_type_and_mindate_is_not_null_returns_col_mindate_to_string_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual(new DateTime(1900,1,2).ToString(), result.RestColumnDefinitions.ColumnDefinitions[1].MinDate);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_minDate_is_datetime_type_and_mindate_isnull_returns_col_na_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[1].MinDate);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxDate_is_datetime_type_and_maxdate_is_not_null_returns_col_maxdate_to_string_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual(new DateTime(2200, 1, 2).ToString(), result.RestColumnDefinitions.ColumnDefinitions[1].MaxDate);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxDate_is_datetime_type_and_maxdate_isnull_returns_col_na_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[1].MaxDate);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_minCurrency_is_currency_type_and_minCurrency_is_not_null_returns_col_minCurrency_to_string_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual("30", result.RestColumnDefinitions.ColumnDefinitions[2].MinCurrency);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_minCurrency_is_currency_type_and_minCurrency_is_null_returns_col_na_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[2].MinCurrency);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxCurrency_is_currency_type_and_maxCurrency_is_not_null_returns_col_maxCurrency_to_string_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual("100", result.RestColumnDefinitions.ColumnDefinitions[2].MaxCurrency);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxCurrency_is_currency_type_and_maxCurrency_is_null_returns_col_na_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[2].MaxCurrency);
        }



        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_minNumber_is_Number_type_and_minNumber_is_not_null_returns_col_minNumber_to_string_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual("1", result.RestColumnDefinitions.ColumnDefinitions[3].MinNumber);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_minNumber_is_number_type_and_minNumber_is_null_returns_col_na_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[3].MinNumber);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxNumber_is_Number_type_and_maxNumber_is_not_null_returns_col_maxNumber_to_string_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj3");
            //Assert
            Assert.AreEqual("100", result.RestColumnDefinitions.ColumnDefinitions[3].MaxNumber);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_is_not_null_and_maxNumber_is_number_type_and_maxNumber_is_null_returns_col_na_in_restcolumndefinition
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("N/A", result.RestColumnDefinitions.ColumnDefinitions[3].MaxNumber);
        }


        [TestMethod]
        public void
            GetSchemaDefinitionV2_when_Schema_definitioncolumn_linkeduri_is_null_sets_Uri_to_empty_string
            ()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var result = mut.GetSchemaDefinitionV2("categoryurl", "schemaurl_return_dsObj4");
            //Assert
            Assert.AreEqual("", result.RestColumnDefinitions.ColumnDefinitions[3].Uri);
            
        }
     
        [TestMethod]
        public void
            SearchSchemaDate_when_from_is_not_valid_format_dd_mm_yyyy_date_returns_stream_with_invalid_date_format()
        {
            //arrange
            var mut = new DataShareService();

            //act
            var stream = mut.SearchSchemaDate("", "schemaurl", "xml", "searchField", "12200999", "12-12-1220");
             
            var sr = new StreamReader(stream);
                var myStr = sr.ReadToEnd();

            var result = myStr.Contains("Invalid Date Format");
            //assert
            Assert.IsTrue(result);

        }

        [TestMethod]
        public void
            SearchSchemaDate_when_to_is_not_valid_format_dd_mm_yyyy_date_returns_stream_with_invalid_date_format()
        {
            //arrange
            var mut = new DataShareService();

            //act
            var stream = mut.SearchSchemaDate("", "schemaurl", "xml", "searchField", "12-12-1220", "1212-1220");

            var sr = new StreamReader(stream);
            var myStr = sr.ReadToEnd();

            var result = myStr.Contains("Invalid Date Format");
            //assert
            Assert.IsTrue(result);

        }
        
        /// <summary>
        /// a test to use generic searchschema
        /// </summary>
        [TestMethod]
        public void
            SearchSchema_when_schema_is_null_returns_stream_with_the_data_is_not_available()
        {
            //arrange
            var mut = new DataShareService();
            //act
            var stream = mut.SearchSchemaTextContains("", "schemaurl", "xml", "searchField", "searchtext");
            var sr = new StreamReader(stream);
            var myStr = sr.ReadToEnd();
            var result = myStr.Contains("This data is not available");
            //assert
            Assert.IsTrue(result);

        }

        /// <summary>
        /// a test to use generic searchschema
        /// </summary>
        [TestMethod]
        public void
            SearchSchema_when_schema_is_not_online_returns_stream_with_the_data_is_not_available()
        {
            //arrange
            var dsObj2 = new DataSetSchema() { Title = "schemaurl-disabled-true", IsDisabled = true, IsApproved = true, Category = new Category() { IsDisabled = false } };
            _repositoryDataSetSchema.Add(dsObj2);
            var mut = new DataShareService();

            //act
            var stream = mut.SearchSchemaTextContains("", "schemaurl-disabled-true", "xml", "searchField", "searchtext");
            var sr = new StreamReader(stream);
            var myStr = sr.ReadToEnd();
            var result = myStr.Contains("This data is not available");
            //assert
            Assert.IsTrue(result);
            //cleanup
            _repositoryDataSetSchema.Delete(dsObj2);
        }
        /// <summary>
        /// a test to use generic searchschema
        /// </summary>
        [TestMethod]
        public void
            SearchSchema_when_result_count_is_0_returns_stream_with_the_no_matching_records_found()
        {
            //arrange
            var dsObj2 = new DataSetSchema() { Title = "schemaurl", IsDisabled = false, IsApproved = true, Category = new Category() { IsDisabled = false } };
            _repositoryDataSetSchema.Add(dsObj2);
            var mut = new DataShareService();

            //act
            var stream = mut.SearchSchemaTextContains("", "schemaurl", "xml", "searchField", "searchtext");
            var sr = new StreamReader(stream);
            var myStr = sr.ReadToEnd();
            var result = myStr.Contains("No matching records found");
            //assert
            Assert.IsTrue(result);
            //cleanup
            _repositoryDataSetSchema.Delete(dsObj2);
        }

        //todo - finish this test
        ///// <summary>
        ///// a test to use generic searchschema
        ///// </summary>
        //[TestMethod]
        //public void
        //    SearchSchema_when_result_count_is_more_than_10000_returns_stream_with_The_maximum_the_api_allows_is_10000()
        //{
        //    //arrange
        //    var dsObj2 = new DataSetSchema() { Title = "schemaurl", IsDisabled = false, IsApproved = true, Category = new Category() { IsDisabled = false } };
        //    _repositoryDataSetSchema.Add(dsObj2);
        //    var mock = new Mock<IDataSetDetailService>();
        //    mock.SetReturnsDefault(new ViewControllerData(){Data = new DataTable("table10000")});
        //    _datasetdetailservice = mock.Object;

        //    var mut = new DataShareService();

        //    //act
        //    var stream = mut.SearchSchemaTextContains("", "schemaurl", "xml", "searchField", "searchtext");
        //    var sr = new StreamReader(stream);
        //    var myStr = sr.ReadToEnd();
        //    var result = myStr.Contains("The maximum the api allows is 10000");
        //    //assert
        //    Assert.IsTrue(result);
        //    //cleanup
            
        //    _repositoryDataSetSchema.Delete(dsObj2);
        //}
        [TestCleanup]
        public void TestCleanup()
        {
            _datasetschemaservice = null;
            _esdFunctionService = null;
            _mockFileSystem = null;
        }
    }
}
