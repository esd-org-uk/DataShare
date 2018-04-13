using System.ComponentModel.DataAnnotations;
using DS.Domain.Base;

namespace DS.Domain
{
    public enum DebugInfoTypeEnum
    {
        All,
        EmailSent,
        FolderWatchTriggered,
        Developer,
        Error
    }
    
    [Table("DS_DebugInfo")]
    public class DebugInfo : TrackChanges
    {
        public DebugInfo(){}
        public DebugInfo(string description, DebugInfoTypeEnum type)
        {
            Description = description;
            Type = type.ToString();
        }

        [MaxLength(4000)]
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
