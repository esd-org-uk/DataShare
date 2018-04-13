using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DS.Domain;
using DS.Domain.Interface;
using DS.Extensions;

namespace DS.Service
{
    public class DataSetDetailSqlRepo : IDataSetDetailSqlRepo
    {
        public DataSet ExecuteQueryDatasetDetailId(string connectionString, DataSetSchemaDefinition dataSetSchemaDefinition, int datasetdetailId)
        {
            var sql = String.Format("SELECT {0} FROM {1} WHERE DatasetDetailId = @datasetDetailId",
                                    dataSetSchemaDefinition.ColumnSelectList,
                                    dataSetSchemaDefinition.TableName);
            var parameters = new List<SqlParameter>
                                 {
                                     new SqlParameter
                                         {
                                             ParameterName = "@datasetDetailId",
                                             SqlDbType = SqlDbType.Int,
                                             Direction = ParameterDirection.Input,
                                             Value = datasetdetailId
                                         }
                                 };

            var data = SqlExtensions.ExecuteQuery(connectionString, sql, parameters.ToArray());
            return data;
        }

        public DataSet ExecuteQuerySearchSchema(string connectionString, IList<FilterCriteria> filter, DataSetSchema schema, int currentPage,
                                                int pageSize, string orderByColumn, string orderDirection)
        {
            var query = BuildSqlQuery(filter, schema, orderByColumn, orderDirection, currentPage, pageSize);

            return SqlExtensions.ExecuteQuery(connectionString, query.Sql, query.Params);
        }

        public DataSet ExecuteQuery(string connectionString, string sql, Array parameters)
        {
            return SqlExtensions.ExecuteQuery(connectionString, sql, parameters);
        }

        public bool SaveToDatabase(DataSetSchemaDefinition schemaDef, UploadResult result, string connectionString)
        {
            if (result.Errors.Any()) return false;

            using (var cn = new SqlConnection(connectionString))//ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString
            {
                cn.Open();
                using (var copy = new SqlBulkCopy(cn))
                {
                    copy.DestinationTableName = schemaDef.TableName;
                    foreach (DataColumn col in result.Data.Columns)
                    {
                        copy.ColumnMappings.Add(col.ColumnName, result.Data.Columns[col.ColumnName].ColumnName);
                    }
                    copy.WriteToServer(result.Data);
                }
            }
            return true;
        }

        public DataSet ExecuteQueryVisualiseSchemaMap(string connectionString, IEnumerable<FilterCriteria> filters, DataSetSchema schema)
        {
            var queryExecute = BuildVisualiseMapSqlQuery(filters, schema);
            return SqlExtensions.ExecuteQuery(connectionString, queryExecute.Sql, queryExecute.Params);
        }

        public DataSet ExecuteQueryVisualiseSchema(VisualSchemaCriteria criteria)
        {
            var queryExecute = BuildVisualiseSqlQuery(criteria.Filters, criteria.Schema, criteria.ChartType, criteria.XAxisColumn, criteria.XAxisType, criteria.XAxisDateFormat, criteria.YAxisColumn, criteria.YAxisAggregate, criteria.PageNum, criteria.PageSize);
            return SqlExtensions.ExecuteQuery(criteria.DbConnectionString, queryExecute.Sql, queryExecute.Params);
        }

        public VisualSchemaCriteria ConvertToVisualSchemaCriteria(string dbConnectionString, IList<FilterCriteria> filter, DataSetSchema schema,
                                                                  string chartType, string xAxis, string yAxis, string yAxisAggregate,
                                                                  int pageNumber, int pageSize)
        {

            var xAxisDetail = xAxis.Split('#');

            var xAxisType = xAxisDetail[0];
            xAxis = xAxisDetail.Length > 1 ? xAxisDetail[1] : "";

            var yAxisDetail = yAxis != null ? yAxis.Split('#') : new string[0];
            yAxis = yAxisDetail.Length > 1 ? yAxisDetail[1] : "";

            var xAxisDateFormat = xAxisDetail.Length == 3 ? xAxisDetail[2] : "";
            return new VisualSchemaCriteria(dbConnectionString, filter, schema, chartType, xAxis, xAxisType, xAxisDateFormat, yAxis, yAxisAggregate, pageNumber, pageSize);
        }

        

        private SqlQuery BuildSqlQuery(IEnumerable<FilterCriteria> filters, DataSetSchema schema, string orderByColumn, string orderDirection,
                                      int pageNum, int pageSize)
        {
            orderDirection = orderDirection == "ASC" ? "ASC" : "DESC";

            var queryDetail = new SqlQuery();
            var parameters = new List<SqlParameter>
                                 {
                                     new SqlParameter
                                         {
                                             ParameterName = "@pageNum",
                                             SqlDbType = SqlDbType.Int,
                                             Direction = ParameterDirection.Input,
                                             Value = pageNum
                                         },
                                     new SqlParameter
                                         {
                                            ParameterName = "@pageSize",
                                             SqlDbType = SqlDbType.Int,
                                             Direction = ParameterDirection.Input,
                                             Value = pageSize
                                        }
                                 };

            //get where clause
            var whereClause = GetWhereClause(filters, schema, parameters);

            var orderBy = schema.Definition.ColumnSelectList.Contains(orderByColumn) ? orderByColumn : schema.Definition.DefaultSortColumn;
            var orderByDirection = orderDirection.ToUpper() == "ASC" ? "ASC" : "DESC";
            var sqlResults = String.Format(@"DECLARE @lbound int,@ubound int,@recct int 
                                            SET @pageNum = ABS(@pageNum)
                                            SET @pageSize = ABS(@pageSize)
                                            IF @pageNum < 1 SET @pageNum = 1
                                            IF @pageSize < 1 SET @pageSize = 1
                                            SET @lbound = ((@pageNum - 1) * @pageSize)
                                            SET @ubound = @lbound + @pageSize + 1
                                            IF @lbound >= @recct BEGIN
                                              SET @ubound = @recct + 1
                                              SET @lbound = @ubound - (@pageSize + 1) 
                                            END
                                            SELECT {0} FROM(SELECT ROW_NUMBER() OVER(ORDER BY {4} {2}) AS row, * 
                                            FROM {1}{3}) AS tbl {5}",
                                              schema.Definition.ColumnSelectList,
                                              schema.Definition.TableName,
                                              orderByDirection,
                                              whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                              orderBy,
                                              pageSize > 0 ? "WHERE row > @lbound AND row <  @ubound" : String.Format("ORDER BY {0} {1}", orderBy, orderByDirection));

            var sqlCount = String.Format(@"SELECT COUNT(*) FROM {0} {1}", schema.Definition.TableName, whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "");
            var sqlTotals = String.Format(@"SELECT Top 1 {0} FROM {1} {2}", schema.Definition.ColumnSumSql, schema.Definition.TableName, whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "");

            queryDetail.Sql = String.Format(@"{0} {1} {2}", sqlCount, sqlResults, sqlTotals);
            queryDetail.Params = parameters.ToArray();

            return queryDetail;
        }

        private SqlQuery BuildVisualiseMapSqlQuery(IEnumerable<FilterCriteria> filters, DataSetSchema schema)
        {
            if (schema == null) return new SqlQuery();
            /************************************************************************************
                Map data just need to return list of lat/lng
            ************************************************************************************/
            var queryDetail = new SqlQuery();
            var parameters = new List<SqlParameter>();
            var selectList = new StringBuilder();
            var latitude = "";
            var longitude = "";
            foreach (var c in schema.Definition.Columns)
            {
                if (c.ColumnName.Contains("Latitude"))
                {
                    latitude = String.Format("{0} as Latitude", c.ColumnName);
                }
                else if (c.ColumnName.Contains("Longitude"))
                {
                    longitude = String.Format("{0} as Longitude", c.ColumnName);
                }
                else
                {
                    selectList.Append(String.Format("cast([{0}] as nvarchar(255)) +','+", c.ColumnName));
                }
            }
            var selectSql = selectList.ToString().Substring(0, selectList.ToString().Length - 5);
            selectSql = String.Format("{0} as Title,{1},{2}", selectSql, latitude, longitude);

            var whereClause = GetWhereClause(filters, schema, parameters);

            var sqlResults = String.Format(@"SELECT {1} FROM {0} {2}",
                                                schema.Definition.TableName,
                                                selectSql,
                                                whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "");
            var sqlCount = String.Format(@"SELECT count(*) FROM {0} {1}",
                                                schema.Definition.TableName,
                                                whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "");

            queryDetail.Sql = String.Format(@"{0} {1}", sqlResults, sqlCount);
            queryDetail.Params = parameters.ToArray();

            return queryDetail;

        }

        private SqlQuery BuildVisualiseSqlQuery(IEnumerable<FilterCriteria> filters, DataSetSchema schema, string chartType, string xAxisColumn, string xAxisType, string xAxisDateFormat, string yAxisColumn, string yAxisAggregate, int pageNum, int pageSize)
        {
            if (schema == null) return new SqlQuery();
            var queryDetail = new SqlQuery();

            var sqlNegativeValues = String.Format(@"SELECT 0 as LessThanZeroCount"); //Ignore negative values check by default

            var parameters = new List<SqlParameter>
                                 {
                                     new SqlParameter
                                         {
                                             ParameterName = "@pageNum",
                                             SqlDbType = SqlDbType.Int,
                                             Direction = ParameterDirection.Input,
                                             Value = pageNum
                                         },
                                     new SqlParameter
                                         {
                                            ParameterName = "@pageSize",
                                             SqlDbType = SqlDbType.Int,
                                             Direction = ParameterDirection.Input,
                                             Value = pageSize
                                        }
                                 };
            var whereClause = GetWhereClause(filters, schema, parameters);

            /************************************************************************************
                Perform grouping and paging queries
            ************************************************************************************/
            //Sql injection attack check, column must exist in schema
            var xAxis = (schema.Definition.Column(xAxisColumn) != null
                             ? xAxisColumn
                             : schema.Definition.Columns[0].ColumnName);

            var yAxis = yAxisColumn != "" ? (schema.Definition.Column(yAxisColumn) != null
                            ? yAxisColumn
                            : schema.Definition.TotalisableColumns[0].ColumnName) : "";

            var aggregate = (yAxisAggregate.ToUpper() == "SUM" || yAxisAggregate.ToUpper() == "COUNT" || yAxisAggregate.ToUpper() == "AVG")
                            ? yAxisAggregate.ToUpper()
                            : "SUM";

            string sqlBody;
            string groupOrderByClause;

            //handle no yAxis eg: no number fields in the dataset
            aggregate = yAxis != "" ? String.Format("{0}({1})", aggregate, yAxis) : String.Format("COUNT({0})", xAxis); //if no yAxis, Count xAxis is the only option
            yAxis = yAxis != "" ? yAxis : "NumberOf"; //if no yAxis use xAxis

            if (xAxisType == "DateTime")//DateTime queries
            {
                switch (xAxisDateFormat)
                {
                    case "Year":
                        groupOrderByClause = String.Format(@"YEAR({0})", xAxis);
                        sqlBody = String.Format(@"SELECT Cast(tbl.{1} as nvarchar(255)) as {1},tbl.{3} FROM (SELECT YEAR({1}) as {1}, {2} as {3},
                                                  ROW_NUMBER() OVER (ORDER BY {6} ASC) AS row_number
                                                  FROM {0} {4}
                                                  GROUP BY {6}) as tbl {5}",
                                                  schema.Definition.TableName,
                                                  xAxis,
                                                  aggregate,
                                                  yAxis,
                                                  whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                                  pageSize > 0 ? "WHERE row_number > @lbound AND row_number < @ubound" : "",
                                                  groupOrderByClause);
                        break;
                    case "Month":
                        groupOrderByClause = String.Format(@"YEAR({0}),MONTH({0})", xAxis);
                        sqlBody = String.Format(@"SELECT DATENAME(month,dateadd(month, tbl.Month - 1, 0))+ ' ' + cast(tbl.Year as nvarchar(4)) as {1}, tbl.{3} from (SELECT MONTH({1}) as Month,YEAR({1}) as Year, {2} as {3},
                                                  ROW_NUMBER() OVER (ORDER BY {6} ASC) AS row_number
                                                  FROM {0} {4}
                                                  GROUP BY {6}) as tbl {5}",
                                                  schema.Definition.TableName,
                                                  xAxis,
                                                  aggregate,
                                                  yAxis,
                                                  whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                                  pageSize > 0 ? "WHERE row_number > @lbound AND row_number < @ubound" : "",
                                                  groupOrderByClause);
                        break;
                    default:
                        groupOrderByClause = String.Format(@"{0}", xAxis);
                        sqlBody = String.Format(@"SELECT CONVERT(CHAR(11), tbl.{1} , 106) as {1}, tbl.{3} from (SELECT {1}, {2} as {3},
                                                  ROW_NUMBER() OVER (ORDER BY {6} ASC) AS row_number
                                                  FROM {0} {4}
                                                  GROUP BY {6}) as tbl {5}",
                                                    schema.Definition.TableName,
                                                    xAxis,
                                                    aggregate,
                                                    yAxis,
                                                    whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                                    pageSize > 0 ? "WHERE row_number > @lbound AND row_number < @ubound" : "",
                                                    groupOrderByClause);
                        break;
                }
            }
            else //Default query 
            {
                groupOrderByClause = String.Format(@"{0}", xAxis);
                sqlBody = String.Format(@"SELECT Cast(tbl.{1} as nvarchar(255)) as {1}, tbl.{3} FROM (SELECT {1} as {1}, {2} as {3},
                                                  ROW_NUMBER() OVER (ORDER BY {6} ASC) AS row_number
                                                  FROM {0} {4}
                                                  GROUP BY {6}) as tbl {5}",
                                                  schema.Definition.TableName,
                                                  xAxis,
                                                  aggregate,
                                                  yAxis,
                                                  whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                                  pageSize > 0 ? "WHERE row_number > @lbound AND row_number < @ubound" : "",
                                                  groupOrderByClause);
            }

            var sqlCount = String.Format(@"SELECT COUNT(tbl.Counters) from (SELECT COUNT(*) as Counters FROM {0} {1} GROUP BY {2}) as tbl",
                                            schema.Definition.TableName,
                                            whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                            groupOrderByClause);

            if (chartType.ToLower() == "pie" & (aggregate.Contains("SUM(") || aggregate.Contains("AVG("))) //Pie chart SUM and AVG need to check for negative values as the pie chart wont work
            {
                sqlNegativeValues = String.Format(@"SELECT COUNT(tbl.Total) AS LessThanZeroCount FROM (SELECT {3} as Total FROM {0} {1} group by {2}) as tbl
                                                   WHERE tbl.Total < 0",
                                                   schema.Definition.TableName,
                                                    whereClause.Length > 0 ? String.Format(" Where {0}", whereClause) : "",
                                                   groupOrderByClause,
                                                   aggregate);
            }

            var sqlResults = String.Format(@"DECLARE @lbound int,@ubound int,@recct int 
                                            SET @pageNum = ABS(@pageNum)
                                            SET @pageSize = ABS(@pageSize)
                                            IF @pageNum < 1 SET @pageNum = 1
                                            IF @pageSize < 1 SET @pageSize = 1
                                            SET @lbound = ((@pageNum - 1) * @pageSize)
                                            SET @ubound = @lbound + @pageSize + 1
                                            IF @lbound >= @recct BEGIN
                                              SET @ubound = @recct + 1
                                              SET @lbound = @ubound - (@pageSize + 1) 
                                            END
                                            {0}", sqlBody);

            queryDetail.Sql = String.Format(@"{0} {1} {2}", sqlResults, sqlCount, sqlNegativeValues);
            queryDetail.Params = parameters.ToArray();

            return queryDetail;
        }

        private string GetWhereClause(IEnumerable<FilterCriteria> filters, DataSetSchema schema, ICollection<SqlParameter> parameters)
        {
            if (filters != null)
            {
                var whereClause = new StringBuilder();
                var i = 0;

                foreach (var filter in filters)
                {
                    if (filter != null && filter.ColumnToSearch != null)
                    {
                        var detail = new SqlFilter(filter, schema.Definition.ColumnSelectList, i);
                        if (detail.FilterSet)
                        {
                            whereClause.Append(String.Format("{0}", detail.Where));
                            parameters.Add(detail.Param);
                            if (detail.Param1 != null)
                                parameters.Add(detail.Param1);

                            whereClause.Append(" and ");
                        }
                    }
                    i++;
                }

                //remove last "and" statement
                return whereClause.ToString().Length > 5 ? whereClause.ToString().Substring(0, whereClause.ToString().Length - 5) : "";
            }
            return "";
        }

        
    }
}
