using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using DS.Domain;
using DS.Domain.Interface;

namespace DS.Service
{
    /// <summary>
    /// Example of extending the default IRepository methods
    /// </summary>
    public  class DataSetDetailService : IDataSetDetailService
    {
        private ISystemConfigurationService _systemConfigurationService;
        private IRepository<DataSetDetail> _repository;
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetDetailSqlRepo _dataSetDetailSqlRepo;
        private ISqlOrderByColumnAndDirectionFormatter _sqlOrderByTextFormatter;


        public DataSetDetailService(ISystemConfigurationService systemConfigurationService
            , IRepository<DataSetDetail> repository
            , IDataSetSchemaService dataSetSchemaService
            , IDataSetDetailSqlRepo dataSetDetailSqlRepo, ISqlOrderByColumnAndDirectionFormatter sqlOrderByTextFormatter)
        {
            _systemConfigurationService = systemConfigurationService;
            _repository = repository;
            _dataSetSchemaService = dataSetSchemaService;
            _dataSetDetailSqlRepo = dataSetDetailSqlRepo;
            _sqlOrderByTextFormatter = sqlOrderByTextFormatter;

        }
        
        


        public  DataSetDetail Get(int id)
        {
            return _repository.GetQuery().Include(d => d.Schema.Definition).Include(d => d.Schema.Category).FirstOrDefault(d => d.Id == id);
        }

        public  string GetTemplateCsv(int schemaId)
        {
            
            var schema = _dataSetSchemaService.Get(schemaId);
            
            return string.Join(",", (schema.Definition.Columns.Select(c => String.Format("\"{0}\"",c.ColumnName)).ToArray()));
        }

        public  DataTable GetData(string url, string schemaUrl)
        {
            var details = _repository.GetQuery().Include(d => d.Schema.Definition).Include(d => d.Schema.Definition.Columns).ToList().FirstOrDefault(c => c.Schema.FriendlyUrl == schemaUrl && c.FriendlyUrl == url);
            
            if (details == null) return null;

            return  _dataSetDetailSqlRepo.ExecuteQueryDatasetDetailId(_repository.DbConnectionString, details.Schema.Definition, details.Id).Tables[0];
            

        }

        public  ViewControllerData GetLatest(DataSetSchema schema)
        {
            var pageSize = _systemConfigurationService.AppSettingsInt("DefaultViewPageSize");

            return SearchSchema(null, schema, 1, pageSize, schema.Definition.DefaultSortColumn, schema.Definition.DefaultSortColumnDirection, true);

        }

        public  ViewControllerData SearchSchema(IList<FilterCriteria> filter,DataSetSchema schema, string orderByColumn, string orderDirection)
        {
            orderByColumn = _sqlOrderByTextFormatter.CheckOrderByColumn(schema, orderByColumn);

            orderDirection = _sqlOrderByTextFormatter.CheckOrderByDirection(schema, orderDirection);

            return SearchSchema(filter, schema, -1, -1, orderByColumn, orderDirection, false);
        }

        public  ViewControllerData SearchSchema(IList<FilterCriteria> filter, DataSetSchema schema, int currentPage, int pageSize, string orderByColumn, string orderDirection, bool getTotals)
        {

            orderByColumn = _sqlOrderByTextFormatter.CheckOrderByColumn(schema, orderByColumn);

            orderDirection = _sqlOrderByTextFormatter.CheckOrderByDirection(schema, orderDirection);

            var data = _dataSetDetailSqlRepo.ExecuteQuerySearchSchema(_repository.DbConnectionString,  filter,  schema,  currentPage,
                                                pageSize,  orderByColumn,  orderDirection);

            int count = Convert.ToInt32(data.Tables[0].Rows[0].ItemArray[0]);
            
            var totalPages = (count / pageSize);

            if ((count % pageSize) > 0)
                totalPages++;

            return new ViewControllerData
                             {
                                 Count = count,
                                 Data = data.Tables[1],
                                 Totals = data.Tables[2],
                                 CurrentPage = currentPage,
                                 TotalPages = totalPages == 0 ? 1 : totalPages
                             };
        }

        public  ViewControllerData VisualiseSchema(IList<FilterCriteria> filter, DataSetSchema schema, int pageNumber, int pageSize,string chartType,string xAxis, string yAxis, string yAxisAggregate)
        {
            if(String.IsNullOrEmpty(xAxis)) return new ViewControllerData();

            var criteria = _dataSetDetailSqlRepo.ConvertToVisualSchemaCriteria(_repository.DbConnectionString, filter, schema, chartType, xAxis, yAxis, yAxisAggregate, pageNumber, pageSize);
            var data = _dataSetDetailSqlRepo.ExecuteQueryVisualiseSchema(criteria);

            int count = Convert.ToInt32(data.Tables[1].Rows[0].ItemArray[0]);

            var totalPages = (count / pageSize);

            if ((count % pageSize) > 0)
                totalPages++;

            //Get Google Visualisation Json DataTable
            var dataTable = data.Tables[0];
            var dataGraph = new Bortosky.Google.Visualization.GoogleDataTable(dataTable);
            
            return new ViewControllerData
                       {
                           DataGraph = dataGraph.GetJson(),
                           CurrentPage = pageNumber,
                           TotalPages = totalPages == 0 ? 1 : totalPages,
                           HasNegativeValues = (Convert.ToInt32(data.Tables[2].Rows[0].ItemArray[0]) > 0)
                       };
        }

        public  ViewControllerData VisualiseSchemaAsMap(IList<FilterCriteria> filter, DataSetSchema schema)
        {
            
            var data = _dataSetDetailSqlRepo.ExecuteQueryVisualiseSchemaMap(_repository.DbConnectionString, filter,
                                                                            schema);
            return new ViewControllerData
            {
                Data = data.Tables[0],
                Count = Convert.ToInt32(data.Tables[1].Rows[0].ItemArray[0]),
                MapCentreLatitude = Convert.ToDouble(_systemConfigurationService.GetSystemConfigurations().MapCentreLatitude),
                MapCentreLongitude = Convert.ToDouble(_systemConfigurationService.GetSystemConfigurations().MapCentreLongitude),
                MapDefaultZoom = Convert.ToInt16(_systemConfigurationService.GetSystemConfigurations().MapDefaultZoom),
                SpatialGeographyUri = _systemConfigurationService.GetSystemConfigurations().CouncilSpatialUri

            };
        }

        public  IList<DataSetDetail> Search(int schemaId, string searchText, string from, string to)
        {

            var fromdate = from != "" ? Convert.ToDateTime(from) : (DateTime?)null;
            var todate = to != "" ? Convert.ToDateTime(to) : (DateTime?) null;

            searchText = searchText.Trim().ToLower();
            var results = _repository.Query(c => c.Schema.Id == schemaId && c.Title.Trim().ToLower().Contains(searchText));
            
            if (fromdate != null && todate != null && fromdate != DateTime.MinValue && todate != DateTime.MinValue )
                return results.Where(s => s.DateUpdated >= fromdate && s.DateUpdated <= todate).ToList();
            
            if (fromdate != null && fromdate != DateTime.MinValue)
                return results.Where(s => s.DateUpdated >= fromdate).ToList();
            
            if (todate != null && todate != DateTime.MinValue)
                return results.Where(s => s.DateUpdated <= todate).ToList();
            
            return results.ToList();
        }
        

    }
}
