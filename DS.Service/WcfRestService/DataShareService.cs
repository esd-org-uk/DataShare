using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using DS.Domain;
using DS.Domain.Interface;
using DS.Domain.WcfRestService;
using DS.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StructureMap;
using Formatting = System.Xml.Formatting;

namespace DS.Service.WcfRestService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DataShareService : IDataShareService
    {
        private Regex _date_dd_mm_yyyy_Regex = new Regex(@"^\d{2}-\d{2}-\d{4}$");

        public IList<RestCategory> GetCategory()
        {
            var categoryService = ObjectFactory.GetInstance<ICategoryService>();

            return
                categoryService.GetAll(false)
                    //.Where(c => c.IsOnline) /*this is redundant as the getAll_only_returns_is_online_if_set_to_false*.
                               .Select(c => new RestCategory
                                   {
                                       Title = c.Title,
                                       Description = c.Description,
                                       FriendlyUrl = c.Title.ToUrlSlug()
                                   }).ToList();
        }

        public IList<RestSchema> GetSchemas(string categoryUrl)
        {
            var categoryService = ObjectFactory.GetInstance<ICategoryService>();
            return categoryService.GetByFriendlyUrlIsOnline(categoryUrl);
        }

        //not used anymore  see GetSchemaDefinitionV2
        //public IList<RestColumnDefinition> GetSchemaDefinition(string categoryUrl, string schemaUrl)
        //{
        //    var dataSetSchemaService = ObjectFactory.GetInstance<IDataSetSchemaService>();
        //    var schema = dataSetSchemaService.Get(categoryUrl, schemaUrl);

        //    var def = new List<RestColumnDefinition>();

        //    if (schema.IsDisabled)
        //    {
        //        def.Add(new RestColumnDefinition { Name = "This schema has been disabled" });
        //        return def;
        //    }

        //    def.AddRange(schema.Definition.Columns.Select(c => new RestColumnDefinition
        //    {
        //        Name = c.ColumnName,
        //        Description = c.Title,
        //        Type = c.Type,
        //        MaxSize = c.Type == "Text" ? c.MaxSize.ToString() : "N/A",
        //        MinDate = c.Type == "DateTime" && c.MinDate != null ? c.MinDate.ToString() : "N/A",
        //        MaxDate = c.Type == "DateTime" && c.MaxDate != null ? c.MaxDate.ToString() : "N/A",
        //        MinCurrency = c.Type == "Currency" && c.MinCurrency != null ? c.MinCurrency.ToString() : "N/A",
        //        MaxCurrency = c.Type == "Currency" && c.MaxCurrency != null ? c.MaxCurrency.ToString() : "N/A",
        //        MinNumber = c.Type == "Number" && c.MinNumber != null ? c.MinNumber.ToString() : "N/A",
        //        MaxNumber = c.Type == "Number" && c.MaxNumber != null ? c.MaxNumber.ToString() : "N/A",
        //        IsRequired = c.IsRequired,
        //        HelpText = c.HelpText,
        //        Uri = c.LinkedDataUri ?? ""
        //    }));

        //    return def;
        //}

        public SchemaRestDefinition GetSchemaDefinitionV2(string categoryUrl, string schemaUrl)
        {
            var dataSetSchemaService = ObjectFactory.GetInstance<IDataSetSchemaService>();
            var schema = dataSetSchemaService.Get(categoryUrl, schemaUrl);
            SchemaRestDefinition schemaXml;


            if (schema == null)
            {
                schemaXml = new SchemaRestDefinition() {};
                schemaXml.EsdLinks = null;
                schemaXml.ErrorMessage = "This schema does not exist!";
                return schemaXml;
            }

            if (schema.IsDisabled || !schema.IsOnline)
            {
                schemaXml = new SchemaRestDefinition() {};
                schemaXml.EsdLinks = null;
                schemaXml.ErrorMessage = "This schema has been disabled";
                return schemaXml;
            }


            schemaXml = new SchemaRestDefinition(schema.SchemaDefinitionFromUrl) {};

            schemaXml.RestSchema = new RestSchema()
                {
                    Title = schema.Title,
                    ShortDescription = schema.ShortDescription,
                    Description = schema.Description,
                    FriendlyUrl = schema.Title.ToUrlSlug()
                };

            var esdFunctionService = ObjectFactory.GetInstance<IEsdFunctionService>();

            var esdLinks =
                esdFunctionService.GetLinkedFunctionsServices(schema.Id)
                                  .Select(x => new EsdLink() {Id = x.Identifier, Title = x.Label, Type = x.Type})
                                  .ToList();

            schemaXml.EsdLinks = new EsdLinks() {Links = esdLinks};
            if (schema.Definition == null) return schemaXml;

            var def = new List<RestColumnDefinition>();
            def.AddRange(schema.Definition.Columns.Select(c => new RestColumnDefinition
                {
                    Name = c.ColumnName,
                    Description = c.Title,
                    Type = c.Type,
                    MaxSize = ((c.Type?? "").ToLower() == "text") ? c.MaxSize.ToString() : "N/A",
                    MinDate = (c.Type ?? "") == "DateTime" && c.MinDate != null ? c.MinDate.ToString() : "N/A",
                    MaxDate = (c.Type ?? "") == "DateTime" && c.MaxDate != null ? c.MaxDate.ToString() : "N/A",
                    MinCurrency = (c.Type ?? "") == "Currency" && c.MinCurrency != null ? c.MinCurrency.ToString() : "N/A",
                    MaxCurrency = (c.Type ?? "") == "Currency" && c.MaxCurrency != null ? c.MaxCurrency.ToString() : "N/A",
                    MinNumber = (c.Type ?? "") == "Number" && c.MinNumber != null ? c.MinNumber.ToString() : "N/A",
                    MaxNumber = (c.Type ?? "") == "Number" && c.MaxNumber != null ? c.MaxNumber.ToString() : "N/A",
                    IsRequired = c.IsRequired,
                    HelpText = c.HelpText,
                    Uri = c.LinkedDataUri ?? "",
                    DisplayInitial = c.IsShownInitially,
                    SortDirection = c.DefaultSortDirection,
                    Sorted = c.IsDefaultSort,
                    IsTotalled = c.IsTotalisable,
                }));

            schemaXml.RestColumnDefinitions = new RestColumnDefinitions() {ColumnDefinitions = def};
            return schemaXml;
        }

        public IList<RestDataSet> GetDataSets(string categoryUrl, string schemaUrl)
        {
            var dataSetSchemaService = ObjectFactory.GetInstance<IDataSetSchemaService>();
            var rootUrl = ConfigurationManager.AppSettings["RootURL"];

            var datasets = dataSetSchemaService.Repository.GetQuery()
                                               .Include(s => s.DataSets)
                                               .Include(s => s.Category).ToList()
                                               .Where(s => s.FriendlyUrl == schemaUrl).FirstOrDefault()
                                               .DataSets.Where(d => d.IsOnline)
                                               .Select(s => new RestDataSet
                                                   {
                                                       Title = s.Title,
                                                       Note = s.Note,
                                                       FriendlyUrl = s.FileType == "KML"
                                                                         ? s.FileUrl
                                                                         : s.FileType == "RSS"
                                                                               ? s.FileUrl
                                                                               : string.Format(
                                                                                   "{0}/download/{1}/{2}/{3}/XML",
                                                                                   rootUrl, categoryUrl, schemaUrl,
                                                                                   s.FriendlyUrl),
                                                       DateUpdated =
                                                           s.DateUpdated != null
                                                               ? Convert.ToDateTime(s.DateUpdated)
                                                               : s.DateCreated,
                                                       DateCreated = s.DateCreated
                                                   }).ToList();


            return datasets;
        }

        public Stream GetDataSetDetail(string categoryUrl, string schemaUrl, string schemaDetailUrl, string format)
        {
            var dataSetSchemaService = ObjectFactory.GetInstance<IDataSetSchemaService>();
            var schema = dataSetSchemaService.Get(schemaUrl);


            if (!schema.IsOnline)
            {
                return GetErrorStream("This data is not available", format);
            }

            var datasetdetailservice = ObjectFactory.GetInstance<IDataSetDetailService>();

            var result = new ViewControllerData();
            result.Data = datasetdetailservice.GetData(schemaDetailUrl, schemaUrl);
            result.Count = result.Data.Rows.Count;

            if (result.Count == 0)
            {
                return GetErrorStream("No matching records found", format);
            }

            if (result.Count > 10000)
            {
                return
                    GetErrorStream(
                        result.Count +
                        @" records were returned. The maximum the api allows is 10000. Please use the download area of datashare to get a csv copy of all the data .",
                        format);
            }


            return GetDataTableStream(result.Data, format);
        }

        public Stream SearchSchemaTextContains(string categoryUrl, string schemaUrl, string format, string fieldToSearch,
                                               string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperatorNumber = "contains",
                    SearchText = searchText
                };
            return SearchSchema(schemaUrl, format, filter);
            //return SearchSchema( schemaUrl, format, fieldToSearch, "contains", searchText, "", "", "");
        }

        public Stream SearchSchemaTextEquals(string categoryUrl, string schemaUrl, string format, string fieldToSearch,
                                             string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperator = "isequalto",
                    SearchText = searchText
                };
            return SearchSchema(schemaUrl, format, filter);
            //   return SearchSchema( schemaUrl, format, fieldToSearch, "isequalto", searchText, "", "", "");
        }

        public Stream SearchSchemaNumberEquals(string categoryUrl, string schemaUrl, string format, string fieldToSearch,
                                               string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperatorNumber = "isequalto",
                    SearchNumber = searchText
                };
            return SearchSchema(schemaUrl, format, filter);

            //return SearchSchema( schemaUrl, format, fieldToSearch, "isequalto", "", searchText, "", "");
        }

        public Stream SearchSchemaNumberGreaterThan(string categoryUrl, string schemaUrl, string format,
                                                    string fieldToSearch, string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperatorNumber = "greaterthan",
                    SearchNumber = searchText
                };
            return SearchSchema(schemaUrl, format, filter);

            //return SearchSchema( schemaUrl, format, fieldToSearch, "greaterthan", "", searchText, "", "");
        }

        public Stream SearchSchemaNumberGreaterThanEqualTo(string categoryUrl, string schemaUrl, string format,
                                                           string fieldToSearch, string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperatorNumber = "greaterthanequalto",
                    SearchNumber = searchText
                };
            return SearchSchema(schemaUrl, format, filter);

            //return SearchSchema( schemaUrl, format, fieldToSearch, "greaterthanequalto", "", searchText, "", "");
        }

        public Stream SearchSchemaNumberLessThan(string categoryUrl, string schemaUrl, string format,
                                                 string fieldToSearch, string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperatorNumber = "lessthan",
                    SearchNumber = searchText
                };
            return SearchSchema(schemaUrl, format, filter);

            //return SearchSchema( schemaUrl, format, fieldToSearch, "lessthan", "", searchText, "", "");
        }

        public Stream SearchSchemaNumberLessThanEqualTo(string categoryUrl, string schemaUrl, string format,
                                                        string fieldToSearch, string searchText)
        {
            var filter = new FilterCriteria
                {
                    ColumnToSearch = fieldToSearch,
                    SearchOperatorNumber = "lessthanequalto",
                    SearchNumber = searchText
                };

            return SearchSchema(schemaUrl, format, filter);
            //return SearchSchema( schemaUrl, format, fieldToSearch, "lessthanequalto", "", searchText, "", "");
        }

        public Stream SearchSchemaDate(string categoryUrl, string schemaUrl, string format, string searchField,
                                       string from, string to)
        {
            if (!_date_dd_mm_yyyy_Regex.IsMatch(from) || !_date_dd_mm_yyyy_Regex.IsMatch(to))
                return GetErrorStream("Invalid Date Format - needs to be in DD-MM-YYYY format", format);

            var filter = new FilterCriteria {ColumnToSearch = searchField, From = from, To = to};

            return SearchSchema(schemaUrl, format, filter);

            //return SearchSchema( schemaUrl, format, searchField, "", "", "", from, to);
        }


        public Inventory GetEsdInventory()
        {
            var config = (ObjectFactory.GetInstance<ISystemConfigurationService>()).GetSystemConfigurations();
            var councilUri = config.CouncilUri;
            var councilUrl = config.CouncilUrl;
            var councilName = config.CouncilName;
            var spatialUri = config.CouncilSpatialUri;


            var esdApiService = ObjectFactory.GetInstance<IEsdInventoryApiService>();

            esdApiService.OpenGovLicenceUrl = System.Configuration.ConfigurationManager.AppSettings["OpenGovLicence"];
            esdApiService.RootUrl = System.Configuration.ConfigurationManager.AppSettings["RootUrl"];
            esdApiService.RightsHolder = councilName;

            var datasets = esdApiService.GetInventoryDataset();
            //.Where(x=>x.Title.Contains("Payments over 500"));/* use here if only want to select a specific dataset*/
            var inv = new Inventory()
                {
                    Metadata = new InventoryMetadata()
                        {
                            Description = councilName + " Datasets",
                            Publisher = councilUri,
                            Title =
                                String.Format("Inventory covering a selection of {0} datasets",
                                              councilName),
                            Coverage = new string[] {spatialUri}
                        },
                    Identifier = councilUrl,
                    Creator = councilUrl,
                    Datasets = datasets.ToArray(),
                    Language = "en",
                };
            if (datasets != null && datasets.Count > 0)
                inv.Modified = datasets.OrderByDescending(x => x.Modified).FirstOrDefault().Modified;

            return inv;
        }


        private Stream SearchSchema(string schemaUrl, string format, string searchField, string searchOperator,
                                    string searchText, string searchNumber, string from, string to)
        {
            var dataSetSchemaService = ObjectFactory.GetInstance<IDataSetSchemaService>();
            var schema = dataSetSchemaService.Repository.GetQuery()
                                             .Include(s => s.Category)
                                             .Include(s => s.Definition.Columns).ToList()
                                             .FirstOrDefault(c => c.FriendlyUrl == schemaUrl);

            if (schema == null)
            {
                return GetErrorStream("This data is not available", format);
            }

            if (!schema.IsOnline)
            {
                return GetErrorStream("This data is not available", format);
            }


            var filter = new List<FilterCriteria>
                {
                    new FilterCriteria
                        {
                            ColumnToSearch = searchField,
                            SearchOperator = searchText != "" ? searchOperator : "",
                            SearchOperatorNumber = searchNumber != "" ? searchOperator : "",
                            SearchText = searchText != "" ? searchText : "",
                            SearchNumber = searchNumber != "" ? searchNumber : "",
                            From = from != "" ? from : "",
                            To = to != "" ? to : "",
                        }
                };

            var datasetdetailservice = ObjectFactory.GetInstance<IDataSetDetailService>();
            var result = datasetdetailservice.SearchSchema(filter, schema, -1, -1, searchField, "ASC", false);

            if (result.Count == 0)
            {
                return GetErrorStream("No matching records found", format);
            }

            if (result.Count > 10000)
            {
                return
                    GetErrorStream(
                        result.Count +
                        " records were returned. The maximum the api allows is 10000. Please change the search criteria to return less records.",
                        format);
            }

            return GetDataTableStream(result.Data, format);
        }

        private Stream SearchSchema(string schemaUrl, string format, FilterCriteria filterCriteria)
        {
            var dataSetSchemaService = ObjectFactory.GetInstance<IDataSetSchemaService>();
            var schema = dataSetSchemaService.Repository.GetQuery()
                                             .Include(s => s.Category)
                                             .Include(s => s.Definition.Columns).ToList()
                                             .FirstOrDefault(c => c.FriendlyUrl == schemaUrl);

            if (schema == null)
                return GetErrorStream("This data is not available", format);


            if (!schema.IsOnline)
                return GetErrorStream("This data is not available", format);


            var filter = new List<FilterCriteria> {filterCriteria};

            var datasetdetailservice = ObjectFactory.GetInstance<IDataSetDetailService>();
            var result = datasetdetailservice.SearchSchema(filter, schema, -1, -1, filterCriteria.ColumnToSearch, "ASC",
                                                           false);

            if (result.Count == 0)
                return GetErrorStream("No matching records found", format);


            if (result.Count > 10000)
                return
                    GetErrorStream(
                        result.Count +
                        " records were returned. The maximum the api allows is 10000. Please change the search criteria to return less records.",
                        format);


            return GetDataTableStream(result.Data, format);
        }

        private static Stream GetDataTableStream(DataTable dt, string format)
        {
            var loweredFormat = format ?? "xml";
            loweredFormat = loweredFormat.ToLower();
            var dtSerializer = ObjectFactory.GetNamedInstance<IDataTableSerializer>(loweredFormat);
            return dtSerializer.GetStream(dt);
        }


        private static Stream GetErrorStream(string errorMessage, string format)
        {
            var error = new DataTable("Error");
            error.Columns.Add("ErrorDetail", typeof (string));
            var dr = error.NewRow();
            dr["ErrorDetail"] = errorMessage;
            error.Rows.Add(dr);
            var loweredFormat = format ?? "xml";
            loweredFormat = loweredFormat.ToLower();
            var dtSerializer = ObjectFactory.GetNamedInstance<IDataTableSerializer>(loweredFormat);
            return dtSerializer.GetStream(error);
        }
    }
}
