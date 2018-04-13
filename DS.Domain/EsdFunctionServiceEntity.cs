using System;

namespace DS.Domain
{
    public class EsdFunctionServiceEntity
    {
        
        public int Identifier { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int ParentIdentifier { get; set; }
        public string ModifiedIdentifier { get; set; }

    }
}