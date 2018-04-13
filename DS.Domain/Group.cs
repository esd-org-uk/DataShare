using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DS.Domain
{
    [Table("DS_Group")]
    public class Group
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(75)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public IList<GroupMember> Members { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<DataSetSchema> Schemas { get; set; }
    }

    [Table("DS_GroupUsers")]
    public class GroupMember    
    {
        public int id { get; set; }
        public Group Group { get; set; }
        public string User { get; set; }
    }
}
