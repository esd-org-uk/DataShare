using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DS.Domain
{
    [Table("DS_SchemaESDFunctionServiceLink")]
    public class SchemaESDFunctionServiceLink
    {   
        [Key]
        public int LinkId { get; set; }
        
        public int SchemaId { get; set; }
        
        public string EsdFunctionServiceId { get; set; }

        
    }
}
