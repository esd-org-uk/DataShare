using System;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DS.WebUI.Controllers.Extensions
{
    public class XmlResult : ActionResult
    {
        private readonly object _objectToSerialize;
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public XmlResult(object objectToSerialize, string fileName)
        {
            _objectToSerialize = objectToSerialize;
            _fileName = fileName;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize
        {
            get { return _objectToSerialize; }
        }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (_objectToSerialize == null) return;

            context.HttpContext.Response.Clear();
            var xs = new XmlSerializer(_objectToSerialize.GetType());
            context.HttpContext.Response.ContentType = "text/xml";
            context.HttpContext.Response.Headers.Add("Content-Disposition", String.Format("attachment; filename={0}.xml", _fileName));
            xs.Serialize(context.HttpContext.Response.Output, _objectToSerialize);
        }
    }


    public class CsvResult : ActionResult
    {
        private readonly string _csvData;
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvResult"/> class.
        /// </summary>
        /// <param name="csvData">The object to return as CSV.</param>
        /// <param name="fileName">The filename.</param>
        public CsvResult(string csvData, string fileName)
        {
            _csvData = csvData;
            _fileName = fileName;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object CsvData
        {
            get { return _csvData; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (_csvData == null) return;

            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/csv";
            context.HttpContext.Response.Headers.Add("Content-Disposition", String.Format("attachment; filename={0}.csv", _fileName));
            context.HttpContext.Response.Write(_csvData);
        }
    }

    public class TextFileResult : ActionResult
    {
        private readonly string _data;
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvResult"/> class.
        /// </summary>
        /// <param name="data">The object to return as CSV.</param>
        /// <param name="fileName">The filename.</param>
        public TextFileResult(string data, string fileName)
        {
            _data = data;
            _fileName = fileName;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object CsvData
        {
            get { return _data; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (_data == null) return;

            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.Headers.Add("Content-Disposition", String.Format("attachment; filename={0}.txt", _fileName));
            context.HttpContext.Response.Write(_data);
        }
    }

    public class JsonNetResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings();
        }

        public override void ExecuteResult(ControllerContext context)
        {

            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null) return;

            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter {DateTimeFormat = "dd/MM/yyyy"});

            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (var writer = new JsonTextWriter(response.Output) { Formatting = Formatting })
            {
                serializer.Serialize(writer, Data);
            }

        }
    }
}