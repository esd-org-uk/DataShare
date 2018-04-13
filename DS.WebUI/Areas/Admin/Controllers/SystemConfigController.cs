using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{
    [CustomHttps]
    [Authorize(Roles = "SuperAdministrator")]
    public class SystemConfigController : BaseController
    {
        private readonly ISystemConfigurationService _systemConfigurationService;

        public SystemConfigController(ISystemConfigurationService systemConfigurationService, ICategoryService categoryService)
            :base(systemConfigurationService, categoryService)
        {
            _systemConfigurationService = systemConfigurationService;
        }

        //
        // GET: /Admin/SystemConfig/
        public ActionResult Index()
        {
            var model = new SystemConfigurationViewModel()
                {
                    ConfigurationObject = _systemConfigurationService.GetSystemConfigurations()
                };
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(SystemConfigurationViewModel postedModel)
        {

            _systemConfigurationService.UpdateSystemConfigurations(postedModel.ConfigurationObject);

            return RedirectToAction("Index", "SystemConfig");
         
        }

    }
}
