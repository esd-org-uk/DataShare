using System.Collections.Generic;
using DS.Domain;
using DS.Domain.Interface;
using DS.Domain.WcfRestService;
using DS.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DS.Tests.ServicesTests
{
    /// <summary>
    /// Summary description for DataShareSchemaImportServiceTests
    /// </summary>
    [TestClass]
    public class DataShareSchemaImportServiceTests
    {
        private IDataSetSchemaService _dataSetSchemaService;
        private IXmlToObjectService _xmlToObjectService;
        private IDataSetSchemaColumnService _dataSetSchemaColumnService;

        [TestInitialize]
     public void TestInit()
     {
         
     }
        [TestMethod]
        public void
            ImportFromUrl_when_xml_is_empty_string_returns_importdatasetschemaresult_with_error_message_error_loading_schema_from_url
            ()
        {
         //arrange
            var dschema = new DataSetSchema(){SchemaDefinitionFromUrl = ""};
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("")).Returns("");
            _xmlToObjectService = mock.Object;
            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);
            
            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("Error loading schema from ", result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }


        [TestMethod]
        public void
            ImportFromUrl_when_deserializedschema_is_null_returns_importdatasetschemaresult_with_error_message_no_definition_found_at_url
            ()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            SchemaRestDefinition nullobj = null;
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(nullobj);
            _xmlToObjectService = mock.Object;
            
            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("No definition found at " + dschema.SchemaDefinitionFromUrl, result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }

        [TestMethod]
        public void
            ImportFromUrl_when_deserializedschema_error_message_is_notemptyornull_returns_importdatasetschemaresult_with_error_message_no_definition_found_at_url
            ()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            var obj = new SchemaRestDefinition(){ErrorMessage = "this is not empty"};
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(obj);
            _xmlToObjectService = mock.Object;

            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual(obj.ErrorMessage, result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }

        [TestMethod]
        public void
            ImportFromUrl_when_deserializedschema_has_no_restschema_returns_importdatasetschemaresult_with_error_message_no_schema_found_at_url
            ()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            var obj = new SchemaRestDefinition() { ErrorMessage = "" };
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(obj);
            _xmlToObjectService = mock.Object;

            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("No schema found at " + dschema.SchemaDefinitionFromUrl, result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }

        [TestMethod]
        public void
            ImportFromUrl_when_deserializedschema_has_restschema_and_no_restcolumndefinitions_returns_importdatasetschemaresult_with_error_message_no_schema_found_at_url
            ()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            var obj = new SchemaRestDefinition() { ErrorMessage = "", RestSchema = new RestSchema()};
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(obj);
            _xmlToObjectService = mock.Object;
            _dataSetSchemaService = new Mock<IDataSetSchemaService>().Object;
            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("No schema found at " + dschema.SchemaDefinitionFromUrl, result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }

        [TestMethod]
        public void
            ImportFromUrl_when_imported_ok_returns_importdatasetschemaresult_with_no_error_message()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            var obj = new SchemaRestDefinition() { ErrorMessage = "", RestSchema = new RestSchema(){}, RestColumnDefinitions = new RestColumnDefinitions()};
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(obj);
            _xmlToObjectService = mock.Object;
                _dataSetSchemaService = new Mock<IDataSetSchemaService>().Object;
            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("", result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }


        [TestMethod]
        public void
            ImportFromUrl_when_columnDefinitions_contains_publisherUri_returns_importdatasetschemaresult_with_error_message_with_xml_schema_cannot_contain_reserved_columns()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            var obj = new SchemaRestDefinition()
            {
                ErrorMessage = "",
                RestSchema = new RestSchema() { },
                RestColumnDefinitions = new RestColumnDefinitions()
                    {
                        ColumnDefinitions = new List<RestColumnDefinition>()
                            {
                                new RestColumnDefinition(){Name = "publisherUri"}
                            }
                    }
            };
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(obj);
            _xmlToObjectService = mock.Object;
            _dataSetSchemaService = new Mock<IDataSetSchemaService>().Object;
            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("Error: Columns cannot contain reserved column names - PublisherUri/PublisherLabel/Publisher Uri/Publisher Label. http://dummyurl"
                , result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }

        [TestMethod]
        public void
            ImportFromUrl_when_columnDefinitions_contains_publisherlabel_returns_importdatasetschemaresult_with_error_message_with_xml_schema_cannot_contain_reserved_columns()
        {
            //arrange
            var dschema = new DataSetSchema() { SchemaDefinitionFromUrl = "http://dummyurl" };
            var obj = new SchemaRestDefinition()
            {
                ErrorMessage = "",
                RestSchema = new RestSchema() { },
                RestColumnDefinitions = new RestColumnDefinitions()
                {
                    ColumnDefinitions = new List<RestColumnDefinition>()
                            {
                                new RestColumnDefinition(){Name = "publisherLabel"}
                            }
                }
            };
            var mock = new Mock<IXmlToObjectService>();
            mock.Setup(x => x.GetXmlFromUrl("http://dummyurl")).Returns("<SchemaRestDefinition></SchemaRestDefinition>");
            mock.Setup(x => x.ConvertXml<SchemaRestDefinition>("<SchemaRestDefinition></SchemaRestDefinition>"))
                .Returns(obj);
            _xmlToObjectService = mock.Object;
            _dataSetSchemaService = new Mock<IDataSetSchemaService>().Object;
            var service = new DataShareSchemaImportService(_xmlToObjectService, _dataSetSchemaService,
                                                           _dataSetSchemaColumnService);

            //act
            var result = service.ImportFromUrl(dschema);
            //assert
            Assert.AreEqual("Error: Columns cannot contain reserved column names - PublisherUri/PublisherLabel/Publisher Uri/Publisher Label. http://dummyurl"
                , result.ErrorMessage);
            //cleanup
            _xmlToObjectService = null;
        }
    }
}
