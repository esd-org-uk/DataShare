using System.Data.Services.Common;

namespace DS.Domain
{
    public class DataSetSchemaTagLookup 
    {
        public int Id { get; set; }
        public int DataSetSchemaId { get; set; }
        public Tag Tag { get; set; }
    }
}
