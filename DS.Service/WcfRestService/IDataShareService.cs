using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using DS.Domain;
using DS.Domain.WcfRestService;

namespace DS.Service.WcfRestService
{
    [ServiceContract(SessionMode=SessionMode.NotAllowed)]
    public interface IDataShareService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/")]
        IList<RestCategory> GetCategory();
         
        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}")]
        IList<RestSchema> GetSchemas(string categoryUrl);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}")]
        IList<RestDataSet> GetDataSets(string categoryUrl, string schemaUrl);

        //[OperationContract]
        //[WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/definition")]
        //IList<RestColumnDefinition> GetSchemaDefinition(string categoryUrl, string schemaUrl);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/definition")]
        [XmlSerializerFormat]
        SchemaRestDefinition GetSchemaDefinitionV2(string categoryUrl, string schemaUrl);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/{schemaDetailUrl}?format={format}")]
        Stream GetDataSetDetail(string categoryUrl, string schemaUrl, string schemaDetailUrl, string format);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByTextContains?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaTextContains(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByTextEquals?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaTextEquals(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByNumberEquals?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaNumberEquals(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByNumberGreaterThan?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaNumberGreaterThan(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByNumberGreaterThanEqualTo?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaNumberGreaterThanEqualTo(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByNumberLessThan?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaNumberLessThan(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate = "/{categoryUrl}/{schemaUrl}/SearchByNumberLessThanEqualTo?format={format}&fieldToSearch={fieldToSearch}&searchText={searchText}")]
        Stream SearchSchemaNumberLessThanEqualTo(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string searchText);

        [OperationContract]
        [WebGet(UriTemplate ="/{categoryUrl}/{schemaUrl}/SearchSchemaDate?format={format}&fieldToSearch={fieldToSearch}&from={from}&to={to}")]
        Stream SearchSchemaDate(string categoryUrl, string schemaUrl, string format, string fieldToSearch, string from,string to);

        [OperationContract, XmlSerializerFormat]
        [WebGet(UriTemplate = "/EsdInventory", ResponseFormat = WebMessageFormat.Xml)]
        Inventory GetEsdInventory();


    }
}
