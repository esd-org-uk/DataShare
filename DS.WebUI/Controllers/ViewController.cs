using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DS.Domain;
using DS.Domain.Interface;
using DS.WebUI.Controllers.Base;
using DS.WebUI.Controllers.Extensions;
using Elmah;

namespace DS.WebUI.Controllers
{
    public class ViewController : BaseController
    {
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetDetailService _dataSetDetailService;

        public ViewController(IDataSetSchemaService dataSetSchemaService,
                              IDataSetDetailService dataSetDetailService,
                              ICategoryService categoryService, 
                            ISystemConfigurationService systemConfigurationService)
            : base(systemConfigurationService, categoryService)
        {
            _dataSetSchemaService = dataSetSchemaService;
            _dataSetDetailService = dataSetDetailService;
            _categoryService = categoryService;
        }

        public ActionResult Index(string category, string schema, int? currentPage, int? numberToShow,
                                  string orderByColumn, string orderByDirection, IList<FilterCriteria> filter,
                                  string download, string preview)
        {
            ViewBag.ShowingLatest = false;
            ViewBag.Title = "View - DataShare";

            if (schema == null && category == null) return View("Index", _categoryService.GetAll(false));

            var schemaDetail = _dataSetSchemaService.Get(schema);

            if (schemaDetail == null)
            {
                if (category.ToLower() == "all"){
                    ViewBag.Title = "View all - DataShare";
                    return View("Schema", _dataSetSchemaService.GetAll());
                }

                var categoryDetail = _categoryService.Get(category);

                if (categoryDetail == null) return ReturnErrorView();

                ViewBag.Title = "View " + categoryDetail.Title + " data - DataShare";
                return View("Schema", _categoryService.GetByFriendlyUrl(category, false));
            }


            ViewBag.Schema = schemaDetail;
            ViewBag.FullDescription = schemaDetail.Description.RemoveWhiteSpaces().Replace("\"", "\'");

            if (!schemaDetail.IsOnline && preview != "true")
                return PartialView("Offline", schemaDetail.Title);
            

            if (!String.IsNullOrEmpty(download))
                return ReturnDownloadCsvData(filter, schemaDetail, orderByColumn, orderByDirection);
            

            if (schemaDetail.IsExternalData)
            {
                ViewBag.SchemaTitle = schemaDetail.Title;
                return View("ExternalDataSet", _dataSetSchemaService.GetByFriendlyUrl(category, schema));
            }

            ViewControllerData result = null;
            if (filter != null && filter.Count > 0)
            {
                ViewBag.PreFilter = filter;
                result = _dataSetDetailService.SearchSchema(filter,
                                                            schemaDetail,
                                                            currentPage ?? 1,
                                                            numberToShow ?? _systemConfigurationService.AppSettingsInt("DefaultViewPageSize"),
                                                            schemaDetail.Definition == null
                                                                ? ""
                                                                : schemaDetail.Definition.DefaultSortColumn,
                                                            schemaDetail.Definition == null
                                                                ? ""
                                                                : schemaDetail.Definition.DefaultSortColumnDirection,
                                                            true);
            }
            else
            {
                ViewBag.ShowingLatest = true;
                result = _dataSetDetailService.GetLatest(ViewBag.Schema);
            }

            if (schemaDetail.Definition != null)
                result.InitialColumns = schemaDetail.Definition.InitialVisibleColumnIndexes;
            ViewBag.Title = "View " + schemaDetail.Title + " - DataShare";

            return View("DataSet", result);
        }

        private ActionResult ReturnErrorView()
        {
            if (Response != null)
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                Response.TrySkipIisCustomErrors = true;
            }
            return View("Error");
        }


        [HttpPost]
        public ActionResult Ajax(string category, string schema, int? currentPage, int? numberToShow,
                                 string orderByColumn, string orderByDirection, IList<FilterCriteria> filter,
                                 string download, bool getVisualisationData, string chartType, string xAxis,
                                 string yAxis, int? chartCurrentPage, int? chartNumberToShow, string yAxisAggregate)
        {
            var schemaDetail = _dataSetSchemaService.Get(schema);
            try
            {
                if(!getVisualisationData)
                    return new JsonNetResult
                    {
                        Data = _dataSetDetailService.SearchSchema(filter,
                                          schemaDetail,
                                          currentPage ?? 1,
                                          numberToShow ?? _systemConfigurationService.AppSettingsInt("DefaultViewPageSize"),
                                          orderByColumn,
                                          orderByDirection,
                                          true)
                    };
                
                //get DataTable for map
                if (chartType == "map" && schemaDetail.Definition.HasLatLngColumns) //
                return new JsonNetResult {Data = _dataSetDetailService.VisualiseSchemaAsMap(filter, schemaDetail)};
                    

                //get DataTable for visualisation
                return Json(_dataSetDetailService.VisualiseSchema(filter,
                                                                schemaDetail,
                                                                chartCurrentPage?? 1,
                                                                chartNumberToShow ?? _systemConfigurationService.AppSettingsInt("DefaultGraphPageSize"),
                                                                chartType, xAxis, yAxis, yAxisAggregate));
                

                
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                
                return new JsonNetResult {Data = new ViewControllerData {Count = 0, CurrentPage = 1, Data = null}};
            }
        }

        public ActionResult Help()
        {
            return View("Help");
        }

        private ActionResult ReturnDownloadCsvData(IList<FilterCriteria> filter, 
            DataSetSchema schemaDetail, string orderByColumn, string orderByDirection)
        {
            var results = _dataSetDetailService.SearchSchema(filter,
                                                              schemaDetail,
                                                              orderByColumn,
                                                              orderByDirection);
            var csv = results.Data.AppendBody(_systemConfigurationService)
                             .ToCsv(",",
                                    schemaDetail.Definition == null
                                        ? new List<DataSetSchemaColumn>()
                                        : schemaDetail.Definition.Columns);
            return new CsvResult(csv, schemaDetail.FriendlyUrl);
        }
    }
}
