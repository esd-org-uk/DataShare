using System.Collections.Generic;
using DS.Domain.WcfRestService;

namespace DS.Domain
{
    public class RestColumnDefinitions
    {
        public RestColumnDefinitions()
        {
            ColumnDefinitions = new List<RestColumnDefinition>();
        }
        public List<RestColumnDefinition> ColumnDefinitions { get; set; }

    }
}