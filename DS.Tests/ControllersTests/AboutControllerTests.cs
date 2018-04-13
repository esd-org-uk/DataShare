using System;
using System.Web.Mvc;
using DS.Domain.Interface;
using DS.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DS.Tests.ControllersTests
{
    [TestClass]
    public class AboutControllerTests
    {
        private ICategoryService _categoryService;
        private ISystemConfigurationService _systemConfigurationService;


        [TestMethod]
        public void Version_returns_history_view()
        {
            //arrange
            var controller = new AboutController(_systemConfigurationService, _categoryService);
            //action
            var result = (ViewResult)controller.Version();
            //assert
            Assert.AreEqual("History", result.ViewName);

        }
    }
}
