using System.Web.Mvc;
using DS.Domain.Interface;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Controllers
{
    public class AboutController : BaseController
    {

        public AboutController(ISystemConfigurationService systemConfigurationService, ICategoryService categoryService) 
            : base(systemConfigurationService, categoryService)
        {
            
        }

        //
        // GET: /About/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Licence()
        {
            return View("Licence");
        }

        public ActionResult History()
        {
            return View("History");
        }


        public ViewResult Version()
        {
            return View("History");
        }
    }
}
