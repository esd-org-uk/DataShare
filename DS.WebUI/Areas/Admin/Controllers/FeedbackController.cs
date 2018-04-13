using System.Web.Mvc;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{
    [CustomHttps]
    [Authorize(Roles = "SuperAdministrator")]
    public class FeedbackController : BaseController
    {
        private IContactService _contactService;

        public FeedbackController(IContactService contactService, ICategoryService categoryService, ISystemConfigurationService systemConfigurationService) 
            :base(systemConfigurationService, categoryService)

        {
            _contactService = contactService;
        }
        
        //
        // GET: /Admin/Feedback/

        public ActionResult Index()
        {
            return View(_contactService.GetAll());
        }

    }
}
