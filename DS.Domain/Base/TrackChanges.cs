using System;
using System.ComponentModel.DataAnnotations;

namespace DS.Domain.Base
{
    public class TrackChanges : ITrackChanges
    {
        public int Id { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }
        [MaxLength(255)]
        public string UpdatedBy { get; set; }
        [MaxLength(255)]
        public string CreatedBy { get; set; }
    }
}
