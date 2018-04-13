using System.Data;
using System.Web.Mvc;

namespace DS.Domain
{
    public class ViewControllerData
    {
        public DataTable Data { get; set; }
        public string DataGraph { get; set; }
        public DataTable Totals { get; set; }
        public int Count { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string InitialColumns { get; set; }
        public bool HasNegativeValues { get; set; }
        public double MapCentreLatitude { get; set; }
        public double MapCentreLongitude { get; set; }
        public int MapDefaultZoom { get; set; }
        public string SpatialGeographyUri { get; set; }
    }

    public class SelectListItemExtended : SelectListItem
    {
        public string CssClass { get; set; }
    }
}
