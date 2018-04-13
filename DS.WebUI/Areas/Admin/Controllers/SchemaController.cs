using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WebUI.Controllers.Base;

namespace DS.WebUI.Areas.Admin.Controllers
{[CustomHttps]
    [Authorize(Roles = "SchemaEditor,SchemaCreator,SuperAdministrator")]
    public class SchemaController : BaseController
    {
    private IEsdFunctionService _esdFunctionService;
    private IDataShareSchemaImportService _dataShareSchemaImportService;
    private IDataSetSchemaService _dataSetSchemaService;
    private IDataSetSchemaColumnService _dataSetSchemaColumnService;

    public SchemaController(IEsdFunctionService esdFunctionService, 
        IDataShareSchemaImportService dataShareSchemaImportService, 
        IDataSetSchemaService dataSetSchemaService,
        IDataSetSchemaColumnService dataSetSchemaColumnService,
        ICategoryService categoryService, ISystemConfigurationService systemConfigurationService) 
        : base(systemConfigurationService, categoryService)
    {
        _esdFunctionService = esdFunctionService;
        _dataShareSchemaImportService = dataShareSchemaImportService;
        _dataSetSchemaService = dataSetSchemaService;
        _dataSetSchemaColumnService = dataSetSchemaColumnService;
        _categoryService = categoryService;
    }
        
        public ActionResult Index(string categoryName)
        {
            ViewBag.CategoryName = categoryName;
            return categoryName != null ? View("Schema", _categoryService.GetByFriendlyUrl(categoryName, true))
                                    : View(_categoryService.GetAll(true));
        }

        [Authorize(Roles = "SchemaCreator,SuperAdministrator")]
        public ActionResult Create(string categoryName)
        {
            ViewBag.CategoryName = categoryName;
            return View();
        }
        [Authorize(Roles = "SchemaCreator,SuperAdministrator")]
        public ActionResult SchemaSource(string categoryName)
        {
            ViewBag.CategoryName = categoryName;

            if (TempData["SchemaSourceErrorMessage"] != null)
                ViewBag.Message = string.Format("<p class='note'>{0}.</p>", Convert.ToString(TempData["SchemaSourceErrorMessage"]));
            
            TempData["SchemaSourceErrorMessage"] = null;

            return View(new SchemaSourceViewModel(){CategoryName = categoryName});
        }

        [HttpPost]
        [Authorize(Roles = "SchemaCreator,SuperAdministrator")]
        [ValidateInput(false)]
        public ActionResult SchemaSource(SchemaSourceViewModel model, string categoryName)
        {
            if (Request["useUrl"] != "yes")
                return RedirectToAction("Create");
            
            if (String.IsNullOrEmpty(model.SchemaDefinitionFromUrl))
                return RedirectToAction("Create");
            
            if (!ModelState.IsValid)
                return RedirectToAction("SchemaSource");
            
            var schema = new DataSetSchema()
                {
                    Category = _categoryService.Get(categoryName)
                    ,IsStandardisedSchemaUrl =  model.IsStandardisedSchemaUrl
                    ,SchemaDefinitionFromUrl = model.SchemaDefinitionFromUrl
                };

            var resultImport = _dataShareSchemaImportService.ImportFromUrl(schema);

            TempData["SchemaSourceErrorMessage"] = resultImport.ErrorMessage;
            if(!String.IsNullOrEmpty(resultImport.ErrorMessage))
                return RedirectToAction("SchemaSource");
            
            TempData["SchemaSourceErrorMessage"] = null;
                
            schema = resultImport.DataSetSchema;
            //save esd links
            SaveEsdLinks(schema);
            return RedirectToAction("Edit",  new {categoryName = schema.Category.FriendlyUrl,schemaName  = schema.FriendlyUrl} );
            
        }

        [HttpPost]
        [Authorize(Roles = "SchemaCreator,SuperAdministrator")]
        [ValidateInput(false)]
        public ActionResult Create(DataSetSchema schema, string categoryName)
        {
            try
            {
                

                if (ModelState.IsValid)
                {
                    schema.Category = _categoryService.Get(categoryName);
                    _dataSetSchemaService.Create(schema);    
                    ViewBag.CategoryName = categoryName;
                    ResetBreadCrumbsCache();
                    return schema.IsExternalData
                               ? Redirect(String.Format("/Admin/Schema/{0}", categoryName))
                               : Redirect(String.Format("/Admin/Schema/{0}/{1}/Edit?showfields=1", categoryName, schema.FriendlyUrl));
                }
                ViewBag.CategoryName = categoryName;
                
                ResetBreadCrumbsCache();

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.CategoryName = categoryName;
                ViewBag.Message = string.Format("<p class='warning'>Unable to create schema.  {0}</p>", ex.Message);
                return View();
            }
        }


        [HttpPost]
        [Authorize(Roles = "SchemaCreator,SuperAdministrator")]
        public JsonResult SaveSorting(string[] str, string categoryName, string schemaName)
        {
            
            return new JsonResult(){JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = _dataSetSchemaColumnService.SaveSorting(str, categoryName, schemaName)};
        }


    private static void ResetBreadCrumbsCache()
        {
            if (HttpRuntime.Cache["BreadCrumbs"] != null)
            {
                HttpRuntime.Cache.Remove("BreadCrumbs");
            }
        }

        public ActionResult AddColumn(string categoryName, string schemaName)
        {
            ViewBag.SchemaName = schemaName;
            ViewBag.CategoryName = categoryName;
            ViewBag.AlreadySorted = IsAlreadySorted(schemaName);
            return View("AddColumn");
        }

        [HttpPost]
        public ActionResult AddColumn(DataSetSchemaColumn column, string schemaName, string categoryName)
        {
            try
            {
                var schemaDetails = _dataSetSchemaService.Get(schemaName);

                if (!ModelState.IsValid)
                {
                    ViewBag.SchemaName = schemaName;
                    ViewBag.CategoryName = categoryName;
                    ViewBag.AlreadySorted = IsAlreadySorted(schemaName);
                    ViewBag.IsError = true;
                    return View("AddColumn", column);
                }

                column.SchemaDefinition = schemaDetails.Definition;
                _dataSetSchemaColumnService.Create(column);
                return Redirect(string.Format("/Admin/Schema/{0}/{1}/Edit?showfields=2", categoryName, schemaName));
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.SchemaName = schemaName;
                ViewBag.CategoryName = categoryName;
                ViewBag.AlreadySorted = IsAlreadySorted(schemaName);
                ViewBag.Message = string.Format("<p class='warning'>Unable to create field definition.  {0}</p>", ex.Message);
                return View();
            }
        }

        public ActionResult Edit(string categoryName, string schemaName)
        {


            if (TempData["SchemaSourceErrorMessage"] != null)
                ViewBag.Message = string.Format("<p class='note'>{0}.</p>", Convert.ToString(TempData["SchemaSourceErrorMessage"]));

            TempData["SchemaSourceErrorMessage"] = null;

 

            var schemaDetail = _dataSetSchemaService.Get(schemaName);
            schemaDetail.Category = _categoryService.Get(categoryName);
            ViewBag.SchemaName = schemaName;
            ViewBag.CategoryName = categoryName;

            PopulateDsSchemaWithEsdValues(schemaDetail);

            return View(schemaDetail);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DataSetSchema dsSchema, string categoryName, string schemaName, string showfields)
        {
            ViewBag.SchemaName = schemaName;
            ViewBag.CategoryName = categoryName;

            try
            {
                dsSchema.Category = _categoryService.Get(categoryName);
                dsSchema.Definition = _dataSetSchemaService.Get(schemaName).Definition;

                if (!ModelState.IsValid)
                {
                    PopulateDsSchemaWithEsdValues(dsSchema);
                    return View("Edit", dsSchema);
                }
               
                    SaveEsdLinks(dsSchema);/*save esd links before populating esd values as it will disappear when populated*/
                    PopulateDsSchemaWithEsdValues(dsSchema);
                    _dataSetSchemaService.Save(dsSchema);
                //ViewBag.Message = "<p class='note'></p>";
                ResetBreadCrumbsCache();
                TempData["SchemaSourceErrorMessage"] = "Schema definition successfully saved.";
                return RedirectToAction("Edit", new { categoryName, schemaName, showfields = showfields });
                //return View("Edit", dsSchema);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                dsSchema.Category = _categoryService.Get(categoryName);
                dsSchema.Definition = _dataSetSchemaService.Get(schemaName).Definition;
                ViewBag.Message = string.Format("<p class='warning'>Unable to save schema definition.  {0}.</p>", ex.Message);
                return View(dsSchema);
            }
        }

        private void SaveEsdLinks(DataSetSchema dsSchema)
        {
            var xlist = new List<string>();
            if (dsSchema.CurrentMappedEsdFunctionService != null)
            {
                foreach (var x in dsSchema.CurrentMappedEsdFunctionService)
                {
                    xlist.Add(Convert.ToString(x));
                }
            }

            _esdFunctionService.SaveLinkedFunctionServices(dsSchema.Id, xlist);
        }

        public ActionResult Approve(string categoryName, string schemaName)
        {
            _dataSetSchemaService.Approve(categoryName, schemaName);
            return Redirect(string.Format("/Admin/Schema/{0}", categoryName));
        }

        public ActionResult Delete(string categoryName, string schemaName)
        {
            _dataSetSchemaService.Delete(categoryName, schemaName);
            ResetBreadCrumbsCache();

            return Redirect(string.Format("/Admin/Schema/{0}", categoryName));
        }

        public ActionResult DeleteAll(string categoryName, string schemaName, string tab = "")
        {
            _dataSetSchemaService.DeleteAll(categoryName, schemaName);
            ResetBreadCrumbsCache();

            return Redirect(string.Format("/Admin/Schema/{0}?tab={1}", categoryName, tab));
        }

        public ActionResult Enable(string categoryName, string schemaName)
        {
            _dataSetSchemaService.Enable(categoryName, schemaName);
            return Redirect(string.Format("/Admin/Schema/{0}", categoryName));
        }

        public ActionResult EditColumn(string categoryName, string schemaName, string columnName)
        {
            ViewBag.AlreadySorted = IsAlreadySorted(schemaName);
            var col = _dataSetSchemaColumnService.Get(categoryName, schemaName, columnName);
            ViewBag.SchemaName = schemaName;
            ViewBag.CategoryName = categoryName;
            return View("AddColumn", col);
        }

        [HttpPost]
        public ActionResult EditColumn(DataSetSchemaColumn dsColumn, string categoryName, string schemaName)
        {
            
            
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.SchemaName = schemaName;
                    ViewBag.CategoryName = categoryName;
                    return View("AddColumn", dsColumn);
                }

                _dataSetSchemaColumnService.Save(dsColumn);
                ViewBag.Message = string.Format("<p class='note'>Changes to column {0} successfully saved.</p>", dsColumn.Title);
                return Redirect(string.Format("/Admin/Schema/{0}/{1}/Edit?showfields=2", categoryName, schemaName));
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                ViewBag.SchemaName = schemaName;
                ViewBag.CategoryName = categoryName;
                ViewBag.Message = string.Format("<p class='warning'>Unable to save column.  {0}.</p>", ex.Message);
                ViewBag.AlreadySorted = IsAlreadySorted(schemaName);
                var col = _dataSetSchemaColumnService.Get(categoryName, schemaName, dsColumn.Title);
                return View("AddColumn", col);
            }
        }

        public ActionResult DeleteColumn(string categoryName, string schemaName, string columnName)
        {
            var col = _dataSetSchemaColumnService.Get(categoryName, schemaName, columnName);
            _dataSetSchemaColumnService.Delete(col);
            return Redirect(string.Format("/Admin/Schema/{0}/{1}/Edit?showfields=2", categoryName, schemaName));
        }

        private bool IsAlreadySorted(string schemaName)
        {
            var schema = _dataSetSchemaService.Get(schemaName);
            return schema.Definition.Columns.FirstOrDefault(c => c.IsDefaultSort) != null;
        }

        private void PopulateDsSchemaWithEsdValues(DataSetSchema schemaDetail)
        {


            var esdService = _esdFunctionService;//new EsdFunctionService();

            //esdService.XmlContent = GetFunctionServicesXmlContentAndSaveToSession();

            //esdService.ProcessXmlContent();
            schemaDetail.EsdFunctions = esdService.GetFunctions();
            schemaDetail.EsdServices = esdService.GetServices();
            schemaDetail.CurrentMappedEsdFunctionService = esdService.GetLinkedFunctionsServices(schemaDetail.Id).Select(x => x);
        }
        private string GetFunctionServicesXmlContentAndSaveToSession()
        {
            //var wc = new WebClient();
            //wc.Proxy = new WebProxy(("10.4.246.76"), 3128);
            ////wc.Proxy = new WebProxy("");
            ////var result = wc.DownloadString("http://standards.esd.org.uk/xml?uri=list/functions&mappedToUri=list/services");
            //var result = wc.DownloadString("http://id.esd.org.uk/list/functions");
            //esdService.XmlContent = result;

            if (Session["functions_services.xml"] == null)
            {
                var x = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/functions_services.xml"));
                Session["functions_services.xml"] = x;
                return x;
            }
            return Convert.ToString(Session["functions_services.xml"]);


        }
    }
}
