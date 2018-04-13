using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using StructureMap;

namespace DS.WebUI.Controllers.Extensions
{
    public static class DataTableExtensions
    {
        //private static SystemConfigurationService _systemConfiguration = new SystemConfigurationService(
        //        ObjectFactory.GetInstance<IRepository<SystemConfigurationObject>>()
        //        , new HttpCache());

        public static string ToCsv(this DataTable table, string seperatorChar)
        {
            return ToCsv(table, seperatorChar, null);
        }

        public static string ToCsv(this DataTable table, string seperatorChar, IList<DataSetSchemaColumn> columns)
        {
            try
            {
                var seperator = "";
                var builder = new StringBuilder();
                if (columns != null)
                {
                    //prepend body and bodyname column headings
                    builder.Append("PublisherLabel");
                    seperator = seperatorChar;
                    builder.Append(seperator).Append("PublisherURI");
                    foreach (var col in columns)
                    {
                        builder.Append(seperator).Append(String.Format(@"""{0}""", col.Title));
                        if(!string.IsNullOrEmpty(col.LinkedDataUri))//This columns is linked data
                        {
                            builder.Append(seperator).Append(String.Format(@"""{0} Uri""", col.Title));
                        }
                    }
                }
                else
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        builder.Append(seperator).Append(String.Format(@"""{0}""", col.ColumnName));
                        seperator = seperatorChar;
                    }
                }
                builder.Append("\n");
                foreach (DataRow row in table.Rows)
                {
                    seperator = "";
                    foreach (DataColumn col in table.Columns)
                    {
                        var columnName = col.ColumnName;
                        var colDef = columns != null ? columns.FirstOrDefault(c => c.ColumnName == columnName) : null;

                        switch (col.DataType.FullName)
                        {
                            case "System.String":
                                builder.Append(seperator).Append(String.Format(@"""{0}""", row[columnName]));
                                break;
                            default:
                                builder.Append(seperator).Append(row[columnName]);
                                break;
                        }

                        if (colDef != null && !string.IsNullOrEmpty(colDef.LinkedDataUri))
                        {
                            //This columns is linked data so add uri
                            var colValue = row[columnName].ToString();
                            if(colValue.Length > 0)
                            {
                                var uri = colDef.LinkedDataUri.Replace("#Data#", colValue);
                                builder.Append(seperator).Append(String.Format(@"""{0}""", uri));
                            }
                            else
                            {
                                builder.Append(seperator).Append("");
                            }
                        }
                        seperator = seperatorChar;
                    }
                    builder.Append("\n");
                }

                return builder.ToString();

            }
            catch (Exception e)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(e));
                return "Error";
            }
        }


        public static XmlDocument ToXml(this DataTable table, IList<DataSetSchemaColumn> columns)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                var root = xmlDoc.CreateElement("Root");
                
                foreach (DataRow row in table.Rows)
                {
                    var rowXml = xmlDoc.CreateElement("Row");
                    foreach (DataColumn col in table.Columns)
                    {
                        var columnName = col.ColumnName;
                        var colDef = columns != null ? columns.FirstOrDefault(c => c.ColumnName == columnName) : null;
                        var colValue = row[columnName].ToString();

                        var colXml = xmlDoc.CreateElement(columnName);
                        colXml.InnerXml = HttpUtility.HtmlEncode(colValue);
                        rowXml.AppendChild(colXml);

                        if (colDef == null || string.IsNullOrEmpty(colDef.LinkedDataUri)) continue;

                        //This columns is linked data so add uri
                        var colUriXml = xmlDoc.CreateElement(columnName + "Uri");
                        if (colValue.Length > 0)
                        {
                            var uri = colDef.LinkedDataUri.Replace("#Data#", colValue);
                            colUriXml.InnerXml = uri;
                        }
                        rowXml.AppendChild(colUriXml);
                    }
                    root.AppendChild(rowXml);
                }
                xmlDoc.AppendChild(root);
                return xmlDoc;

            }
            catch (Exception e)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(e));
                var error = new XmlDocument();
                error.LoadXml("<Error></Error>");
                return error;
            }
        }

        public static DataTable AppendBody(this DataTable table, ISystemConfigurationService systemConfigurationService)
        {
            var bodyName = String.Format("'{0}'", systemConfigurationService.GetSystemConfigurations().CouncilName);
            var bodyUri = String.Format("'{0}'", systemConfigurationService.GetSystemConfigurations().CouncilUri);
            table = table ?? new DataTable();
            var colBody = table.Columns.Add("PublisherLabel", typeof(String), bodyName);
            colBody.SetOrdinal(0);
            var colUri = table.Columns.Add("PublisherURI", typeof(String), bodyUri);
            colUri.SetOrdinal(1);
            
            return table;
        }
    }
}
