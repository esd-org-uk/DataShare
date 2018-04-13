using System.Collections.Generic;

namespace DS.Domain.Interface
{
    public interface IEsdFunctionService
    {
//        string XmlContent { get; set; }
        List<EsdFunction> GetFunctions();
        List<EsdService> GetServices();
  //      void ProcessXmlContent();

        /// <summary>
        /// Delete existing entries and then create entries to link Redbridge schema id with ESD standard functions/services id.
        /// </summary>
        /// <param name="schemaId">Redbridge Schema Id</param>
        /// <param name="esdIds">ESD standard functions/services id.</param>
        void SaveLinkedFunctionServices(int schemaId, List<string> esdIds);

        /// <summary>
        /// used to get the list of current links from Redbridge schema id to ESD standard functions/services id.
        /// </summary>
        /// <param name="schemaId"></param>
        /// <returns></returns>
        List<EsdFunctionServiceEntity> GetLinkedFunctionsServices(int schemaId);
    }
}