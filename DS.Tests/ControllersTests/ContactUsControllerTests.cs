using System;
using System.Web.Mvc;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using DS.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests.ControllersTests
{
    [TestClass]
    public class ContactUsControllerTests
    {
        private IContactService _contactService;
        private ICategoryService _categoryService;
        private ISystemConfigurationService _systemConfigurationService;

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
      
            var mock2 = new Mock<ICategoryService>();
            _categoryService = mock2.Object;
            var mock3 = new Mock<ISystemConfigurationService>();
            mock3.Setup(x=>x.GetSystemConfigurations()).Returns(new SystemConfigurationObject());
            _systemConfigurationService = mock3.Object;

        }

        [TestMethod]
        public void Index_when_model_state_is_valid_redirect_to_action_confirm()
        {
            //arrange
            var mock = new Mock<IContactService>();
            _contactService = mock.Object;
            var contact = new Contact();
            var controller = new ContactUsController(_contactService, _categoryService, _systemConfigurationService);
            controller.ModelState.Clear();
            //act
            var result = (RedirectToRouteResult)controller.Index(contact);
            //assert
            Assert.AreEqual("Confirm", result.RouteValues["action"]);
            //cleanup
            _contactService = null;

        }


        [TestMethod]
        public void Index_when_model_state_is_invalid_redirect_to_action_index()
        {
            //arrange
            var mock = new Mock<IContactService>();
            _contactService = mock.Object;
            var contact = new Contact();
            var controller = new ContactUsController(_contactService, _categoryService, _systemConfigurationService);
            controller.ModelState.AddModelError("test", "error");
            //act
            var result = (ViewResult)controller.Index(contact);
            //assert
            Assert.AreEqual("Index", result.ViewName);
            //cleanup
            _contactService = null;
        }

        [TestMethod]
        public void Index_when_saving_feedback_throws_exception_action_index()
        {
            //arrange
            var contact = new Contact();
            var controller = new ContactUsController(_contactService, _categoryService, _systemConfigurationService);
            //act
            var result = (ViewResult)controller.Index(contact);
            //assert
            Assert.AreEqual("Index", result.ViewName);
            //cleanup

        }



        [TestCleanup]
        public void TestCleanup()
        {
            _categoryService = null;
            _contactService = null;
            _systemConfigurationService = null;
        }
    }
}
