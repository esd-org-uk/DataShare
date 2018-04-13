using System.Web.Mvc;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        private IDataSetSchemaService _dataSetSchemaService;

        public HomeController(IDataSetSchemaService dataSetSchemaService, ISystemConfigurationService systemConfigurationService, ICategoryService categoryService)
            :base(systemConfigurationService, categoryService)
        {
            _dataSetSchemaService = dataSetSchemaService;
        }


        public ActionResult Index()
        {
            var featured = _dataSetSchemaService.GetFeatured();
            return View("Index", featured);
        }
    }
}
