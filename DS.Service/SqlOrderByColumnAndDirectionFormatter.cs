using System;
using DS.Domain;
using DS.Domain.Interface;

namespace DS.Service
{
    public class SqlOrderByColumnAndDirectionFormatter : ISqlOrderByColumnAndDirectionFormatter
    {
        public string CheckOrderByDirection(DataSetSchema schema, string orderDirection)
        {
            if (!string.IsNullOrEmpty(orderDirection)) return orderDirection;

            if (schema != null && schema.Definition != null)
                return schema.Definition.DefaultSortColumnDirection;

            return "ASC";
        }

        public string CheckOrderByColumn(DataSetSchema schema, string orderByColumn)
        {
            if (string.IsNullOrEmpty(orderByColumn) && schema != null && schema.Definition != null)
                return schema.Definition.DefaultSortColumn;
            
            if (!string.IsNullOrEmpty(orderByColumn) && !orderByColumn.StartsWith("["))
                return String.Format("[{0}]", orderByColumn);
            
            return orderByColumn;
        }
    }
}