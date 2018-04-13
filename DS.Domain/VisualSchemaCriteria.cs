using System.Collections.Generic;

namespace DS.Domain
{
    public class VisualSchemaCriteria
    {
        private string _dbConnectionString;
        private IEnumerable<FilterCriteria> _filters;
        private DataSetSchema _schema;
        private string _chartType;
        private string _xAxisColumn;
        private string _xAxisType;
        private string _xAxisDateFormat;
        private string _yAxisColumn;
        private string _yAxisAggregate;
        private int _pageNum;
        private int _pageSize;

        public VisualSchemaCriteria(string dbConnectionString, IEnumerable<FilterCriteria> filters, DataSetSchema schema, string chartType, string xAxisColumn, string xAxisType, string xAxisDateFormat, string yAxisColumn, string yAxisAggregate, int pageNum, int pageSize)
        {
            _dbConnectionString = dbConnectionString;
            _filters = filters;
            _schema = schema;
            _chartType = chartType;
            _xAxisColumn = xAxisColumn;
            _xAxisType = xAxisType;
            _xAxisDateFormat = xAxisDateFormat;
            _yAxisColumn = yAxisColumn;
            _yAxisAggregate = yAxisAggregate;
            _pageNum = pageNum;
            _pageSize = pageSize;
        }

        public string DbConnectionString
        {
            get { return _dbConnectionString; }
        }

        public IEnumerable<FilterCriteria> Filters
        {
            get { return _filters; }
        }

        public DataSetSchema Schema
        {
            get { return _schema; }
        }

        public string ChartType
        {
            get { return _chartType; }
        }

        public string XAxisColumn
        {
            get { return _xAxisColumn; }
        }

        public string XAxisType
        {
            get { return _xAxisType; }
        }

        public string XAxisDateFormat
        {
            get { return _xAxisDateFormat; }
        }

        public string YAxisColumn
        {
            get { return _yAxisColumn; }
        }

        public string YAxisAggregate
        {
            get { return _yAxisAggregate; }
        }

        public int PageNum
        {
            get { return _pageNum; }
        }

        public int PageSize
        {
            get { return _pageSize; }
        }
    }
}