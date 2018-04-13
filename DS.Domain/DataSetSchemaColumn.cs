using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using DS.Domain.Base;
using System;

namespace DS.Domain
{
    [Table("DS_DataSetSchemaColumns")]
    public class DataSetSchemaColumn : TrackChanges, IValidatableObject
    {
        public DataSetSchemaDefinition SchemaDefinition { get; set; }
        
        [MaxLength(255)]
        [Required]
        public string Title { get; set; }
        
        [MaxLength(255)]
        [DisplayName("Column name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z].*$", ErrorMessage = "Column name must start with a letter.")]
        public string ColumnName { get; set; }
        
        [MaxLength(50)]
        [Required]
        public string Type { get; set; } //TODO: Convert to enun once ctp6 comes out in first quater of 2011
       
        [DisplayName("Maximum length")]
        [Integer(ErrorMessage = "Please enter a number")]
        [Range(1,4000, ErrorMessage = "Maximum length must be between 1 and 4000")]
        public int MaxSize { get; set; }
        
        [DisplayName("Min date")]
        [Date(ErrorMessage = "Please enter a valid date")]
        public DateTime? MinDate { get; set; }
        
        [DisplayName("Max date")]
        [Date(ErrorMessage = "Please enter a valid date")]
        public DateTime? MaxDate { get; set; }
        
        [DisplayName("Min number")]
        [Integer(ErrorMessage = "Please enter a number")]
        public int? MinNumber { get; set; }
        
        [DisplayName("Max number")]
        [Integer(ErrorMessage = "Please enter a number")]
        public int? MaxNumber { get; set; }
        
        [DisplayName("Min currency")]
        public double? MinCurrency { get; set; }
        
        [DisplayName("Max currency")]
        public double? MaxCurrency { get; set; }

        [DisplayName("Required")]
        public bool IsRequired { get; set; }
        
        [DisplayName("Display on initial view")]
        public bool IsShownInitially { get; set; }
        
        [DisplayName("Total this column")]
        public bool IsTotalisable { get; set; }

        [DisplayName("Allow filtering on this column")]
        public bool IsFilterable { get; set; }
        
        [DisplayName("Default sort")]
        public bool IsDefaultSort { get; set; }

        [DisplayName("Sort direction")]
        public string DefaultSortDirection { get; set; }

        [DisplayName("URI")]
        [MaxLength(1000)]
        public string LinkedDataUri { get; set; }

        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string HelpText { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ColumnName.ToLower() == "publisheruri" || ColumnName.ToLower() == "publisherlabel" || ColumnName.ToLower() == "publisher uri" || ColumnName.ToLower() == "publisher label")
                yield return new ValidationResult("This column name is reserved.", new[] { "ColumnName" });
        }

        #region ummapped properties
        [NotMapped]
        public bool IsNumeric
        {
            get { return Type == "Number" || Type == "Currency"; }
        }
        [NotMapped]
        public bool? IsStandardisedSchemaUrl { get; set; }

        public int? DefaultDisplayWeight { get; set; }

        #endregion

    }
}
