using System;

namespace DS.Domain.WcfRestService
{
    public class RestDataSet
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public string FriendlyUrl { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
