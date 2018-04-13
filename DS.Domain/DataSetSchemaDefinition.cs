using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using DS.Domain.Base;

namespace DS.Domain
{
    [Table("DS_DataSetSchemaDefinition")]
    public class DataSetSchemaDefinition : TrackChanges
    {
        [MaxLength(255)]
        public string TableName { get; set; }
        public List<DataSetSchemaColumn> Columns { get; set; }
        public string DefaultSortDirection { get; set; }

        [NotMapped]
        public List<DataSetSchemaColumn> SortedColumns {
            get { return Columns.OrderBy(c => c.Title).ToList(); } 
        }

        [NotMapped]
        public List<DataSetSchemaColumn> InitialColumns
        {
            get { return Columns.Where(c => c.IsShownInitially).ToList(); }
        }

        [NotMapped]
        public List<DataSetSchemaColumn> NonNumuericColumns
        {
            get { return Columns.Where(c => !c.IsNumeric).ToList(); }
        }

        [NotMapped]
        public List<DataSetSchemaColumn> NumuericColumns
        {
            get { return Columns.Where(c => c.IsNumeric).ToList(); }
        }

        [NotMapped]
        public string InitialVisibleColumnIndexes
        {
            get
            {
                var indexList = new List<string>();
                var index = 0;
                foreach (var c in Columns)
                {
                    if (c.IsShownInitially)
                    {
                        indexList.Add(index.ToString());
                    }
                    index++;
                }
                return string.Join(",", indexList);
            }
        }

        [NotMapped]
        public string ColumnSelectList
        {
            get
            {
                return string.Join(",", (Columns.Select(c => String.Format("[{0}]",c.ColumnName)).ToArray()));
            }
        }

        [NotMapped]
        public string ColumnSumSql
        {
            get
            {
                return string.Join(",", (Columns.Select(c =>c.IsTotalisable? String.Format("sum([{0}]) as [{0}]", c.ColumnName): String.Format("null as [{0}]", c.ColumnName)).ToArray()));
            }
        }

        [NotMapped]
        public bool HasLatLngColumns
        {
            get
            {
                var latLngCols = Columns.Where(c => c.ColumnName.Contains("Latitude") || c.ColumnName.Contains("Longitude"));
                return latLngCols.Count() >= 2;
            }
        }

        [NotMapped]
        public string DefaultSortColumn
        {
            get
            {
                var defaultSort = (Columns ?? new List<DataSetSchemaColumn>()).FirstOrDefault(c => c.IsDefaultSort);
                
                return defaultSort != null ? string.Format("[{0}]", defaultSort.ColumnName) : String.Format("[{0}]", Columns.First().ColumnName);
            }
        }

        [NotMapped]
        public string DefaultSortColumnDirection
        {
            get
            {
                var defaultSort = (Columns ?? new List<DataSetSchemaColumn>()).FirstOrDefault(c => c.IsDefaultSort);
                return defaultSort != null ? defaultSort.DefaultSortDirection : "ASC";
            }
        }

        public DataSetSchemaColumn Column(string columnName)
        {
            return Columns.FirstOrDefault(c => c.ColumnName.Replace(" ", "").ToLower() == columnName.Replace(" ", "").ToLower());
        }

        [NotMapped]
        public List<DataSetSchemaColumn> TotalisableColumns
        {
            get { return Columns.Where(c => c.IsTotalisable).ToList(); }
        }

        [NotMapped]
        public string AsString
        {
              get
              {
                  var def = new StringBuilder();
                  foreach (var c in Columns)
                  {
                      def.Append(String.Format("Column title: {1}{0}Type:{2}{0}Is required: {3} {0}{4}{5}{6}{7}{8}{9}{0}{0}", Environment.NewLine, c.ColumnName, c.Type == "DateTime" ? String.Format("{0} eg: (21/02/2001 or 21/02/2001 23:21:59)", c.Type) : c.Type, c.IsRequired,
                          c.MinNumber != null ? String.Format("Minimum number: {0}{1}", c.MinNumber, Environment.NewLine) : "",
                          c.MaxNumber != null ? String.Format("Maximum number: {0}{1}", c.MaxNumber, Environment.NewLine) : "",
                          c.MinCurrency != null ? String.Format("Mininimum currency: {0}{1}", c.MinCurrency, Environment.NewLine) : "",
                          c.MaxCurrency != null ? String.Format("Maximum currency: {0}{1}", c.MaxCurrency, Environment.NewLine) : "",
                          c.MinDate != null ? String.Format("Minimum date: {0}{1}", c.MinDate, Environment.NewLine) : "",
                          c.MinNumber != null ? String.Format("Maximum date: {0}{1}", c.MaxDate, Environment.NewLine) : ""));
                  }
                  return def.ToString();
              }
        }
    }
}
