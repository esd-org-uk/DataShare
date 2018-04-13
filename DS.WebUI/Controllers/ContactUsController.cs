using System;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Controllers
{
    public class ContactUsController : BaseController
    {
        private IContactService _contactService;

        public ContactUsController(IContactService contactService, ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
        : base(systemConfigurationService, categoryService)
        {
            _contactService = contactService;
        }
        //
        // GET: /ContactUs/

        public ActionResult Index()
        {
            return View();
        } 

        //
        // POST: /ContactUs/

        [HttpPost]
        public ActionResult Index(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "<p class='warning'>Unable to submit your enquiry.  Please try again.</p>";
                return View("Index");

            }
            try
            {
                _contactService.Create(contact);
                return RedirectToAction("Confirm");
            }
            catch(Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.Message = "<p class='warning'>Unable to submit your enquiry.  Please try again.</p>";
                return View("Index");
            }
        }

        public ActionResult Confirm()
        {
            return View();
        }
        
 
    }
}
