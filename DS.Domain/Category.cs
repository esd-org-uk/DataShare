using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DS.Domain.Base;
using DS.Extensions;

namespace DS.Domain
{
    [Table("DS_Category")]
    public class Category : TrackChanges
    {
        [Required]
        [MaxLength(75)]
        public string Title { get; set; }
        
        [Required]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool IsDisabled { get; set; }
        public IList<DataSetSchema> Schemas { get; set; }
        [MaxLength(1024)]
        public string ImageUrl { get; set; }

        //public IList<Group> Groups { get; set; }

        [NotMapped]
        public string CssClass { 
            get
            {
                return Title.RemovePunctuationAndSpacing();
            }
        }

        [NotMapped]
        public string FriendlyUrl
        {
            get
            {
                return Title.ToUrlSlug();
            }
        
        }

        [NotMapped]//todo - ken to remove as this is WritingEverthingTwice,  - it can use the flag isDisabled ? - best to use Don'tRepeatYourself
        public bool IsOnline
        {
            get
            {       
                return !IsDisabled;
            }
        }
    }
}
