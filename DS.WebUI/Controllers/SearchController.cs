using System.Web.Mvc;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Controllers
{
    public class SearchController : BaseController
    {
        private IDataSetSchemaService _dataSetSchemaService;

        public SearchController(IDataSetSchemaService dataSetSchemaService, ISystemConfigurationService systemConfigurationService, ICategoryService categoryService) 
            : base(systemConfigurationService, categoryService)
        {
            _dataSetSchemaService = dataSetSchemaService;
        }
        [HttpPost]
        public ActionResult Index(string searchtext)
        {
            ViewBag.Title = "Search results - DataShare";
            ViewBag.SearchText = searchtext;
            return View("Search", _dataSetSchemaService.Search(searchtext));
        }

    }
}
