using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.Extensions;
using DS.Service;
using DS.WebUI.Controllers.Base;
using DS.WebUI.Controllers.Extensions;
using Elmah;

namespace DS.WebUI.Areas.Admin.Controllers
{[CustomHttps]
    [Authorize(Roles = "Uploader,SchemaEditor,SchemaCreator,SuperAdministrator")]
    public class UploadController : BaseController
    {
    private IDataSetSchemaService _dataSetSchemaService;
    private IDataSetDetailService _dataSetDetailService;
    private IDataSetDetailUploaderService _uploaderService;

    public UploadController(IDataSetSchemaService dataSetSchemaService, 
        IDataSetDetailService dataSetDetailService,
        ICategoryService categoryService, 
        IDataSetDetailUploaderService uploaderService, ISystemConfigurationService systemConfigurationService) 
        : base (systemConfigurationService, categoryService)
    {
        _dataSetSchemaService = dataSetSchemaService;
        _dataSetDetailService = dataSetDetailService;
        _categoryService = categoryService;
        _uploaderService = uploaderService;
    }
    
    
    public ActionResult Index(string category, string schema, string actionToPerform)
        {
            ViewBag.Message = TempData["Message"];
            return LoadList(category, schema, actionToPerform);
        }

        [HttpPost]
        public ActionResult Index(string category, string schema, string actionToPerform, string titleExternal, string externalUrl, string externalType)
        {
            var schemaDetail = _dataSetSchemaService.Get(category, schema);
            if (externalUrl.Substring(0,4)!="http")
            {
                externalUrl = "http://" + externalUrl;
            }
            _uploaderService.AddExternalDataSet(schemaDetail.Id, titleExternal, externalUrl, externalType);

            ViewBag.Message = "<p class='note'>External dataset successfully added.</p>";
            return View("Add", schemaDetail);
        }

        public ActionResult LoadList(string category, string schema, string actionToPerform)
        {
            if (!string.IsNullOrEmpty(actionToPerform))
            {
                var schemaDetail = _dataSetSchemaService.Get(category, schema);

                ViewBag.Title = "Upload data - Admin - DataShare";
                ViewBag.SchemaUrl = schemaDetail.FriendlyUrl;
                switch (actionToPerform)
                {
                    case "Add":
                        return View(actionToPerform, schemaDetail);
                    case "Edit":
                        var dataSets = _dataSetSchemaService.GetByFriendlyUrl(category, schema, true);
                        return View(actionToPerform, dataSets);
                }
                
            }

            if (!string.IsNullOrEmpty(schema))
            {
                var dataSets = _dataSetSchemaService.GetByFriendlyUrl(category, schema);
                return View("DataSet", dataSets);
            }

            return category != null ? View("Schema", _categoryService.GetByFriendlyUrl(category, true))
                                    : View("Index", _categoryService.GetAll(true));
        }

        public ActionResult Delete(int dataSetDetailId, int schemaId)
        {
            var dataSetDetail = _dataSetDetailService.Get(dataSetDetailId);
            
            //remove all the data for dataSetDetailId
            if (!dataSetDetail.Schema.IsExternalData) _uploaderService.DeleteExternalDataSetDetail(dataSetDetail);
            
            //{
            //    var deleteSql = String.Format(@"exec DS_DataSet_Delete ""{0}"", {1}",dataSetDetail.Schema.Definition.TableName, dataSetDetailId);
            //    SqlExtensions.ExecuteQuery(_dataSetDetailService.Repository.Connection.ConnectionString, deleteSql);
            //}
            
            var category = dataSetDetail.Schema.Category.FriendlyUrl;
            var schema = dataSetDetail.Schema.FriendlyUrl;

            _uploaderService.DeleteDataSetDetail(dataSetDetail);

            //_dataSetDetailService.Repository.Delete(dataSetDetail);
            //_dataSetDetailService.Repository.SaveChanges();
            
            //ViewBag.Message = "Deleted successfully";
            TempData["Message"] = dataSetDetail.Title + " deleted successfully.";
            return Redirect(String.Format("~/Admin/Upload/{0}/{1}/Edit", category, schema));
        }

        [Authorize]
        public bool CheckExistsAlready(int schemaId, string title)
        {
            return _uploaderService.DatasetExistsAlready(schemaId, title);
        }

        [Authorize]
        public void UploadCsv(int? chunk, string name, string title)
        {
            var fileUpload = Request.Files[0];
            chunk = chunk ?? 0;
            
            using (var fs = new FileStream(Path.Combine(_systemConfigurationService.AppSettings("MediaAssetFolder"), name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
        }
        
        [Authorize]
        public ActionResult SaveCsv(int schemaId, string title, string fileName)
        {
            var filePath = Path.Combine(_systemConfigurationService.AppSettings("MediaAssetFolder"), fileName);
            var result = _uploaderService.SaveCsv(schemaId, title, filePath);
            result.Errors = result.Errors.Take(100).ToList();

            return Json(new { result.Errors });
        }

        public ActionResult DownloadTemplate(int schemaId, string schemaName)
        {
            var data = _dataSetDetailService.GetTemplateCsv(schemaId);
            return new CsvResult(data, schemaName);
        }

        public ActionResult DownloadDefinition(int schemaId)
        {
            var schema = _dataSetSchemaService.Get(schemaId);
            var def = new StringBuilder();
            def.Append(String.Format(@"This document contains the definition and validation rules for uploading data to ""{0}""{1}{1}",schema.Title, Environment.NewLine));
            def.Append("List of columns:" + Environment.NewLine + Environment.NewLine);
            def.Append(schema.Definition.AsString);
            return new TextFileResult(def.ToString(), schema.Title);
        }

        public ActionResult GetData(string url)
        {
            var returnData = new UploadExternalResult {FileSize = 0};
            try
            {
                var request = WebRequest.Create(new Uri(url));
                using (var response = request.GetResponse())
                {
                    if (response != null && response.ContentLength > 0)
                    {
                        returnData.FileSize = Convert.ToInt32(response.ContentLength);
                        return new JsonResult {Data = returnData, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
                    }
                }
            }

            catch (WebException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new JsonResult { Data = returnData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = returnData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}
