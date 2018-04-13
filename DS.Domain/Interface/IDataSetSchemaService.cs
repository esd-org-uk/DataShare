using System.Collections.Generic;

namespace DS.Domain.Interface
{
    public interface IDataSetSchemaService
    {
        IRepository<DataSetSchema> Repository { get; }
        IList<DataSetDetail> GetByFriendlyUrl(string categoryUrl, string schemaUrl, bool showAll = false);
        IList<DataSetSchema> GetFullList();
        IList<DataSetSchema> GetAll();
        IList<DataSetSchema> GetFeatured();
        IList<DataSetSchema> GetOverDue();
        DataSetSchema Get(int id);
        DataSetSchema Get(string categoryUrl, string friendlyUrl);
        DataSetSchema Get(string friendlyUrl);
        //DataSetSchemaDefinition GetDefinition(int id);
        IList<DataSetSchema> Search(string searchTerm);
        void Create(DataSetSchema schema);
        void Save(DataSetSchema schemaData);
        void Delete(string categoryUrl, string schemaUrl);
        void DeleteAll(string categoryUrl, string schemaUrl);
        void Approve(string categoryUrl, string schemaUrl);
        void Enable(string categoryUrl, string schemaUrl);
    }
}