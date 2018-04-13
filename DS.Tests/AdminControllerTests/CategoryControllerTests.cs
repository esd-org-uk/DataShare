using System;
using System.Collections;
using System.Collections.Generic;
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
    public class CategoryControllerTests
    {
        private ISystemConfigurationService _systemConfigurationService;
        private ICategoryService _categoryService;

        private Mock<HttpResponseBase> _mockResponseBase;
        private Mock<HttpContextBase> _mockHttpContextBase;
        private Mock<HttpRequestBase> _mockRequestBase;
        //_mockRequestBase.Setup(x => x.IsAuthenticated).Returns(false);
        //_mockHttpContextBase.SetupGet(x => x.Request).Returns(_mockRequestBase.Object);
        //_mockHttpContextBase.SetupGet(x => x.Response).Returns(_mockResponseBase.Object);
        //controller.ControllerContext = new ControllerContext(_mockHttpContextBase.Object, new RouteData(), controller);

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
        public void Index_returns_defaultview()
        {
            //arrange
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Index();
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup
        }
        [TestMethod]
        public void Create_returns_view_defaultview()
        {
            //arrange
            
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Create();
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup
        }

        [TestMethod]
        public void Create_httpPost_when_model_state_is_valid_returns_redirect_to_Index()
        {
            //arrange
            var category = new Category();
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            //act
            var result = (RedirectToRouteResult)controller.Create(category);
            //assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            //cleanup
        }
        [TestMethod]
        public void Create_httpPost_when_model_state_is_invalid_returns_defaultview()
        {
            //arrange
            var category = new Category();
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            controller.ModelState.AddModelError("errorkey", "errorMessage");
            //act
            var result = (ViewResult)controller.Create(category);
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup
        }

        [TestMethod]
        public void Create_httpPost_when_method_throws_exception_returns_defaultview()
        {
            //arrange
            var category = new Category();
            var mock = new Mock<ICategoryService>();
            mock.Setup(x => x.Create(category)).Throws(new Exception());
            _categoryService = mock.Object;
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            
            //act
            var result = (ViewResult)controller.Create(category);
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup
            _categoryService = null;
        }

        [TestMethod]
        public void Edit_httpPost_when_method_throws_exception_returns_defaultview()
        {
            //arrange
            var category = new Category();
            var mock = new Mock<ICategoryService>();
            mock.Setup(x => x.Save(category)).Throws(new Exception());
            _categoryService = mock.Object;
            var controller = new CategoryController(_categoryService, _systemConfigurationService);

            //act
            var result = (ViewResult)controller.Edit(category);
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup
            _categoryService = null;
        }

        [TestMethod]
        public void Edit_httpPost_when_modelstate_is_valid_Returns_defaultview()
        {
            //arrange
            var category = new Category();
            
            var controller = new CategoryController(_categoryService, _systemConfigurationService);

            //act
            var result = (ViewResult)controller.Edit(category);
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup
            
        }
        [TestMethod]
        public void Edit_httpPost_when_modelstate_is_invalid_Returns_defaultview()
        {
            //arrange
            var category = new Category();

            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            controller.ModelState.AddModelError("error", "ErrorMessage");
            //act
            var result = (ViewResult)controller.Edit(category);
            //assert
            Assert.AreEqual("", result.ViewName);
            //cleanup

        }

        [TestMethod]
        public void Delete_Returns_view_index()
        {
            //arrange
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Delete("categoryname");
            //assert
            Assert.AreEqual("Index", result.ViewName);
            //cleanup
        }

        [TestMethod]
        public void Enables_Returns_view_index()
        {
            //arrange
            var controller = new CategoryController(_categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Enable("categoryname");
            //assert
            Assert.AreEqual("Index", result.ViewName);
            //cleanup
        }
    }
}
