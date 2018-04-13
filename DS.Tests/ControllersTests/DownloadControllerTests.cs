using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using DS.Domain;
using DS.Domain.Interface;
using DS.WebUI.Controllers;
using DS.WebUI.Controllers.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DS.Tests.ControllersTests
{
    [TestClass]
    public class DownloadControllerTests
    {
        private IDataSetSchemaService _datasetschemaservice;
        private IDataSetDetailService _datasetdetailservice;
        private ICategoryService _categoryservice;
        private ISystemConfigurationService _systemConfigurationService;

        //private Mock<HttpRequestBase> _mockRequestBase;
        private Mock<HttpResponseBase> _mockResponseBase;
        private Mock<HttpContextBase> _mockHttpContextBase;

        [TestInitialize]
        public void TestInit()
        {
            // ObjectFactory.Initialize(
            //x =>
            //{
            //    x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();
            //    x.For(typeof(IRepository<>)).Use(typeof(MemoryRepository<>));
            //    x.For<ICategoryService>().Use<CategoryService>();
            //    x.For<ISystemConfigurationService>().Use<SystemConfigurationService>();
            //    x.For<>()

            //}

            //);

            //var mock2 = new Mock<ICategoryService>();
            //_categoryService = mock2.Object;
            var mock3 = new Mock<ISystemConfigurationService>();
            mock3.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject());
            _systemConfigurationService = mock3.Object;

            _mockResponseBase = new Mock<HttpResponseBase>();
            _mockHttpContextBase = new Mock<HttpContextBase>();
            
            

        }
        
        //private ControllerContext MockControllerContext(Mock<HttpRequestBase> request, Controller controller)
        //{
        //    //var request = new Mock<HttpRequestBase>();
        //    //request.Setup(x => x.IsAuthenticated).Returns(false);

        //    var context = new Mock<HttpContextBase>();
        //    context.SetupGet(x => x.Request).Returns(request.Object);
        //    return new ControllerContext(context.Object, new RouteData(), controller);
        //}
        [TestMethod]
        public void LoadList_When_schema_is_not_null_and_schemadetail_is_null_returns_view_error()
        {

            //arrange
            DataSetSchema nullobj = null;
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get("schema")).Returns(nullobj);
            _datasetschemaservice = mock.Object;

            _mockHttpContextBase.SetupGet(x => x.Response).Returns(_mockResponseBase.Object);
            var controller = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);
            controller.ControllerContext = new ControllerContext(_mockHttpContextBase.Object, new RouteData(), controller);

            //act
            var result = (ViewResult)controller.Index(null, "schema", null, null, null, null, null, null);
            //assert
            Assert.AreEqual("Error", result.ViewName);
            //cleanup
        }

        [TestMethod]
        public void Index_Download_when_all_parameters_are_null_will_return_viewresult()
        {
            //arrange
            var mock = new Mock<ICategoryService>();
            _categoryservice = mock.Object;
            
            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);

            // Act
            var result = customersController.Index(null, null, null, null, null, null, null, null);// as ViewResult;

            // Assert
            Assert.AreEqual(typeof(ViewResult), result.GetType(), "Should have returned a ViewResult");
            //Assert.AreEqual("Index", result.ViewName, "View name should have been Index");

            //var data = (IList<Category>)result.Model;
            //Assert.IsTrue(data.Count > 0, "Shuuld have returned a list of categories");
        }


        [TestMethod]
        public void Index_Download_when_all_parameters_are_null_will_return_view_name_of_index()
        {
            //arrange
            var mock = new Mock<ICategoryService>();
            _categoryservice = mock.Object;

            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);

            // Act
            var result = customersController.Index(null, null, null, null, null, null, null, null) as ViewResult;

            // Assert
            
            Assert.AreEqual("Index", result.ViewName, "View name should have been Index");

            //var data = (IList<Category>)result.Model;
            //Assert.IsTrue(data.Count > 0, "Shuuld have returned a list of categories");
        }
        [TestMethod]
        public void Index_Download_when_all_parameters_are_null_will_return_list_category_as_view_model()
        {
            //arrange
            var returnItem = new List<Category>(){new Category()};
            var mock = new Mock<ICategoryService>();
            mock.Setup(x => x.GetAll(false)).Returns(returnItem);
            _categoryservice = mock.Object;

            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);

            // Act
            var result = customersController.Index(null, null, null, null, null, null, null, null) as ViewResult;
            var data = (List<Category>)result.Model;
            // Assert
            
            //Assert.AreEqual(typeof(List<Category>), result.Model);
            Assert.IsTrue(data.Count > 0, "Shuuld have returned a list of categories");
        }


        [TestMethod]
        public void Index_Download_when_only_category_is_given_will_return_a_schema_viewname()
        {
            //arrange
            var returnItem = new Category();
            var mock = new Mock<ICategoryService>();
            mock.Setup(x => x.Get("friendlyurl")).Returns(returnItem);
            _categoryservice = mock.Object;
            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);


            // Act
            var result = customersController.Index("friendlyurl", null, null, null, null, null, null, null) as ViewResult;

            // Assert
            
            Assert.AreEqual("Schema", result.ViewName, "View name should have been Schema");

            //var data = (IList<DataSetSchema>)result.Model;
            //Assert.IsTrue(data != null && data.Count > 0, "Shuuld have returned a list of categories");
        }


        [TestMethod]
        public void Index_Download_when_only_category_is_given_will_return_viewmodel_as_list_of_datasetschema_more_than_zero()
        {
            //arrange
            var returnItem = new Category();
            var mock = new Mock<ICategoryService>();
            mock.Setup(x => x.Get("friendlyurl")).Returns(returnItem);
            mock.Setup(x => x.GetByFriendlyUrl("friendlyurl", false)).Returns(new List<DataSetSchema>(){new DataSetSchema()});
            _categoryservice = mock.Object;
            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);


            // Act
            var result = customersController.Index("friendlyurl", null, null, null, null, null, null, null) as ViewResult;
            var data = (IList<DataSetSchema>)result.Model;
            // Assert
            Assert.IsTrue(data.Count > 0, "Shuuld have returned a list of categories");
        }

        [TestMethod]
        public void Index_Download_when_categoryurl_and_schema_url_is_given_will_return_a_viewname_of_DataSet()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get("schemafriendlyurl")).Returns(new DataSetSchema());
            _datasetschemaservice = mock.Object;

            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);
            
            // Act
            var result = customersController.Index("categoryfriendlyurl", "schemafriendlyurl", null, null, null, null, null, null) as ViewResult;

            // Assert
            
            Assert.AreEqual("DataSet", result.ViewName, "View name should have been DataSet");

            //var data = (IList<DataSetDetail>)result.Model;
            //Assert.IsTrue(data != null && data.Count > 0, "Shuuld have returned a list of DataSets");
        }

        [TestMethod]
        public void Index_Download_when_categoryurl_and_schema_url_is_given_will_return_a_list_of_datasetdetails_more_than_zero()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get("schemafriendlyurl")).Returns(new DataSetSchema());
            mock.Setup(x => x.GetByFriendlyUrl("categoryfriendlyurl", "schemafriendlyurl", false)).Returns(new List<DataSetDetail>(){new DataSetDetail()});
            _datasetschemaservice = mock.Object;

            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);

            // Act
            var result = customersController.Index("categoryfriendlyurl", "schemafriendlyurl", null, null, null, null, null, null) as ViewResult;
            var data = (IList<DataSetDetail>)result.Model;

            // Assert

            Assert.IsTrue(data.Count > 0, "Shuuld have returned a list of DataSets");
        }


        [TestMethod]
        public void Index_when_category_url_schema_url_filetitle_is_given_and_format_is_xml_returns_XmlResulttype()
        {
            
            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get("schemaurl")).Returns(new DataSetSchema());
            _datasetschemaservice = mock.Object;
            var dt = new DataTable("tablename");
            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.GetData("dataseturl", "schemaurl")).Returns(dt);
            _datasetdetailservice = mock2.Object;

            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);

            // Act
            var result = customersController.Index("categoryurl", "schemaurl", "dataseturl", "xml", null, null, null, null);

            // Assert
            Assert.AreEqual(typeof(XmlResult), result.GetType(), "Should have returned an XmlResult");
            //Assert.IsNotNull(result, "Should have returned an XmlResult");
            //Assert.IsTrue(((XmlDocument)result.ObjectToSerialize).HasChildNodes, "Xml document was empty");
        }

        [TestMethod]
        public void Index_when_category_url_schema_url_filetitle_is_given_and_format_is_xml_returns_XmlDocument_that_is_not_empty()
        {

            //arrange
            var mock = new Mock<IDataSetSchemaService>();
            mock.Setup(x => x.Get("schemaurl")).Returns(new DataSetSchema());
            _datasetschemaservice = mock.Object;
            var dt = new DataTable("tablename");
            dt.Columns.Add(new DataColumn("test"));
            var row = dt.NewRow();
            row["test"] = "testvalue";
            dt.Rows.Add(row);
            var mock2 = new Mock<IDataSetDetailService>();
            mock2.Setup(x => x.GetData("dataseturl", "schemaurl")).Returns(dt);
            _datasetdetailservice = mock2.Object;

            var customersController = new DownloadController(_datasetschemaservice, _datasetdetailservice, _categoryservice, _systemConfigurationService);

            // Act
            var result = customersController.Index("categoryurl", "schemaurl", "dataseturl", "xml", null, null, null, null) as XmlResult;

            // Assert
            Assert.IsTrue(((XmlDocument)result.ObjectToSerialize).HasChildNodes, "Xml document was empty");
        }



     

    }
}
