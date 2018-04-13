using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{[CustomHttps]
     [Authorize(Roles = "Uploader,SchemaEditor,SchemaCreator,SuperAdministrator")]
    public class ChangePasswordController : BaseController
    {
        
        
        #region Constructor

        readonly UserAdminService userAdminService;

        public ChangePasswordController(ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
        : base(systemConfigurationService, categoryService)
        {
            userAdminService = new UserAdminService(); 
        }

        #endregion 


        public ActionResult Index()
        {
            ViewBag.UserName = Membership.GetUser().UserName;
            return View("ChangePassword");
        }

        [HttpPost]
        public ActionResult Index(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = userAdminService.ChangePassword(model.UserName, model.OldPassword, model.NewPassword);
                ViewBag.Message = result ? "<p class='note'>Password updated successfully.</p>" : "<p class='warning'>Password not changed.  Current password not correct.</p>";
                ViewBag.UserName = model.UserName;
                return View("ChangePassword",model);
            }
            ViewBag.Message = "<p class='warning'>Unable to change password. Check the passwords match.</p>";
            ViewBag.UserName = model.UserName;
            return View("ChangePassword",model);
        }

    }
}
