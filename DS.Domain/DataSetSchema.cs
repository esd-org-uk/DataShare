using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using DS.Domain.Base;
using DS.Extensions;

namespace DS.Domain
{
    [Table("DS_DataSetSchema")]
    [EmailIsRequired("OwnerEmail", "UploadFrequency")]
    public class DataSetSchema : TrackChanges
    {
        public Category Category { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(512)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Short description is required.")]
        [DisplayName("Short Description")]
        [MaxLength(500)]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "A full description is required.")]
        [DisplayName("Full Description")]
        [MaxLength(4000)]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Description { get; set; }

        [Required(ErrorMessage = "Upload frequency is required")]
        [DisplayName("Upload frequency")]
        public int UploadFrequency { get; set; }

        public DateTime? DateLastReminded { get; set; }

        [DisplayName("Owner email address")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(255)]
        public string OwnerEmail { get; set; }

        //public IList<Group> Groups { get; set; }
        //public IList<DataSetSchemaTagLookup> Tags { get; set; }
        public IList<DataSetDetail> DataSets { get; set; }
        public DataSetSchemaDefinition Definition { get; set; }
        [DisplayName("Feature on home page?")]
        public bool IsFeatured { get; set; }
        [DisplayName("Data is stored in an external file")]
        public bool IsExternalData { get; set; }

        [DisplayName("Data is completely replaced on every upload.")]
        public bool IsAllDataOverwittenOnUpload { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsApproved { get; set; }


        public bool? IsStandardisedSchemaUrl { get; set; }
        
        
        /// <summary>
        /// IsOnline false when not approved
        /// IsOnline false when is category is not online
        /// IsOnline true when not disabled and category is online
        /// </summary>
        [NotMapped]
        public bool IsOnline
        {
            get
            {
                if (!IsApproved)
                    return false;

                return !IsDisabled ? (Category != null ? Category.IsOnline : false) : !IsDisabled;
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

        [NotMapped]
        public DateTime? DateLastUploadedTo
        {
            get
            {
                DateTime? empty = null;
                if (DataSets == null) return empty;
                return DataSets.Count == 0 ? empty : DataSets.OrderByDescending(d => d.DateCreated).FirstOrDefault().DateCreated;
            }
        }
        [NotMapped]
        public List<EsdFunction> EsdFunctions { get; set; }


        [DisplayName("Classification")]
        [NotMapped]
        public IEnumerable CurrentMappedEsdFunctionService { get; set; }

        [NotMapped]
        public List<EsdService> EsdServices { get; set; }

        [DisplayName("Schema definition URL to import from")]
        public string SchemaDefinitionFromUrl { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class EmailIsRequiredAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "Owner's email address must be supplied for regular upload reminders to be sent.";
        private readonly object _typeId = new object();

        public EmailIsRequiredAttribute(string emailProperty, string frequencyProperty)
            : base(_defaultErrorMessage)
        {
            EmailProperty = emailProperty;
            FrequencyProperty = frequencyProperty;
        }

        public string FrequencyProperty { get; private set; }
        public string EmailProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            var emailValue = properties.Find(EmailProperty, true).GetValue(value);
            var frequencyValue = properties.Find(FrequencyProperty, true).GetValue(value);
            return frequencyValue != null && (int)frequencyValue == 0 ||
                   emailValue != null && !string.IsNullOrEmpty(emailValue.ToString());
        }
    }

}
