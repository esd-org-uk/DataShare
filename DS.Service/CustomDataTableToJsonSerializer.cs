namespace DS.Service
{
    using System.Data;
    using System.IO;
    using System.ServiceModel.Web;
    using System.Text;
    using DS.Domain.Interface;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Formatting = System.Xml.Formatting;
    
    public class CustomDataTableToJsonSerializer :IDataTableSerializer
    {
        public MemoryStream GetStream(DataTable dataTable)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            serializer.NullValueHandling = NullValueHandling.Ignore;
            var jsonwriter = new JsonTextWriter(writer)
            {
                Formatting = (Newtonsoft.Json.Formatting)Formatting.Indented
            };
            serializer.Serialize(jsonwriter, dataTable);

            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/json";

            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }



}
