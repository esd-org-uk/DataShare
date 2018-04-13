using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using DS.Domain.Base;

namespace DS.Domain
{
    [Table("DS_Contact")]
    public class Contact : TrackChanges
    {
        [DisplayName("Your message")]
        [Required(ErrorMessage = "Please enter your message")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        public string Message { get; set; }
        [DisplayName("Your name")]
        [Required(ErrorMessage = "Please enter your name")]
        [MaxLength(500)]
        public string FromName { get; set; }
        [DisplayName("Your email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        public string FromEmail { get; set; }
    }
}
