using System;

namespace DS.Domain
{
    public class FilterCriteria
    {
        public FilterCriteria()
        {
            SearchOperator = "";
            SearchOperatorNumber = "";
            SearchNumber = "";
            SearchText = "";
            From = "";
            To = "";
        }
        public string ColumnToSearch { get; set; }
        public string SearchOperator { get; set; }
        public string SearchText { get; set; }
        public string SearchOperatorNumber { get; set; }
        public string SearchNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string SearchType { 
            get
            {
                if (!String.IsNullOrEmpty(From) && !String.IsNullOrEmpty(To))
                {
                    return "Date";
                }
                return !String.IsNullOrEmpty(SearchOperatorNumber) ? "Number" : "Text";
            }
        }
    }
}