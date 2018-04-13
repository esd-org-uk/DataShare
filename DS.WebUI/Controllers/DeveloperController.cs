using System.Web.Mvc;
using DS.Domain.Interface;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Controllers
{
    public class DeveloperController : BaseController
    {

        public DeveloperController(ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
            : base(systemConfigurationService, categoryService)
        {
            
        }
        //
        // GET: /Developer/

        public ActionResult Index()
        {
            return View();
        }

    }
}
