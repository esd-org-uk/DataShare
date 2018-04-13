using System;
using System.Collections.Generic;
using System.Data;

namespace DS.Domain.Interface
{
    public interface IDataSetDetailSqlRepo
    {
        DataSet ExecuteQueryDatasetDetailId(string connectionString, DataSetSchemaDefinition dataSetSchemaDefinition, int datasetdetailId);

        DataSet ExecuteQuerySearchSchema(string connectionString, IList<FilterCriteria> filter, DataSetSchema schema, int currentPage,
                                         int pageSize, string orderByColumn, string orderDirection);

        DataSet ExecuteQuery(string connectionString, string sql, Array parameters);

        bool SaveToDatabase(DataSetSchemaDefinition schemaDef, UploadResult result, string connectionString);

        DataSet ExecuteQueryVisualiseSchemaMap(string connectionString, IEnumerable<FilterCriteria> filters,
                                               DataSetSchema schema);

        DataSet ExecuteQueryVisualiseSchema(VisualSchemaCriteria criteria);

        VisualSchemaCriteria ConvertToVisualSchemaCriteria(string dbConnectionString, IList<FilterCriteria> filter, DataSetSchema schema, string chartType, string xAxis, string yAxis, string yAxisAggregate, int pageNumber, int pageSize);
    }
}