using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;
using DS.Domain;
using DS.Domain.Interface;

namespace DS.WebUI.Controllers.Extensions
{
    public static class ControllerExtenstions
    {
        public static string XslFileTransform(this Controller controller, string xmlData, string xslFilePath)
        {
            var xslTransform = new XslCompiledTransform();
            xslTransform.Load(xslFilePath);
            var sw = new StringWriter();
            var xmlWriter = new XmlTextWriter(sw);
            var settings = new XmlReaderSettings { IgnoreWhitespace = true, ProhibitDtd = false };
            var xmlReader = XmlReader.Create(new StringReader(xmlData), settings);

            xslTransform.Transform(xmlReader, null, xmlWriter, null);
            xmlWriter.Flush();
            xmlWriter.Close();

            return sw.ToString();
        }

        public static ActionResult DownloadDataSet(this Controller controller, string downloadFormat, DataTable data, string filename, IList<DataSetSchemaColumn> columns, ISystemConfigurationService systemConfigurationService)
        {
            data.AppendBody(systemConfigurationService);
            switch (downloadFormat.ToLower())
            {
                case "xml":
                    var xml = data.ToXml(columns);
                    return new XmlResult(xml, filename);
                default:
                    var csv = columns != null ? data.ToCsv(",",columns) : data.ToCsv(",");
                    return new CsvResult(csv, filename);
            }
        }

        //private static XElement ConvertToRdf(XElement xmlfile)
        //{
        //    var urlNode = new XElement("Url") { Value = HttpContext.Current.Request.RawUrl };
        //    xmlfile.Add(urlNode);

        //    var doc = new XmlDocument();
        //    doc.LoadXml(xmlfile.ToString());
        //    return XElement.Parse(ApplyXslt(doc, "rdf.xslt"));
        //}

        //private static string ApplyXslt(XmlDocument xml, string xsltFileName)
        //{
        //    var sw = new StringWriter();
        //    var xw = new XmlTextWriter(sw);
        //    xml.WriteTo(xw);

        //    var rdfXsl = String.Format("{0}XslTemplates/{1}", ConfigurationManager.AppSettings["FilePath"], xsltFileName);
        //    return "";// XslFileTransform(sw.ToString(), rdfXsl);
        //}


    }
}
