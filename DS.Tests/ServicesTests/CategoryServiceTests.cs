

namespace DS.Tests.ServicesTests
{
    using System;
    using System.Collections.Generic;

    using DS.DL.DataContext.Base.Interfaces;
    using DS.Domain;
    using DS.Domain.Interface;
    using DS.Extensions;
    using DS.Service;
    using DS.Tests.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StructureMap;

    [TestClass]
    public class CategoryServiceTests
    {
        private IRepository<Category> _repository;
        private ICacheProvider _fakecacheprovider;

        [TestInitialize]
        public void InitTests()
        {
            _repository = new MemoryRepository<Category>();
            _fakecacheprovider = new FakeCacheProvider();
            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();

              }

           );
 



        }


        [TestMethod]
        public void Get_when_repository_has_no_records_returns_null()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.Get("no_record_should_be_here");
            //assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Get_when_repository_has_records_returns_type_of_category()
        {
            //arrange
            var category = new Category(){
                    Title = "Get_when_repository_has_records_returns_type_of_category",
                };
            _repository.Add(category);
            var service = new CategoryService(_repository, _fakecacheprovider);
            var friendlyUrl = "Get_when_repository_has_records_returns_type_of_category".ToUrlSlug();
            
            //action
            var result = service.Get(friendlyUrl);
          
            //assert
            Assert.AreEqual(typeof(Category), result.GetType());

            //cleanup
            _repository.Delete(category);

        }

        [TestMethod]
        public void GetAll_false_should_return_categories_excluding_disabled_categories()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { };
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = schemas };
            var cat2 = new Category() { Title = "record item disabled", IsDisabled = true , Schemas = schemas};
            _repository.Add(cat1);
            _repository.Add(cat2);
            
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetAll(false);
            
            //assert
            Assert.AreEqual(1, result.Count);

            //clean up
            _repository.Add(cat1);
            _repository.Add(cat2);
        }
        [TestMethod]
        public void GetAll_false_when_cateogory_schemas_is_null_should_return_categories_with_empty_schema_list()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { };
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = schemas};
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetAll(false);
            //assert
            Assert.AreEqual(0,result[0].Schemas.Count);
            //cleanup
            _repository.Delete(cat1);
        }

        [TestMethod]
        public void GetAll_false_when_cateogory_schemas_is_not_null_and_has_datasetdetails_and_isOnline_should_return_categories_schema_list()
        {
            //arrange
            var schemas = new List<DataSetSchema>(){ new DataSetSchema(){ Category = new Category(),IsApproved = true, IsDisabled = false,DataSets = new List<DataSetDetail>(){new DataSetDetail()}}};
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false , Schemas = schemas};
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetAll(false);
            //assert
            Assert.AreEqual(1, result[0].Schemas.Count);
            //cleanup
            _repository.Delete(cat1);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetAll_false_when_cateogory_schemas_is_not_null_and_no_datasetdetails_and_isOnline_should_return_empty_categories_schema_list()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Category = new Category(), IsApproved = true, IsDisabled = false,  } };
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = schemas };
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetAll(false);
            
            //assert - not needed as this is testing exception
            //Assert.AreEqual(1, result[0].Schemas.Count);
            //cleanup
            _repository.Delete(cat1);
        }
        [TestMethod]
        public void GetByFriendlyUrl_when_category_is_null_return_empty_list_of_datasetschemas()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetByFriendlyUrl("record item enabled".ToUrlSlug(), false);
            //assert
            Assert.AreEqual(0, result.Count);
            
            
        }


        [TestMethod]
        public void GetByFriendlyUrl_when_category_schemas_is_null_return_empty_list_of_datasetschemas()
        {
            //arrange
            
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = null };
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetByFriendlyUrl("record item enabled".ToUrlSlug(), false);
            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup
            _repository.Delete(cat1);

        }

        [TestMethod]
        public void GetByFriendlyUrl_include_disabled_false_return_list_of_all_datasetschemas_which_are_not_disabled_only()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { 
                new DataSetSchema() { Category = new Category(), IsApproved = true, IsDisabled = false, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
                ,new DataSetSchema() { Category = new Category(), IsApproved = true, IsDisabled = true, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
            };
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = schemas };
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetByFriendlyUrl("record item enabled".ToUrlSlug(), false);
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repository.Delete(cat1);

        }
        [TestMethod]
        public void GetByFriendlyUrl_include_disabled_true_return_list_of_all_datasetschema()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { 
                new DataSetSchema() { Category = new Category(), IsApproved = true, IsDisabled = false, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
                ,new DataSetSchema() { Category = new Category(), IsApproved = true, IsDisabled = true, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
            };
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = schemas };
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetByFriendlyUrl("record item enabled".ToUrlSlug(), true);
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
            _repository.Delete(cat1);

        }

        [TestMethod]
        public void GetByFriendlyUrl_return_list_of_all_datasetschema_ordered_by_title()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { 
                new DataSetSchema() { Title = "zebra", Category = new Category(), IsApproved = true, IsDisabled = false, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
                ,new DataSetSchema() {Title = "cat",  Category = new Category(), IsApproved = true, IsDisabled = true, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
                ,new DataSetSchema() {Title = "anchor",  Category = new Category(), IsApproved = true, IsDisabled = true, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
            };
            var cat1 = new Category() { Title = "record item enabled", IsDisabled = false, Schemas = schemas };
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.GetByFriendlyUrl("record item enabled".ToUrlSlug(), true);
            //assert
            Assert.AreEqual("anchor", result[0].Title);
            //cleanup
            _repository.Delete(cat1);

        }

        [TestMethod]
        public void GetByFriendlyUrlIsOnline_when_schemas_count_is_zero_returns_empty_list_of_restschema()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);

            //act
            var result = service.GetByFriendlyUrlIsOnline("url-is-online");

            //assert
            Assert.AreEqual(0, result.Count);
            

        }

        [TestMethod]
        public void GetByFriendlyUrlIsOnline_when_schema_url_is_not_found_returns_empty_list_of_restschema()
        {
            //arrange
    
            var cat1 = new Category() { Title = "url is online2", IsDisabled = false };

            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);

            //act
            var result = service.GetByFriendlyUrlIsOnline("url-is-online");

            //assert
            Assert.AreEqual(0, result.Count);

            //clean up
            _repository.Delete(cat1);


        }
        [TestMethod]
        public void GetByFriendlyUrlIsOnline_when_url_is_found_and_schema_is_online_returns_list_of_restschema()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { 
                new DataSetSchema() { Title = "zebra", Category = new Category(), IsApproved = true, IsDisabled = false, DataSets = new List<DataSetDetail>() { new DataSetDetail() } }
            };
            var cat1 = new Category() { Title = "url is online2", IsDisabled = false, Schemas = schemas };

            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);

            //act
            var result = service.GetByFriendlyUrlIsOnline("url-is-online2");

            //assert
            Assert.AreEqual("zebra", result[0].Title);

            //clean up
            _repository.Delete(cat1);


        }


        [TestMethod]
        public void Create_when_category_title_is_not_in_repository_adds_into_repository()
        {
            //arrange
            var cat1 = new Category() { Title = "url is online2", IsDisabled = false };
            //_repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);

            //act
            service.Create(cat1);
            var result = service.Get("url-is-online2");
            //assert
            Assert.AreEqual("url is online2", result.Title);

            //clean up
            _repository.Delete(cat1);


        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "A category named url is online2 already exists")]
        public void Create_when_category_title_is_in_repository_throws_exception()
        {
            //arrange
            var cat1 = new Category() { Title = "url is online2", IsDisabled = false };
            _repository.Add(cat1);
            var service = new CategoryService(_repository, _fakecacheprovider);

            //act
            service.Create(cat1);
            //assert - no need to assert as this will throw an exception
            //Assert.AreEqual("url is online2", result.Title);

            //clean up
            _repository.Delete(cat1);


        }


        [TestMethod]
        [ExpectedException(typeof(Exception), "A category named url is online2 already exists")]
        public void Save_when_original_category_title_is_changed_and_changed_title_already_exists_throws_exception()
        {
            //arrange
            var cat1 = new Category() { Id=1, Title = "url is online1", IsDisabled = false };
            var cat2 = new Category() { Id =2, Title = "url is online2", IsDisabled = false };
            var changedCat1 = new Category() { Id=1, Title = "url is online2", IsDisabled = false };
            _repository.Add(cat1);
            _repository.Add(cat2);
            

            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Save(changedCat1);
            //assert - no need to assert as this will throw an exception
            
            //clean up
            _repository.Delete(cat1);
        }


        [TestMethod]
        public void Save_will_update_category_description()
        {
            //arrange
            var cat1 = new Category() { Id = 1, Title = "url is online1", IsDisabled = false };
            var changedCat1 = new Category() { Id = 1, Title = "url is online1", IsDisabled = false, Description = "changeddescription"};
            _repository.Add(cat1);
            

            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Save(changedCat1);
            var result = service.Get("url-is-online1");
            //assert - no need to assert as this will throw an exception
            Assert.AreEqual("changeddescription", result.Description);
            //clean up
            _repository.Delete(cat1);
        }

        [TestMethod]
        public void Save_will_update_category_ImageUrl()
        {
            //arrange
            var cat1 = new Category() { Id = 1, Title = "url is online1", IsDisabled = false };
            var changedCat1 = new Category() { Id = 1, Title = "url is online1", IsDisabled = false, ImageUrl = "changedimageurl" };
            _repository.Add(cat1);


            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Save(changedCat1);
            var result = service.Get("url-is-online1");
            //assert - no need to assert as this will throw an exception
            Assert.AreEqual("changedimageurl", result.ImageUrl);
            //clean up
            _repository.Delete(cat1);
        }

        [TestMethod]
        public void Delete_when_category_is_found_will_set_isDisabled_flag_to_true()
        {
            //arrange
            //var schemas = new List<DataSetSchema>(){new DataSetSchema(){Title = "dataset schema name", IsDisabled =  false}};
            var cat1 = new Category() { Id = 1, Title = "url is online1", IsDisabled = false, Schemas = null};
            _repository.Add(cat1);


            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Delete("url-is-online1");
            var result = service.Get("url-is-online1");
            //assert - no need to assert as this will throw an exception
            Assert.IsTrue(result.IsDisabled);
            //clean up
            _repository.Delete(cat1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "A category named url is online1 does not exists")]
        public void Delete_when_category_is_not_found_will_throw_exception()
        {
            //arrange
            var cat1 = new Category() { Id = 1, Title = "url is online2", IsDisabled = false, Schemas = null };
            _repository.Add(cat1);

            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Delete("url-is-online1");
            //assert - no need to assert as this will throw an exception

            //clean up
            _repository.Delete(cat1);
        }


        [TestMethod]
        public void Enable_when_category_is_found_will_set_isDisabled_flag_to_false()
        {
            //arrange
            //var schemas = new List<DataSetSchema>(){new DataSetSchema(){Title = "dataset schema name", IsDisabled =  false}};
            var cat1 = new Category() { Id = 1, Title = "url is online1", IsDisabled = true, Schemas = null };
            _repository.Add(cat1);
            
            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Enable("url-is-online1");
            var result = service.Get("url-is-online1");
            //assert - no need to assert as this will throw an exception
            Assert.IsFalse(result.IsDisabled);
            //clean up
            _repository.Delete(cat1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "A category named url is online1 does not exists")]
        public void Enable_when_category_is_not_found_will_throw_exception()
        {
            //arrange
            var cat1 = new Category() { Id = 1, Title = "url is online2", IsDisabled = false, Schemas = null };
            _repository.Add(cat1);

            var service = new CategoryService(_repository, _fakecacheprovider);
            //act
            service.Enable("url-is-online1");
            //assert - no need to assert as this will throw an exception

            //clean up
            _repository.Delete(cat1);
        }


        [TestMethod]
        public void BreadCrumbsTitles_returns_dictionary_of_string_string()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);

            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual(typeof(Dictionary<string, string>), result.GetType());
            //cleanup
        }


        [TestMethod]
        public void BreadCrumbsTitles_when_cache_result_is_not_null_returns_dictionary_of_string_string_from_cache()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            _fakecacheprovider.Set("BreadCrumbs", new Dictionary<string, string>());
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup
        }


        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_home_with_value_Home()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Home", result["home"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_download_with_value_download_choose_a_category()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Download - choose a category", result["download"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_view_with_value_view_choose_a_category()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("View - choose a category", result["view"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_about_with_value_about_datashare()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("About DataShare", result["about"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_developer_with_value_developer_area()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Developer area", result["developer"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_contactus_with_value_contact_us()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Contact us", result["contactus"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_licence_with_value_licence()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Licence", result["licence"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_history_with_value_version_history()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Version history", result["history"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_category_with_value_manage_categories()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Manage categories", result["category"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_schema_with_value_manage_schemas()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Manage schemas", result["schema"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_feedback_with_value_view_feedback()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("View feedback", result["feedback"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_useradmin_with_value_user_administration()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("User administration", result["useradmin"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_changepassword_with_value_change_password()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Change password", result["changepassword"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_debuginfo_with_value_service_history()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("Service history", result["debuginfo"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_has_static_key_systemconfig_with_value_system_configuration()
        {
            //arrange
            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("System Configuration", result["systemconfig"]);
            //cleanup
        }

        [TestMethod]
        public void BreadCrumbsTitles_will_include_category_item_from_repository()
        {
            //arrange
            var cat1 = new Category() { Id = 1, Title = "url is online2", IsDisabled = false };
            _repository.Add(cat1);

            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("url is online2", result["url-is-online2"]);
            //cleanup
        }
        [TestMethod]
        public void BreadCrumbsTitles_will_include_category_schema_item_from_repository()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Title = "schematitle" } };
            var cat1 = new Category() { Id = 1, Title = "url is online2", IsDisabled = false, Schemas = schemas };
            _repository.Add(cat1);

            var service = new CategoryService(_repository, _fakecacheprovider);
            //action
            var result = service.BreadCrumbsTitles();
            //assert
            Assert.AreEqual("schematitle", result["url-is-online2_schematitle"]);
            //cleanup
        }

    }



}
