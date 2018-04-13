using System;
using System.Data;
using System.Net;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.WebUI.Controllers.Base;
using DS.WebUI.Controllers.Extensions;

namespace DS.WebUI.Controllers
{
    public class DownloadController : BaseController
    {
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetDetailService _dataSetDetailService;

        public DownloadController(IDataSetSchemaService dataSetSchemaService,
                                  IDataSetDetailService dataSetDetailService,
                                  ICategoryService categoryService,
                                  ISystemConfigurationService systemConfigurationService)
            : base(systemConfigurationService, categoryService)
        {
            _dataSetSchemaService = dataSetSchemaService;
            _dataSetDetailService = dataSetDetailService;
            _categoryService = categoryService;
        }

        public ActionResult Index(string category, string schema, string fileTitle, string downloadFormat,
                                  string searchText, int? sId, string from, string to)
        {
            return LoadList(category, schema, fileTitle, downloadFormat, searchText, sId, from, to);
        }

        public ActionResult LoadList(string category, string schema, string fileTitle, string downloadFormat,
                                     string searchText, int? sId, string from, string to)
        {
            if (schema == null)
                return ReturnCategoryView(category);

            var schemaDetail = _dataSetSchemaService.Get(schema);
            
            if (schemaDetail == null)
                return ReturnErrorView();

            if (!string.IsNullOrEmpty(fileTitle) && !string.IsNullOrEmpty(downloadFormat))
                return ReturnDownloadFile(fileTitle, schema, schemaDetail, downloadFormat);
            
            SetViewBagValues(schemaDetail);

            if (sId != null && (searchText != null || from != null || to != null))
                return ReturnDatasetSearch((int)sId, searchText, from, to);

            var dataSets = _dataSetSchemaService.GetByFriendlyUrl(category, schema);
            return View("DataSet", dataSets);
      
            
        }

        private void SetViewBagValues(DataSetSchema schemaDetail)
        {
            ViewBag.Title = String.Format("Download {0} - DataShare", schemaDetail.Title);
            ViewBag.SchemaTitle = schemaDetail.Title;
            ViewBag.ShortDescription = schemaDetail.ShortDescription;

            //Remove all whitespace and make sure it's ok to pass this to js
            ViewBag.FullDescription = (schemaDetail.Description != null)
                                          ? schemaDetail.Description.RemoveWhiteSpaces().Replace("\"", "\'")
                                          : "";
            ViewBag.SchemaId = schemaDetail.Id;
            ViewBag.IsExternalData = schemaDetail.IsExternalData;
        }
        
        private ActionResult ReturnDownloadFile(string fileTitle, string schema, DataSetSchema schemaDetail, string downloadFormat)
        {
            var fileDetail = _dataSetDetailService.GetData(fileTitle, schema);

            if (fileDetail == null) return ReturnErrorView();

            schemaDetail.Definition = schemaDetail.Definition ?? new DataSetSchemaDefinition();
            return this.DownloadDataSet(downloadFormat, fileDetail, fileTitle,
                                        schemaDetail.Definition.Columns, _systemConfigurationService);
            
        }

        private ActionResult ReturnCategoryView(string category)
        {
            if (category == null)
                return View("Index", _categoryService.GetAll(false));

            if (category == "All")
                return View("Schema", _dataSetSchemaService.GetAll());

            var categoryDetail = _categoryService.Get(category);
            if (categoryDetail != null)
                return View("Schema", _categoryService.GetByFriendlyUrl(category, false));

            return ReturnErrorView();
        }

        private ActionResult ReturnDatasetSearch(int sId, string searchText, string from , string to)
        {
            var results = _dataSetDetailService.Search((int)sId, searchText, from, to);
            return View("DataSet", results);
        }


        private ActionResult ReturnErrorView()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.TrySkipIisCustomErrors = true;
            return View("Error");
        }
        
    }
}
