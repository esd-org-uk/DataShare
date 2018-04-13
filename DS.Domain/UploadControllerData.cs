using System.Collections.Generic;
using System.Data;
using System.Web;

namespace DS.Domain
{
    public class MediaAssetUploadModel
    {
        public string Title { get; set; }
        public HttpPostedFileBase FileData { get; set; }
        public string SecurityToken { get; set; }
        public string Filename { get; set; }
        public int SchemaId { get; set; }
    }

    public class UploadFile
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public int SchemaId { get; set; }
    }

    public class UploadResult
    {
        public int Id { get; set; }
        public DataTable Data { get; set; }
        public List<string> Errors { get; set; }
    }

    public class UploadExternalResult
    {
        public int FileSize { get; set; }
        public string Type { get; set; }
    }
}
