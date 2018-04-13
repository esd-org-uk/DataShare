using System.Xml;
using System.Xml.Serialization;
using DS.Domain.WcfRestService;

namespace DS.Domain
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class SchemaRestDefinition
    {

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces _xsns;

       public SchemaRestDefinition()
       {
           _xsns = new XmlSerializerNamespaces(
            new XmlQualifiedName[] { //new XmlQualifiedName("inv", "http://schemas.esd.org.uk/inventory"),  
            //new XmlQualifiedName("conformsTo", "http://standards.esd.org.uk/schemas/inventory.xsd"), 
            
            });
           EsdLinks = new EsdLinks();
           ErrorMessage ="";
       }

       public SchemaRestDefinition(string conformsTo)
       {
           _xsns = new XmlSerializerNamespaces(
            new XmlQualifiedName[] { //new XmlQualifiedName("inv", "http://schemas.esd.org.uk/inventory"),  
            //new XmlQualifiedName("conformsTo", "http://standards.esd.org.uk/schemas/inventory.xsd"), 
            
            });
           EsdLinks = new EsdLinks();
           ConformsTo = conformsTo;
           ErrorMessage  = "";
       }
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
       public string ConformsTo { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public RestColumnDefinitions RestColumnDefinitions { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public RestSchema RestSchema { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public EsdLinks EsdLinks { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public string ErrorMessage { get; set; }
    }

  
}