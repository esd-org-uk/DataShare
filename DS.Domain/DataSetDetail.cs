using System.ComponentModel.DataAnnotations;
using DS.Domain.Base;
using DS.Extensions;

namespace DS.Domain
{
    [Table("DS_DataSetDetails")]
    public class DataSetDetail : TrackChanges
    {
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(4000)]
        public string Note { get; set; }
        public int VersionNumber { get; set; }
        
        public DataSetSchema Schema { get; set; }
        
        public int NumOfRows { get; set; }
        public long XmlFileSize { get; set; }
        public long CsvFileSize { get; set; }
        
        [MaxLength(255)]
        public string FileUrl { get; set; }
        [MaxLength(50)]
        public string FileType { get; set; }

        [NotMapped]
        public string FriendlyUrl
        {
            get
            {
                return Title.ToUrlSlug();
            }
        }
        [NotMapped]
        public string XmlFileSizeText
        {
            get 
            {
                return XmlFileSize.ConvertBytesToString(); 
            }
        }
        [NotMapped]
        public string CsvFileSizeText
        {
            get
            {
                return CsvFileSize.ConvertBytesToString(); 
            }
        }

        [NotMapped]
        public bool IsOnline
        {
            get
            {
                return Schema.IsOnline;
            }
        }
    }
}
