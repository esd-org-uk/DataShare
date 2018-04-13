using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using StructureMap;

namespace DS.Service
{
    public class EsdFunctionService : IEsdFunctionService
    {
        /// <summary>
        /// to set for the i.e. "Functions/Function"
        /// set in the constructor
        /// </summary>
        private readonly string _xPathOfFunctions;

        /// <summary>
        /// to set for the services i.e. "Services/Service"
        /// set in the constructor
        /// </summary>
        private readonly string _xPathOfServices;

        private readonly List<EsdFunction> _loadedFunctions;
        private readonly List<EsdService> _loadedServices;
        private IRepository<SchemaESDFunctionServiceLink> _repository;
        


        public EsdFunctionService(IRepository<SchemaESDFunctionServiceLink> repository, IGetFunctionServiceXmlContent functionServiceXmlContent)
        {
            _loadedFunctions = new List<EsdFunction>();
            _loadedServices = new List<EsdService>();
            _xPathOfFunctions = "Functions/Function";
            _xPathOfServices = "Services/Service";
            _repository = repository;
            _xmlContent = functionServiceXmlContent.GetFunctionServiceXmlContent();

            if (string.IsNullOrWhiteSpace(_xmlContent)) throw new ArgumentException("Xml content is not in valid format");

            ProcessXmlContent();

        }

        private string _xmlContent;// { get; set; }
        //private string XmlContent;// { get; set; }


        public List<EsdFunction> GetFunctions()
        {
            return _loadedFunctions; 
        }

        public List<EsdService> GetServices()
        {
            return _loadedServices; 
        }

        private void ProcessXmlContent()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xmlContent);


            var nodes = xmlDoc.SelectNodes(_xPathOfFunctions);
            //if (nodes == null) throw new Exception("Unable to find functions/function in xml");
            if (nodes.Count == 0) throw new Exception("Unable to find functions/function in xml");

            PopulateChildEsdFunctions(nodes, 0);
        }

        private void PopulateChildEsdFunctions(XmlNodeList nodes, int parentFunctionId)
        {
            foreach (XmlNode childrenNode in nodes)
            {
                var identifier = Convert.ToInt32(childrenNode.SelectSingleNode("Identifier").InnerText);
                _loadedFunctions.Add(new EsdFunction()
                    {
                        Label = childrenNode.SelectSingleNode("Label").InnerText,
                        Description = childrenNode.SelectSingleNode("Description").InnerText,
                        Url = childrenNode.SelectSingleNode("URI").InnerText,
                        Identifier = identifier,
                        ParentIdentifier = parentFunctionId,
                    });

                var childrenNodesFunctions = childrenNode.SelectNodes(_xPathOfFunctions);

                if (childrenNodesFunctions != null && childrenNodesFunctions.Count > 0)
                    PopulateChildEsdFunctions(childrenNodesFunctions, identifier);

                var childrenNodesServices = childrenNode.SelectNodes(_xPathOfServices);

                if (childrenNodesServices != null && childrenNodesServices.Count > 0)
                    PopulateChildEsdServices(childrenNodesServices, identifier);
            }
        }

        private void PopulateChildEsdServices(XmlNodeList nodes, int parentFunctionId)
        {
            foreach (XmlNode childrenNode in nodes)
            {
                _loadedServices.Add(new EsdService()
                    {
                        Label = childrenNode.SelectSingleNode("Label").InnerText,
                        Description = childrenNode.SelectSingleNode("Description").InnerText,
                        Identifier = Convert.ToInt32(childrenNode.SelectSingleNode("Identifier").InnerText),
                        Url = childrenNode.SelectSingleNode("URI").InnerText,
                        ParentIdentifier = parentFunctionId,
                    });
            }
        }


        /// <summary>
        /// remove all existing links from redbridge schemas to the ESD standard functions/services id.
        /// </summary>
        /// <param name="schemaId">Redbridge Schema id</param>
        private void DeleteLinkedFunctionServices(int schemaId)
        {
            var entries = _repository.GetQuery().Where(x => x.SchemaId == schemaId).ToList();
            entries.ForEach(x => _repository.Delete(x));
            _repository.SaveChanges();
        }

        /// <summary>
        /// Delete existing entries and then create entries to link Redbridge schema id with ESD standard functions/services id.
        /// </summary>
        /// <param name="schemaId">Redbridge Schema Id</param>
        /// <param name="esdIds">ESD standard functions/services id.</param>
        public void SaveLinkedFunctionServices(int schemaId, List<string> esdIds)
        {
            DeleteLinkedFunctionServices(schemaId);
            var linkedRange = esdIds.Select(x => new SchemaESDFunctionServiceLink()
                {
                    EsdFunctionServiceId = x,
                    SchemaId = schemaId
                }).ToList();
            linkedRange.ForEach(x => _repository.Add(x));
            _repository.SaveChanges();
        }

        /// <summary>
        /// used to get the list of current links from Redbridge schema id to ESD standard functions/services id.
        /// </summary>
        /// <param name="schemaId"></param>
        /// <returns></returns>
        public List<EsdFunctionServiceEntity> GetLinkedFunctionsServices(int schemaId)
        {
            var returnList = new List<EsdFunctionServiceEntity>();
            var esdIds =
                _repository.GetQuery().Where(x => x.SchemaId == schemaId).Select(x => x.EsdFunctionServiceId).ToList();
            if (!esdIds.Any()) return returnList;


            returnList.AddRange(
                GetFunctions()
                    .Where(x => esdIds.Contains(x.Type + Convert.ToString(x.Identifier)))
                    .Select(x => new EsdFunctionServiceEntity()
                        {
                            ModifiedIdentifier = x.Type + x.Identifier,
                            Created = x.Created,
                            Description = x.Description,
                            Identifier = x.Identifier,
                            Label = x.Label,
                            Modified = x.Modified,
                            ParentIdentifier = x.ParentIdentifier,
                            Type = x.Type,
                            Url = x.Url,
                        }));

            returnList.AddRange(
                GetServices()
                    .Where(x => esdIds.Contains(x.Type + Convert.ToString(x.Identifier)))
                    .Select(x => new EsdFunctionServiceEntity()
                        {
                            ModifiedIdentifier = x.Type + x.Identifier,
                            Created = x.Created,
                            Description = x.Description,
                            Identifier = x.Identifier,
                            Label = x.Label,
                            Modified = x.Modified,
                            ParentIdentifier = x.ParentIdentifier,
                            Type = x.Type,
                            Url = x.Url,
                        }));
            return returnList;
        }
    }
}
