using System;
using System.Web.Mvc;
using System.Web.Security;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{
    [CustomHttps]
    [Authorize(Roles = "SuperAdministrator")]
    public class UserAdminController : BaseController
    {

        #region Constructor

        readonly UserAdminService userAdminService;

        public UserAdminController(ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
            : base(systemConfigurationService, categoryService)
        {
            userAdminService = new UserAdminService(); 
        }

        #endregion 
        
        public ActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            var users = userAdminService.GetUsers();
            return View("Index", users);
        }

        #region Users
        public ActionResult CreateUser()
        {
            return View("User");
        }

        [HttpPost]
        public ActionResult CreateUser(RegisterModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var status = userAdminService.CreateUser(user.UserName, user.Password, user.Email, user.Role);
                    if (status != MembershipCreateStatus.Success)
                    {
                        ViewBag.Message = string.Format("<p class='warning'>{0}<p>", AccountValidation.ErrorCodeToString(status));
                        return View("User");
                    }
                    TempData["Message"] = string.Format("<p class='note'>User '{0}' successfully created.<p>", user.UserName);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    ViewBag.Message = string.Format("<p class='warning'>There was a problem creating this user. {0}<p>", ex.Message);
                    return View("User");
                }

            }
            ViewBag.Message = "<p class='warning'>Unable to create user.<p>";
            return View("User");
        }

        public ActionResult EditUser(string userName)
        {
            var user = userAdminService.GetUserForEdit(userName);
            return View("EditUser", user);
        }

        [HttpPost]
        public ActionResult EditUser(EditUserModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    userAdminService.UpdateUser(user);
                    TempData["Message"] = string.Format("<p class='note'>Changes to user '{0}' successfully saved.<p>", user.UserName);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    ViewBag.Message = string.Format("<p class='warning'>There was a problem with updating this user. {0}<p>", ex.Message);
                    return View("User");
                }
            }
            ViewBag.Message = "<p class='warning'>There was a problem with updating this user.<p>";
            return View("User");
        }

        public ActionResult DeleteUser(string userName)
        {
            try
            {
                userAdminService.DeleteUser(userName);
                TempData["Message"] = string.Format("<p class='note'>User '{0}' successfully deleted.<p>", userName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                TempData["Message"] = string.Format("<p class='warning'>There was a problem deleting user '{0}'. {1}<p>", userName, ex.Message);
                return RedirectToAction("Index");
            }
        }
        
        #endregion

    }
}
