using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Controllers
{
    public class SecurityController : BaseController
    {

        public SecurityController(ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
            : base(systemConfigurationService, categoryService)
        {
            
        }
        [CustomHttps]
        public ActionResult Login()
        {
            return View("Login");
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
            Response.Redirect(String.Format("~/Admin"));
        }
        [CustomHttps]
        public ActionResult Authenticate(string userName, string password, string rememberMe, string returnUrl)
        {
            if (Membership.ValidateUser(userName, password))
            {
                //Administrator or assigned Admin have access
                FormsAuthentication.SetAuthCookie(userName,(rememberMe != null));
                Response.Redirect(returnUrl); 
            }
            else
            {
                ViewBag.ErrorMessage = "Incorrect login details entered.";
                return View("Login");
            }
            return null;
        }

        public void CreateUser(string userName, string emailAddress, string password, string returnUrl)
        {
            try
            {
                // try to create user and then login that user
                FormsAuthentication.SetAuthCookie(userName, true);
                Response.Redirect(returnUrl);
            }
            catch (MembershipCreateUserException e)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(e));
                // something went wrong, pass the message down to the view
                ViewBag.ErrorMessage = e.Message;
            }
        }
    }
}
