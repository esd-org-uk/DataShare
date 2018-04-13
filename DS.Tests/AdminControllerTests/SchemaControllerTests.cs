using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DS.Domain;
using DS.Domain.Interface;
using DS.WebUI.Areas.Admin.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DS.Tests.AdminControllerTests
{
    [TestClass]
    public class SchemaControllerTests
    {
        private IEsdFunctionService _esdFunctionService;
        private IDataShareSchemaImportService _dataShareSchemaImportService;
        private IDataSetSchemaService _datasetSchemaService;
        private IDataSetSchemaColumnService _dataSetSchemaColumnService;
        private ICategoryService _categoryService;
        private ISystemConfigurationService _systemConfigurationService;
        private Mock<HttpResponseBase> _mockResponseBase;
        private Mock<HttpContextBase> _mockHttpContextBase;
        private Mock<HttpRequestBase> _mockRequestBase;
   
        [TestInitialize]
        public void TestInit()
        {

            _mockResponseBase = new Mock<HttpResponseBase>();
            _mockRequestBase = new Mock<HttpRequestBase>();
            _mockHttpContextBase = new Mock<HttpContextBase>();
            var mock2 = new Mock<ICategoryService>();
            _categoryService = mock2.Object;
            var mock3 = new Mock<ISystemConfigurationService>();
            mock3.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject());
            _systemConfigurationService = mock3.Object;
        }

        [TestMethod]
        public void Index_when_category_name_is_not_null_returns_schema_view()
        {
            //arrange
            var controller = new SchemaController(_esdFunctionService, _dataShareSchemaImportService, _datasetSchemaService,
                                                  _dataSetSchemaColumnService, _categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Index("categoryname");
            //assert
            Assert.AreEqual("Schema", result.ViewName);
            //cleanup

        }


        [TestMethod]
        public void Index_when_category_name_is_null_returns_defaultview()
        {
            //arrange
            var controller = new SchemaController(_esdFunctionService, _dataShareSchemaImportService, _datasetSchemaService,
                                                  _dataSetSchemaColumnService, _categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Index(null);
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup

        }



        [TestMethod]
        public void Create_returns_defaultview()
        {
            //arrange
            var controller = new SchemaController(_esdFunctionService, _dataShareSchemaImportService, _datasetSchemaService,
                                                  _dataSetSchemaColumnService, _categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Create("categoryname");
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup

        }


        [TestMethod]
        public void SchemaSource_returns_defaultview()
        {
            //arrange
            var controller = new SchemaController(_esdFunctionService, _dataShareSchemaImportService, _datasetSchemaService,
                                                  _dataSetSchemaColumnService, _categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.SchemaSource("categoryname");
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup

        }

        [TestMethod]
        public void SchemaSource_httpPost_when_request_use_url_not_yes_returns_redirect_to_action_Create()
        {
            //arrange
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("useUrl", "notyest");
            _mockRequestBase.Setup(x => x.Params).Returns(nameValueCollection);
            _mockHttpContextBase.SetupGet(x => x.Request).Returns(_mockRequestBase.Object);
            _mockHttpContextBase.SetupGet(x => x.Response).Returns(_mockResponseBase.Object);
            

            var controller = new SchemaController(_esdFunctionService, _dataShareSchemaImportService, _datasetSchemaService,
                                                  _dataSetSchemaColumnService, _categoryService, _systemConfigurationService);
            controller.ControllerContext = new ControllerContext(_mockHttpContextBase.Object, new RouteData(), controller);   
            var model = new SchemaSourceViewModel();
            //act
            var result = (RedirectToRouteResult)controller.SchemaSource(model, "categoryname");
            //assert
            Assert.AreEqual("Create", result.RouteValues["action"]);
            //cleanup

        }

        /// <summary>
        /// todo: rrecheck this code
        /// </summary>
        [TestMethod]
        public void SchemaSource_httpPost_when_model_schemadefinitionfromurl_is_null_returns_redirect_to_action_Create()
        {
            //arrange
            var nameValueCollection = new NameValueCollection(){};
            nameValueCollection.Add("useUrl", "yes");
            var mockRequestBase = new Mock<HttpRequestBase>();
            //mockRequestBase.Setup(x => x.Params).Returns(nameValueCollection);
            mockRequestBase.SetReturnsDefault(nameValueCollection);
            _mockHttpContextBase.SetupGet(x => x.Request).Returns(mockRequestBase.Object);
            

            var controller = new SchemaController(_esdFunctionService, _dataShareSchemaImportService, _datasetSchemaService,
                                                  _dataSetSchemaColumnService, _categoryService, _systemConfigurationService);
            controller.ControllerContext = new ControllerContext(_mockHttpContextBase.Object, new RouteData(), controller);
            var model = new SchemaSourceViewModel(){SchemaDefinitionFromUrl = null};
            //act
            var result = (RedirectToRouteResult)controller.SchemaSource(model, "categoryname");
            //assert
            Assert.AreEqual("Create", result.RouteValues["action"]);
            //cleanup

        }


   
    }
}
