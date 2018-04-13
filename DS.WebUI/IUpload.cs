using System.IO;
using System.ServiceModel;

namespace DS.WebUI
{
    [ServiceContract]
    public interface IUpload
    {
        [OperationContract]
        bool AddFile(Stream fileData);
    }
}
