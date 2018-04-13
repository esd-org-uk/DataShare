
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace DS.Service
{
    using System;

    using System.Data;
    using System.IO;
    using DS.Domain.Interface;

    
    public class CustomDataTableToXmlSerializer :IDataTableSerializer
    {
        public MemoryStream GetStream(DataTable dataTable)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var xmlw = new XmlTextWriter(writer);
            var sb = new StringBuilder();

            var xmlData = new StringWriter(sb);
            dataTable.WriteXml(xmlData);
            var xmlfile = new XmlDocument();
            xmlfile.LoadXml(xmlData.ToString());
            xmlfile.Save(xmlw);

            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
            
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
