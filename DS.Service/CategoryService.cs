using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Domain.WcfRestService;
using DS.Extensions;
using StructureMap;

namespace DS.Service
{
    public  class CategoryService : ICategoryService
    {
        private IRepository<Category> _repository;
        private ICacheProvider _cacheProvider;

        public CategoryService(IRepository<Category> repository, ICacheProvider cacheProvider)
        {
            _repository = repository;
            _cacheProvider = cacheProvider;
        }
 

        public Category Get(string friendlyurl)
        {
            return _repository.GetQuery().ToList().FirstOrDefault(c => c.FriendlyUrl == friendlyurl);
        }

        //public  IList<Category> GetAll()
        //{
        //    return GetAll(false);
        //}

        public  IList<Category> GetAll(bool includeDisabled)
        {
            List<Category> categories;
            if (includeDisabled)
            {
                categories = _repository.GetQuery()
                                       .Include(c => c.Schemas)
                                       .Include("Schemas.DataSets").ToList()
                                       .OrderBy(c=>c.Title).ToList();
                
            }
            else
            {
                    categories = _repository.GetQuery()
                                       .Include(c => c.Schemas)
                                       .Include("Schemas.DataSets").ToList()
                                       .Where(c => c.IsOnline)
                                       .OrderBy(c=>c.Title).ToList();
                
            }
                
            

            foreach (var category in categories)
            {
                category.Schemas = category.Schemas.Where(s => s.IsOnline && s.DataSets.Count > 0 || includeDisabled).ToList();
            }
            return categories;
        }

        public  IList<DataSetSchema> GetByFriendlyUrl(string url, bool includeDisabled)
        {
            var category = _repository.GetQuery()
                .Include("Schemas")
                .Include("Schemas.DataSets").ToList().FirstOrDefault(c => c.FriendlyUrl == url);
            
            if(category == null)
                return new List<DataSetSchema>();

            if(category.Schemas == null)
                return new List<DataSetSchema>();

            return category.Schemas.Where(s => s.IsOnline && s.DataSets.Count > 0 || includeDisabled).OrderBy(s => s.Title).ToList();

            //return category.Schemas.Any() ? category.Schemas : new List<DataSetSchema>();
        }

        //public  IList<DataSetSchema> GetByFriendlyUrl(string url)
        //{
        //    return GetByFriendlyUrl(url, false);
        //}

        public IList<RestSchema> GetByFriendlyUrlIsOnline(string url)
        {
           var schemas =   _repository.Query(s => s.Schemas).ToList();

            if(schemas.Count ==0) return new List<RestSchema>();

                
            var firstSchema = schemas.FirstOrDefault(s => s.FriendlyUrl == url.ToLower());
            
            if(firstSchema == null) return new List<RestSchema>();

            return  firstSchema.Schemas.Where(s => s.IsOnline)
                                                     .Select(s => new RestSchema
                                                     {

                                                         Title = s.Title,
                                                         ShortDescription = s.ShortDescription,
                                                         Description = s.Description,
                                                         FriendlyUrl = s.Title.ToUrlSlug()

                                                     }).ToList();

        }

        public  void Create(Category category)
        {
            // check category name does not already exist
            if (_repository.GetQuery().FirstOrDefault(c => c.Title.ToLower() == category.Title.ToLower()) == null)
            {
                _repository.Add(category);
                _repository.SaveChanges();
                return;
            }

            throw new Exception(string.Format("A category named {0} already exists", category.Title));
        }

        public  void Save(Category category)
        {
            var originalCat = _repository.GetQuery().Single(c => c.Id == category.Id);
            // check category name does not already exist if changed
            if (category.Title.ToLower() != originalCat.Title.ToLower())
            {
                if (_repository.GetQuery().FirstOrDefault(c => c.Title.ToLower() == category.Title.ToLower()) != null)
                {
                    throw new Exception(string.Format("A category named {0} already exists", category.Title));
                }

            }
            originalCat.Title = category.Title;
            originalCat.Description = category.Description;
            originalCat.ImageUrl = category.ImageUrl;
            _repository.SaveChanges();
        }

        public  void Delete(string categoryName)
        {
            var category = _repository.GetQuery()
                .Include(c => c.Schemas).ToList().FirstOrDefault(c => c.FriendlyUrl == categoryName);
            if (category == null) throw new Exception(string.Format("A category named {0} does not exists", categoryName)); 

            category.IsDisabled = true;
            _repository.SaveChanges();
        }

        public  void Enable(string categoryName)
        {
            var category = _repository.GetQuery()
                .Include(c => c.Schemas).ToList().FirstOrDefault(c => c.FriendlyUrl == categoryName);
            if (category == null) throw new Exception(string.Format("A category named {0} does not exists", categoryName)); 
            category.IsDisabled = false;
            _repository.SaveChanges();
        }



        #region Breadcrumbs
        public  Dictionary<string, string> BreadCrumbsTitles()
        {
            Dictionary<string, string> cacheResult;
            _cacheProvider.Get("BreadCrumbs", out cacheResult);
            if (cacheResult != null)
            {
                return cacheResult;
            }
            
            cacheResult = BuildBreadCrumbs();
            if(_cacheProvider != null) _cacheProvider.Set("BreadCrumbs", cacheResult);

            return cacheResult;

            //return HttpRuntime.Cache.GetOrStore("BreadCrumbs", BuildBreadCrumbs);
        }

        private  Dictionary<string, string> BuildBreadCrumbs()
        {
            var breadCrumbs = new Dictionary<string, string>
                                  {
                                      {"home","Home"}, 
                                      {"download", "Download - choose a category"}, 
                                      {"view", "View - choose a category"},
                                      {"about", "About DataShare"},
                                      {"developer","Developer area"},
                                      {"contactus","Contact us"},
                                      {"licence","Licence"},
                                      {"history", "Version history"},
                                      {"category","Manage categories"},
                                      {"schema","Manage schemas"},
                                      {"feedback","View feedback"},
                                      {"useradmin","User administration"},
                                      {"changepassword","Change password"},
                                      {"debuginfo","Service history"},
                                        {"systemconfig","System Configuration"}

                                  };

            foreach (var c in _repository.GetQuery().Include("Schemas").Include("Schemas.Category"))
            {
                breadCrumbs.Add(c.FriendlyUrl.ToLower(), c.Title);
                if(c.Schemas != null)
                    foreach (var s in c.Schemas)
                    {
                        breadCrumbs.Add(c.FriendlyUrl + "_" + s.FriendlyUrl, s.Title);
                    }
            }

            return breadCrumbs;
        } 
        #endregion
    }
}
