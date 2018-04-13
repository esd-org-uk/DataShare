using System.Collections.Generic;
using DS.Domain.WcfRestService;

namespace DS.Domain.Interface
{
    public interface ICategoryService
    {
        //IRepository<Category> Repository { get; }
        Category Get (string friendlyurl);
        //IList<Category> GetAll();
        IList<Category> GetAll(bool includeDisabled);
        IList<DataSetSchema> GetByFriendlyUrl(string url, bool includeDisabled);
        //IList<DataSetSchema> GetByFriendlyUrl(string url);
        IList<RestSchema> GetByFriendlyUrlIsOnline(string url);
        void Create(Category category);
        void Save(Category category);
        void Delete(string categoryName);
        void Enable(string categoryName);
        Dictionary<string, string> BreadCrumbsTitles();
    }
}