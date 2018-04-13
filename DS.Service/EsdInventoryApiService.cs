using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using StructureMap;

namespace DS.Service
{
    public class EsdInventoryApiService : IEsdInventoryApiService
    {
        private readonly IEsdFunctionService _esdFunctionService;
        private IRepository<Category> _repository;

        public EsdInventoryApiService(IEsdFunctionService esdFunctionService, IRepository<Category> repository)
        {

            RootUrl = "Please set in Web.config appsettings  with key - RootUrl - root url of the datashare website";
            OpenGovLicenceUrl = "Please set in Web.config appsettings  with key - OpenGovLicence - Open Gov Licence url - i.e. http://www.nationalarchives.gov.uk/doc/open-government-licence";
//            SpatialUrl = "Please set in Web.config appsettings  with key - CouncilURI - url with for the coverage of the dataset - spatial - i.e. http://statistics.data.gov.uk/id/statistical-geography/E09000026";
            _esdFunctionService = esdFunctionService;
            _repository = repository;
        }



        /// <summary>
        /// root url of the datashare website
        /// </summary>
        public string RootUrl { get; set; }

        /// <summary>
        /// Open Gov Licence url - i.e. http://www.nationalarchives.gov.uk/doc/open-government-licence
        /// </summary>
        public string OpenGovLicenceUrl { get; set; }

        ///// <summary>
        ///// url with for the coverage of the dataset - spatial - i.e. http://statistics.data.gov.uk/id/statistical-geography/E09000026
        ///// </summary>
        //public string SpatialUrl { get; set; }
        
        /// <summary>
        /// RightsHolder of the dataset
        /// </summary>
        public string RightsHolder { get; set; }

        public List<InventoryDataset> GetInventoryDataset()
        {


            var xlist = new List<InventoryDataset>();
            var categories = _repository.GetQuery()
                .Include("Schemas")
                .Include("Schemas.DataSets").ToList();

            categories.ForEach(x =>
                {
                    

                    if (x.Schemas != null && x.Schemas.Count > 0)
                    {
                        var schemas = x.Schemas.Where(u => !u.IsDisabled && u.IsApproved).ToList();
                        
                        xlist.AddRange(
                            schemas.Select(schema => new InventoryDataset()
                                {
                                    Description = schema.Description,
                                    Title = schema.Title,
                                    Active = (!schema.IsDisabled).ToString(),
                                    Identifier = x.FriendlyUrl + "/" + schema.FriendlyUrl,
                                    Rights = OpenGovLicenceUrl,
                                    RightsHolder = RightsHolder,
                                    Resources = GetResources(x, schema),
                                    Modified = schema.DateLastUploadedTo ??  new DateTime(1900, 1, 1),
                                    ModifiedSpecified = schema.DataSets != null && schema.DateLastUploadedTo.HasValue,
                                    Subjects = GetLinkedCategories(schema.Id).ToArray(),
                                }).ToList());

                    }
                });
        
            return xlist;
        }


        private List<InventoryDatasetSubjectsSubject> GetLinkedCategories(int schemaId)
        {
           if(_esdFunctionService == null) return new List<InventoryDatasetSubjectsSubject>();
            var x = _esdFunctionService.GetLinkedFunctionsServices(schemaId).Select(y => new InventoryDatasetSubjectsSubject()
            {
                Function = y.Type == "Function" ? y.Url : "",
                Service = y.Type == "Service" ? y.Url : "",
                Scheme = y.Type == "Service" ? "subject.service" : "subject.function",
                
            }).ToList();
            return x;
        }

        private InventoryDatasetResource[] GetResources(Category category, DataSetSchema dataSetSchema)
        {

            var xList = new List<InventoryDatasetResource>();
            if (dataSetSchema.DataSets == null) return xList.ToArray();

            var datasets = dataSetSchema.DataSets.ToList();
            AddDocumentResource(ref xList, dataSetSchema, category);
            AddResource(ref xList, dataSetSchema, category, "csv", datasets, "download", "Download");
            AddResource(ref xList, dataSetSchema, category, "xml", datasets, "download", "Download");
            AddResource(ref xList, dataSetSchema, category, "json", datasets, "api", "Service");
            AddResource(ref xList, dataSetSchema, category, "xml", datasets, "api", "Service");
            

            return xList.ToArray();
        }

        private void AddResource(ref List<InventoryDatasetResource> xList, DataSetSchema dataSetSchema, 
            Category category, string mimeType, List<DataSetDetail> datasets
            , string apiordownload
            ,string typeOfAvailability)
        {

            //var datasets = 
            xList.AddRange(datasets.Select(y => new InventoryDatasetResource()
            {
                Description = y.Note,
                Type = "Data",
                Title = y.Title,
                Identifier = (apiordownload == "api" ? "api/" : "download/" ) + category.FriendlyUrl + "/" + dataSetSchema.FriendlyUrl + "/" + y.FriendlyUrl + "/" + String.Format("{0}", mimeType),
                Modified = dataSetSchema.DateLastUploadedTo ?? new DateTime(1900, 1, 1),
                ModifiedSpecified = dataSetSchema.DateLastUploadedTo.HasValue,
                Language = "en",
                Active = (!dataSetSchema.IsDisabled).ToString(),
                Renditions = new InventoryDatasetResourceRenditions()
                {
                    Rendition = new InventoryDatasetResourceRenditionsRendition()
                    {
                        Availability = typeOfAvailability,
                        Title = y.Title,
                        Description = dataSetSchema.ShortDescription,
                        Identifier = (apiordownload == "api" ?
                        RootUrl + "/api/" + category.FriendlyUrl + "/" + dataSetSchema.FriendlyUrl + "/" + y.FriendlyUrl + "?format=" + String.Format("{0}", mimeType.ToUpper())
                        : RootUrl + "/download/" + category.FriendlyUrl + "/" + dataSetSchema.FriendlyUrl + "/" + y.FriendlyUrl + "/" + String.Format("{1}?version={0}", DateTime.Today.ToShortDateString(), mimeType.ToUpper())),
                        MimeType = (mimeType == "json" ? "application" : "text")+ "/" + mimeType.ToLower(),
                        Modified = y.DateUpdated ?? new DateTime(1900, 1, 1),
                        ModifiedSpecified = y.DateUpdated.HasValue,
                        Active = (!dataSetSchema.IsDisabled).ToString(),
                        ConformsTo = (dataSetSchema.IsStandardisedSchemaUrl?? false) ? new string[] { dataSetSchema.SchemaDefinitionFromUrl } : new string[] { RootUrl + "/api/" + category.FriendlyUrl + "/" + dataSetSchema.FriendlyUrl + "/definition" }

                    }
                }

            }));

        }


        private void AddDocumentResource(ref List<InventoryDatasetResource> xList, DataSetSchema dataSetSchema, Category category)
        {
            xList.Add(new InventoryDatasetResource()
            {
                Description = dataSetSchema.ShortDescription,
                Type = "Document",
                Title = dataSetSchema.Title,
                Identifier = dataSetSchema.FriendlyUrl,
                Modified = dataSetSchema.DateLastUploadedTo ?? new DateTime(1900, 1, 1),
                ModifiedSpecified = dataSetSchema.DateLastUploadedTo.HasValue,
                Renditions = new InventoryDatasetResourceRenditions()
                {
                    Rendition = new InventoryDatasetResourceRenditionsRendition()
                    {
                        Title = dataSetSchema.Title,
                        Description = dataSetSchema.ShortDescription,
                        Identifier = RootUrl + "/Download/" + category.FriendlyUrl + "/" + dataSetSchema.FriendlyUrl,
                        MimeType = "text/html",
                        Availability = "Download",
                        Modified = dataSetSchema.DateLastUploadedTo ?? new DateTime(1900,1,1),
                        ModifiedSpecified = dataSetSchema.DateLastUploadedTo.HasValue,
                        ConformsTo = (dataSetSchema.IsStandardisedSchemaUrl ?? false) ? new string[] {dataSetSchema.SchemaDefinitionFromUrl } : new string[] { RootUrl + "/api/" + category.FriendlyUrl + "/" + dataSetSchema.FriendlyUrl + "/definition" }

                    }
                }

            });
        }


  
    }

}
