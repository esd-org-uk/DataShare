using System;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{[CustomHttps]
    [Authorize(Roles = "SuperAdministrator")]
    public class DebugInfoController : BaseController
    {
    private IDebugInfoService _debugInfoService;

    public DebugInfoController(IDebugInfoService debugInfoService, ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
        : base(systemConfigurationService, categoryService)
    {
        _debugInfoService = debugInfoService;
    }
        public ActionResult Index()
        {
            var debugInfo = _debugInfoService.GetAll();
            return View("Index", debugInfo);
        }

        public ActionResult Filter(string showtype)
        {
            var debugInfo = _debugInfoService.Get((DebugInfoTypeEnum)Enum.Parse(typeof(DebugInfoTypeEnum), showtype));
            return View("Index", debugInfo);
        }

    }
}
