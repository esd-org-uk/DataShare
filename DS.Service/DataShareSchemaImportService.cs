using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS.Domain;
using DS.Domain.Interface;
using DS.Domain.WcfRestService;

namespace DS.Service
{
    public class DataShareSchemaImportService :IDataShareSchemaImportService
    {
        private readonly IXmlToObjectService _xmlToObjectService;
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetSchemaColumnService _dataSetSchemaColumnService;

        public DataShareSchemaImportService(IXmlToObjectService xmlToObject, IDataSetSchemaService dataSetSchemaService, IDataSetSchemaColumnService dataSetSchemaColumnService)
        {
            _xmlToObjectService = xmlToObject;
            _dataSetSchemaService = dataSetSchemaService;
            _dataSetSchemaColumnService = dataSetSchemaColumnService;
        }
    
        public ImportDataSetSchemaResult ImportFromUrl(DataSetSchema schema)
        {
            var xml = _xmlToObjectService.GetXmlFromUrl(schema.SchemaDefinitionFromUrl);

            if (xml == "")
                return ReturnWithErrorMessage(schema, "Error loading schema from " + schema.SchemaDefinitionFromUrl);
            
            var deserializedSchema = _xmlToObjectService.ConvertXml<SchemaRestDefinition>(xml);

            

            if (deserializedSchema == null)
                return ReturnWithErrorMessage(schema, "No definition found at " + schema.SchemaDefinitionFromUrl);

            
        
            if (!String.IsNullOrEmpty(deserializedSchema.ErrorMessage))
                return ReturnWithErrorMessage(schema, deserializedSchema.ErrorMessage);
            
            if (deserializedSchema.RestSchema == null)
                return ReturnWithErrorMessage(schema, "No schema found at " + schema.SchemaDefinitionFromUrl);
            
            if (deserializedSchema.RestColumnDefinitions == null)
                return ReturnWithErrorMessage(schema, "No schema found at " + schema.SchemaDefinitionFromUrl);

            if (deserializedSchema.RestColumnDefinitions.ColumnDefinitions != null 
                &&  deserializedSchema.RestColumnDefinitions
                .ColumnDefinitions.Any(x => x.Name.ToLowerInvariant().Contains("publisher uri") 
                    || x.Name.ToLowerInvariant().Contains("publisheruri")
                    || x.Name.ToLowerInvariant().Contains("publisher label")
                    || x.Name.ToLowerInvariant().Contains("publisherlabel")))
            {
                return ReturnWithErrorMessage(schema, "Error: Columns cannot contain reserved column names - PublisherUri/PublisherLabel/Publisher Uri/Publisher Label. " + schema.SchemaDefinitionFromUrl);
            }

            schema = CreateTable(schema, deserializedSchema);
            
            //create each columns
            foreach (var col in deserializedSchema.RestColumnDefinitions.ColumnDefinitions)
            {

                var newCol = InitializeColumn(col);
                schema.Definition.Columns = schema.Definition.Columns ?? new List<DataSetSchemaColumn>();
                newCol.SchemaDefinition = schema.Definition;
                _dataSetSchemaColumnService.Create(newCol);
            }

            //populate esd links
            schema.CurrentMappedEsdFunctionService =
                deserializedSchema.EsdLinks.Links.Select(x => x.Type + x.Id.ToString()).ToList();
            //save esd links in the front mvc as this need the esd functions list. 

            return new ImportDataSetSchemaResult()
                {
                    DataSetSchema = schema,
                    ErrorMessage = ""
                };
            
        }

     
        private ImportDataSetSchemaResult ReturnWithErrorMessage(DataSetSchema schema, string message)
        {
            return new ImportDataSetSchemaResult()
                {
                    DataSetSchema = schema, 
                    ErrorMessage = message
                };
        }

        private DataSetSchema CreateTable(DataSetSchema schema, SchemaRestDefinition deserializedSchema)
        {
            if (deserializedSchema.RestSchema == null) return schema;
            
            var translatedSchema = new DataSetSchema()
            {
//#if ReleaseToLive
                Title =
                    deserializedSchema.RestSchema.Title,
//#else
                //Title =
                //    deserializedSchema.RestSchema.Title + "_debug_mode_test_" +
                //   DateTime.Now.ToString("HH_mm_ss"),
//#endif
                ShortDescription = deserializedSchema.RestSchema.ShortDescription,
                Description = deserializedSchema.RestSchema.Description,
                SchemaDefinitionFromUrl = String.IsNullOrEmpty(deserializedSchema.ConformsTo) ? schema.SchemaDefinitionFromUrl : deserializedSchema.ConformsTo,
                UploadFrequency = 0,
                IsStandardisedSchemaUrl = schema.IsStandardisedSchemaUrl,
                
                Category = schema.Category
            };


            _dataSetSchemaService.Create(translatedSchema);
            schema = translatedSchema;
            return schema;
        }

        private DataSetSchemaColumn InitializeColumn(RestColumnDefinition col)
        {
            var newCol = new DataSetSchemaColumn()
            {
                Title = col.Description,
                ColumnName = col.Name,
                Type = col.Type,
                IsRequired = col.IsRequired,
                HelpText = col.HelpText,
                LinkedDataUri = col.Uri ?? "",
                IsShownInitially = col.DisplayInitial,
                IsDefaultSort = col.Sorted,
                DefaultSortDirection = col.SortDirection,
                IsTotalisable = col.IsTotalled
            };

            switch (col.Type.ToLowerInvariant())
            {
                case "image":
                case "url":
                case "text":
                    newCol.MaxSize = Convert.ToInt32(col.MaxSize);
                    break;
                case "datetime":
                    DateTime outDate;
                    if (DateTime.TryParse(col.MinDate, out outDate)) newCol.MinDate = Convert.ToDateTime(col.MinDate);
                    if (DateTime.TryParse(col.MaxDate, out outDate)) newCol.MaxDate = Convert.ToDateTime(col.MaxDate);
                    newCol.MaxSize = 8;
                    break;
                case "currency":
                    double outDouble;

                    if (Double.TryParse(col.MaxCurrency, out outDouble)) newCol.MaxCurrency = Convert.ToDouble(col.MaxCurrency);
                    if (Double.TryParse(col.MinCurrency, out outDouble)) newCol.MinCurrency = Convert.ToDouble(col.MinCurrency);
                    newCol.MaxSize = 1;

                    break;
                case "number":
                    int outInt;

                    if (int.TryParse(col.MinNumber, out outInt)) newCol.MinNumber = Convert.ToInt32(col.MinNumber);
                    if (int.TryParse(col.MaxNumber, out outInt)) newCol.MaxNumber = Convert.ToInt32(col.MaxNumber);
                    newCol.MaxSize = 1;

                    break;
                
                default:
                    newCol.MaxSize = newCol.MaxSize <= 0 ? 1 : newCol.MaxSize; 
                    break;
            }
            return newCol;
        }
    }


}
