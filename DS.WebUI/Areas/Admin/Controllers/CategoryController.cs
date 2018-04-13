using System;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{[CustomHttps]
    [Authorize(Roles = "SuperAdministrator")]
    public class CategoryController : BaseController
    {

    public CategoryController(ICategoryService categoryService, ISystemConfigurationService systemConfigurationService)
        : base(systemConfigurationService, categoryService)
    {
        _categoryService = categoryService;
    }

        public ActionResult Index()
        {
            var categories = _categoryService.GetAll(true);
            ViewBag.CreatedCategory = TempData["CreatedCategory"];
            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _categoryService.Create(category);
                    TempData["CreatedCategory"] = category.Title; //Pass created cat to index action
                    return RedirectToAction("Index");
                }
                
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.Message = string.Format("<p class='warning'>Unable to create category.  {0}</p>", ex.Message);
                return View();
            }
        }

        public ActionResult Edit(string categoryName)
        {
            var category = _categoryService.Get(categoryName);
            return View("Edit", category);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _categoryService.Save(category);
                    ViewBag.Message = "<p class='note'>Category successfully saved.</p>";
                    return View(category);
                }
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.Message = string.Format("<p class='warning'>Unable to save category.  {0}</p>", ex.Message);
                return View();
            }
        }

        public ActionResult Delete(string categoryName)
        {
            try
            {
                _categoryService.Delete(categoryName);
                ViewBag.Message = String.Format("<p class='note'>{0} successfully taken offline.</p>", categoryName);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.Message = string.Format("<p class='warning'>Unable to disable category.  {0}</p>", ex.Message);
            }
            var categories = _categoryService.GetAll(true);
            return View("Index", categories);
        }

        public ActionResult Enable(string categoryName)
        {
            try
            {
                _categoryService.Enable(categoryName);
                ViewBag.Message = String.Format("<p class='note'>{0} successfully enabled.</p>", categoryName);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.Message = string.Format("<p class='warning'>Unable to enable category.  {0}</p>", ex.Message);
            }
            var categories = _categoryService.GetAll(true);
            return View("Index", categories);
        }

    }
}
