using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace DS.Domain
{
    public class SchemaSourceViewModel
    {
        
        [Required]
        public string SchemaDefinitionFromUrl { get; set; }

        public bool IsStandardisedSchemaUrl { get; set; }

        public string CategoryName { get; set; }
    }
}