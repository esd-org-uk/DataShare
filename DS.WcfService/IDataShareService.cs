using System.ServiceModel;
using System.ServiceModel.Web;
using DS.Domain;

namespace DS.WcfService
{
    [ServiceContract]
    public interface IDataShareService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Category")]
        Category GetCategory();
    }
}
