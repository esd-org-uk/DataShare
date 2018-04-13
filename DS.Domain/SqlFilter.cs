using System;
using System.Data;
using System.Data.SqlClient;

namespace DS.Domain
{
    public class SqlFilter
    {
        public string Where { get; set; }
        public SqlParameter Param { get; set; }
        public SqlParameter Param1 { get; set; }
        public bool FilterSet { get; set; }
        public int FilterIndex { get; set; }

        public SqlFilter(FilterCriteria filter,string validColumns, int index)
        {
            FilterSet = false;
            FilterIndex = index;
            if (!validColumns.ToLower().Contains(filter.ColumnToSearch.ToLower().Trim()))//Handle sql injection of the column to search name
                return;

            if (!string.IsNullOrEmpty(filter.From) || !string.IsNullOrEmpty(filter.To))//date search, will need to add 2 params
            {
                DateFilter(filter);
                FilterSet = true;
            }
            else if (!string.IsNullOrEmpty(filter.SearchOperatorNumber) && !string.IsNullOrEmpty(filter.SearchNumber))//number search
            {
                NumberFilter(filter);
                FilterSet = true;
            }

            else if (!string.IsNullOrEmpty(filter.SearchText))//text search
            {
                TextFilter(filter);
                FilterSet = true;
            }
        }

        private void TextFilter(FilterCriteria filter)
        {
            var paramName = String.Format("@{0}{1}", filter.ColumnToSearch, FilterIndex);
            var param = new SqlParameter
            {
                ParameterName = paramName,
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar
            };

            switch (filter.SearchOperator)
            {
                case "contains":
                    param.Value = String.Format("%{0}%", filter.SearchText.ToLower());
                    Where = String.Format("lower([{0}]) like {1}", filter.ColumnToSearch, paramName);
                    break;
                case "isequalto":
                    param.Value = filter.SearchText.ToLower();
                    Where = String.Format("lower([{0}]) = {1}", filter.ColumnToSearch, paramName);
                    break;
            }
            Param = param;
        }


        private void NumberFilter(FilterCriteria filter)
        {
            var paramName = String.Format("@{0}{1}",filter.ColumnToSearch,FilterIndex);
            var param = new SqlParameter
            {
                ParameterName = paramName,
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Float,
                Value = filter.SearchNumber,
                Size = filter.SearchNumber.Length
            };
            switch (filter.SearchOperatorNumber)
            {
                case "isequalto":
                    Where = String.Format("[{0}] = {1}",filter.ColumnToSearch,paramName);
                    break;
                case "greaterthan":
                    Where = String.Format("[{0}] > {1}", filter.ColumnToSearch, paramName);
                    break;
                case "greaterthanequalto":
                    Where = String.Format("[{0}] >= {1}", filter.ColumnToSearch, paramName);
                    break;
                case "lessthan":
                    Where = String.Format("[{0}] < {1}", filter.ColumnToSearch, paramName);
                    break;
                case "lessthanequalto":
                    Where = String.Format("[{0}] <= {1}", filter.ColumnToSearch, paramName);
                    break;
            }
            Param = param;
        }

        private void DateFilter(FilterCriteria filter)
        {
            if (!string.IsNullOrEmpty(filter.From))//date search from
            {
                var paraFromName = String.Format("@{0}From{1}", filter.ColumnToSearch, FilterIndex);
                var paramFrom = new SqlParameter
                {
                    ParameterName = paraFromName,
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.DateTime,
                    Value = Convert.ToDateTime(String.Format("{0} 00:00:00",filter.From))
                };
                Param = paramFrom;
                Where = String.Format("[{0}] >= {1} {2} ", filter.ColumnToSearch, paraFromName, !string.IsNullOrEmpty(filter.To) ? "and" : "");
            }

            if (string.IsNullOrEmpty(filter.To)) return;

            var paraToName = String.Format("@{0}To{1}", filter.ColumnToSearch, FilterIndex);
            var paramTo = new SqlParameter
                              {
                                  ParameterName = paraToName,
                                  Direction = ParameterDirection.Input,
                                  SqlDbType = SqlDbType.DateTime,
                                  Value = Convert.ToDateTime(String.Format("{0} 23:59:59", filter.To))
                              };
            Param1 = paramTo;
            Where += String.Format("[{0}] <= {1}", filter.ColumnToSearch, paraToName);
        }
    }
}
