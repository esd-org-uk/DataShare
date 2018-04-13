namespace DS.Domain.WcfRestService
{
    public class RestColumnDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } //TODO: Convert to enun once ctp6 comes out in first quater of 2011
        public string MaxSize { get; set; }
        
        public string MinDate { get; set; }
        public string MaxDate { get; set; }

        public string MinCurrency { get; set; }
        public string MaxCurrency { get; set; }

        public string MinNumber { get; set; }
        public string MaxNumber { get; set; }

        public bool IsRequired { get; set; }
        public string HelpText { get; set; }


        public string Uri { get; set; }

        public bool DisplayInitial { get; set; }

        public bool Sorted { get; set; }

        public string SortDirection { get; set; }

        public bool IsTotalled { get; set; }
    }
}
