using System.Web.Mvc;
using DS.Service;

namespace DS.WebUI.Areas.Admin.Controllers
{
    [CustomHttps]
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View("Index");
        }

    }
}
