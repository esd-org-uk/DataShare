using System;

namespace DS.Domain.Base
{
    public interface ITrackChanges
    {
        int Id { get; set; }
        DateTime? DateUpdated { get; set; }
        DateTime DateCreated { get; set; }
        string UpdatedBy { get; set; }
        string CreatedBy { get; set; }
    }

}
