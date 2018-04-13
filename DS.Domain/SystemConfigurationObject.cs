using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DS.Domain
{
    [Table("SYS_DS_ConfigurationSettings")]
    public class SystemConfigurationObject
    {   
        [Key]
        public int SettingId { get; set; }

        [DisplayName("Council Name"), Required]
        public string CouncilName { get; set; }

        [DisplayName("Council URL"), Required]
        public string CouncilUrl { get; set; }

        [DisplayName("Council URI"), Required]
        public string CouncilUri { get; set; }


        [DisplayName("Map Centre Latitude"), Required]
        public string MapCentreLatitude { get; set; }

        [DisplayName("Map Centre Longitude"), Required]
        public string MapCentreLongitude { get; set; }

        [DisplayName("Default Zoom"), Required]
        public string MapDefaultZoom { get; set; }

        [DisplayName("Analytics Tracking Ref")]
        public string AnalyticsTrackingRef { get; set; }

        [DisplayName("Spatial Geography URI"), Required]
        public string CouncilSpatialUri { get; set; }

        [DisplayName("Feedback Email")]
        public string SendEmailForFeedback { get; set; }


        [DisplayName("SMTP Server")]
        public string SmtpServer { get; set; }

        [DisplayName("SMTP Username")]
        public string SmtpUsername { get; set; }

        [DisplayName("SMTP Password")]
        public string SmtpPassword { get; set; }
        
        //[DisplayName("Default View Page Size"), Required]
        //public int DefaultViewPageSize { get; set; }

        //[DisplayName("Default Graph Page Size"), Required]
        //public int DefaultGraphPageSize { get; set; }

        //[DisplayName("Open Gov Licence Url"), Required]
        //public string OpenGovLicence { get; set; }
    }
}