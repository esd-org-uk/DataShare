using System.Collections.Generic;

namespace DS.Domain
{
    public class EsdLinks
    {
        public EsdLinks()
        {
            Links = new List<EsdLink>();
        }

        public List<EsdLink> Links { get; set; }
    }
}