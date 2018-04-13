using System;
using System.Collections.Generic;
using System.Data;

namespace DS.Domain.Interface
{
    public interface IDataSetDetailService
    {
        
        DataSetDetail Get(int id);

        
        string GetTemplateCsv(int schemaId);
        
        
        DataTable GetData(string url, string schemaUrl);
        ViewControllerData GetLatest(DataSetSchema schema);
        ViewControllerData SearchSchema(IList<FilterCriteria> filter,DataSetSchema schema, string orderByColumn, string orderDirection);
        ViewControllerData SearchSchema(IList<FilterCriteria> filter, DataSetSchema schema, int currentPage, int pageSize, string orderByColumn, string orderDirection, bool getTotals);
        ViewControllerData VisualiseSchema(IList<FilterCriteria> filter, DataSetSchema schema, int pageNumber, int pageSize,string chartType,string xAxis, string yAxis, string yAxisAggregate);
        ViewControllerData VisualiseSchemaAsMap(IList<FilterCriteria> filter, DataSetSchema schema);
        IList<DataSetDetail> Search(int schemaId, string searchText, string from, string to);
        
    }
}