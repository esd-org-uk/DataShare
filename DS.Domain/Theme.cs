using System.ComponentModel.DataAnnotations;
using System.Data.Services.Common;

namespace DS.Domain
{
    [Table("DS_Theme")]
    public class Theme 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CssClass { get; set; }
    }
}
