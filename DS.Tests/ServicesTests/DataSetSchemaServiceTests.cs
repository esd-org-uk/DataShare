using System;
using System.Collections.Generic;
using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class DataSetSchemaServiceTests
    {
        private IRepository<DataSetSchema> _repositoryDataSetSchema;
        private IDataSetSchemaDefinitionService _dataSetSchemaDefinitionService;
        private IRepository<DataSetDetail> _repositoryDataSetDetail;
        private ISqlTableUtility _sqlTableUtility;

        [TestInitialize]
        public void TestInit()
        {
            _repositoryDataSetSchema = new MemoryRepository<DataSetSchema>();
            _repositoryDataSetDetail = new MemoryRepository<DataSetDetail>();
            ObjectFactory.Initialize(x => { x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>(); }
                );
        }

        [TestMethod]
        public void GetByFriendlyUrl_show_all_return_all_datasetdetail_ordered_by_desc_dateupdated()
        {
            //arrange
            //var schema1 = new DataSetSchema() { Category = new Category() { Title = "categoryurl" }, Title = "schemaurl"};
            var ds = new List<DataSetDetail>()
                {
                    new DataSetDetail(){DateUpdated = new DateTime(1998,1,1)}
                    ,new DataSetDetail(){DateUpdated = new DateTime(1999,1,1)}
                    
                };
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "categoryurl" }, Title = "schemaurl", IsDisabled = true , DataSets =ds };
            //_repositoryDataSetSchema.Add(schema1);
            _repositoryDataSetSchema.Add(schema2);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetByFriendlyUrl("categoryurl", "schemaurl", true);
            //assert
            Assert.AreEqual(new DateTime(1999,1,1), result[0].DateUpdated);
            //cleanup
            //_repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);

        }


        [TestMethod]
        public void GetByFriendlyUrl_show_all_is_false_return_only_online_schema_with_datasetdetail_ordered_by_desc_dateupdated()
        {
            //arrange
            var ds1 = new List<DataSetDetail>()
                {
                    new DataSetDetail(){DateUpdated = new DateTime(2011,1,1)}
                    ,new DataSetDetail(){DateUpdated = new DateTime(2010,1,1)}
                    
                };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "categoryurl" , IsDisabled = false}, Title = "schemaurl", DataSets = ds1, IsApproved = true};
            var ds2 = new List<DataSetDetail>();
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "categoryurl" }, Title = "schemaurl", IsDisabled = true, DataSets = ds2 };
            _repositoryDataSetSchema.Add(schema1);
            _repositoryDataSetSchema.Add(schema2);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetByFriendlyUrl("categoryurl", "schemaurl", false);
            //assert
            Assert.AreEqual(new DateTime(2011, 1, 1), result[0].DateUpdated);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);

        }


        [TestMethod]
        public void GetByFriendlyUrl_when_schema_is_null_return_empty_list_of_datasetdetail()
        {
            //arrange
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetByFriendlyUrl("categoryurl", "schemaurl", false);
            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup
        }

        [TestMethod]
        public void GetByFriendlyUrl_when_schema_datasets_is_null_return_empty_listof_datasetdetail()
        {
            //arrange
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", IsApproved = true };
            _repositoryDataSetSchema.Add(schema1);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetByFriendlyUrl("categoryurl", "schemaurl", false);
            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
        }
    

        [TestMethod]
        public void GetAll_returns_DatasetSchemas_with_more_than_zero_Datasets_and_isonline()
        {
            //arrange
            var ds1 = new List<DataSetDetail>(){new DataSetDetail(){DateUpdated = new DateTime(2011,1,1)},new DataSetDetail(){DateUpdated = new DateTime(2010,1,1)}};
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = ds1, IsApproved = true };
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = null, IsApproved = true };
            _repositoryDataSetSchema.Add(schema2);
            var ds3 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = ds3, IsApproved = false };
            _repositoryDataSetSchema.Add(schema3);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetAll();
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);
        }

        [TestMethod]
        public void GetAll_returns_DatasetSchemas_orderedby_title_ascending()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "btitle", DataSets = ds1, IsApproved = true };
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "atitle", DataSets = ds1, IsApproved = true };
            _repositoryDataSetSchema.Add(schema2);
            var ds3 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "ctitle", DataSets = ds3, IsApproved = true };
            _repositoryDataSetSchema.Add(schema3);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetAll();
            //assert
            Assert.AreEqual("atitle", result[0].Title);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);
        }



        [TestMethod]
        public void GetFullList_returns_all_regardless_of_online_or_not()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = ds1, IsApproved = true };
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = null, IsApproved = true };
            _repositoryDataSetSchema.Add(schema2);
            var ds3 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = ds3, IsApproved = false };
            _repositoryDataSetSchema.Add(schema3);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetFullList();
            //assert
            Assert.AreEqual(3, result.Count);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);
        }
        [TestMethod]
        public void GetFeatured_returns_all_is_featured()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "btitle", DataSets = ds1, IsApproved = true, IsFeatured = false};
            _repositoryDataSetSchema.Add(schema1);
            //this is featured.
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "atitle", DataSets = ds1, IsApproved = true, IsFeatured=true };
            _repositoryDataSetSchema.Add(schema2);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetFeatured();
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            
        }

        [TestMethod]
        public void GetFeatured_returns_all_is_featured_and_which_are_online_with_datasets_more_than_zero()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "btitle", DataSets = ds1, IsApproved = true, IsFeatured = true };
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, Title = "atitle", DataSets = null, IsApproved = true, IsFeatured = true };
            _repositoryDataSetSchema.Add(schema2);
            var ds3 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, Title = "schemaurl", DataSets = ds3, IsApproved = false, IsFeatured = true};
            _repositoryDataSetSchema.Add(schema3);

            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetFeatured();
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);

        }
        [TestMethod]
        public void GetFeatured_returns_all_featured_orderedby_dateupdated_ascending()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false },  DataSets = ds1, IsApproved = true, IsFeatured = true, DateUpdated = new DateTime(2000,1,1)};
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, DataSets = ds1, IsApproved = true, IsFeatured = true, DateUpdated = new DateTime(1998, 1, 1) };
            _repositoryDataSetSchema.Add(schema2);
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, DataSets = ds1, IsApproved = false, IsFeatured = true, DateUpdated = new DateTime(2001, 1, 1) };
            _repositoryDataSetSchema.Add(schema3);

            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetFeatured();
            //assert
            Assert.AreEqual(new DateTime(1998,1,1), result[0].DateUpdated);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);

        }


        [TestMethod]
        public void GetFeatured_returns_all_featured_takes_only_top_3()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateUpdated = new DateTime(2011, 1, 1) }, new DataSetDetail() { DateUpdated = new DateTime(2010, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, DataSets = ds1, IsApproved = true, IsFeatured = true, DateUpdated = new DateTime(2000, 1, 1) };
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, DataSets = ds1, IsApproved = true, IsFeatured = true, DateUpdated = new DateTime(1998, 1, 1) };
            _repositoryDataSetSchema.Add(schema2);
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, DataSets = ds1, IsApproved = true, IsFeatured = true, DateUpdated = new DateTime(2001, 1, 1) };
            _repositoryDataSetSchema.Add(schema3);
            var schema4 = new DataSetSchema() { Category = new Category() { Title = "categoryurl", IsDisabled = false }, DataSets = ds1, IsApproved = true, IsFeatured = true, DateUpdated = new DateTime(2010, 1, 1) };
            _repositoryDataSetSchema.Add(schema4);

            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetFeatured();
            //assert
            Assert.AreEqual(new DateTime(2001, 1, 1), result[2].DateUpdated);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);
            _repositoryDataSetSchema.Delete(schema4);

        } 

        [TestMethod]
        public void GetOverDue_returns_DataSetSchemaList_where_upload_frequency_is_more_than_zero()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = new DateTime(2011, 1, 1) }};
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, IsApproved = true, UploadFrequency = 0};
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, IsApproved = true, UploadFrequency = 1, DataSets = ds1 };
            _repositoryDataSetSchema.Add(schema2);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            
            //act
            var result = sut.GetOverDue();
            //assert
            Assert.AreEqual(1, result[0].UploadFrequency);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
        }

        [TestMethod]
        public void GetOverDue_returns_DataSetSchemaList_where_schema_is_online()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = new DateTime(2011, 1, 1) } };
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, IsApproved = true, UploadFrequency = 0 };
            _repositoryDataSetSchema.Add(schema1);
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, IsApproved = false, UploadFrequency = 1, DataSets = ds1 };
            _repositoryDataSetSchema.Add(schema2);
            var schema3 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, IsApproved = true, UploadFrequency = 2, DataSets = ds1 };
            _repositoryDataSetSchema.Add(schema3);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);

            //act
            var result = sut.GetOverDue();
            //assert
            Assert.AreEqual(2, result[0].UploadFrequency);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            _repositoryDataSetSchema.Delete(schema3);
        }


        [TestMethod]
        public void GetOverDue_returns_DataSetSchemaList_where_lastuploadeddate_lessthan_overdue_date_and_date_last_reminded_is_lessthan_overdue_date()
        {
            //arrange
            var ds1 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = DateTime.Now.AddDays(-4) } }; // overdue date is alway -3 ( so this is not overdue)
            var schema1 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, IsApproved = true, UploadFrequency = 1 , DataSets = ds1};
            _repositoryDataSetSchema.Add(schema1);
            var ds2 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = new DateTime(1998,1,1) }  };
            var schema2 = new DataSetSchema() { Category = new Category() { Title = "category", IsDisabled = false }, DateLastReminded = new DateTime(1998, 1, 1), IsApproved = true, UploadFrequency = 2, DataSets = ds2 };
            _repositoryDataSetSchema.Add(schema2);
            
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                               _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.GetOverDue();
            //assert
            Assert.AreEqual(2, result[0].UploadFrequency);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
            _repositoryDataSetSchema.Delete(schema2);
            
        }
        

        [TestMethod]
        public void Get_when_schema_id_is_found_return_datasetschema()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id=1};
            _repositoryDataSetSchema.Add(schema1);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.Get(1);
            //assert
            Assert.AreEqual(1, result.Id);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
        }

        [TestMethod]
        public void Get_when_schema_id_is_not_found_return_null()
        {
            //arrange
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.Get(1);
            //assert
            Assert.AreEqual(null, result);
            //cleanup
            
        }

        [TestMethod]
        public void Get_when_friendlyurl_is_not_found_return_null()
        {
            //arrange
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.Get("friendlyurl");
            //assert
            Assert.AreEqual(null, result);
            //cleanup

        }

        [TestMethod]
        public void Get_when_friendlyurl_is_found_return_datasetschema()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 1, Title = "friendlyurl"};
            _repositoryDataSetSchema.Add(schema1);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.Get("friendlyurl");
            //assert
            Assert.AreEqual(1, result.Id);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
        }
        [TestMethod]
        public void Get_when_friendlyurl_and_categoryurl_is_found_return_datasetschema()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 1, Title = "friendlyurl" , Category = new Category(){Title = "categoryurl"}};
            _repositoryDataSetSchema.Add(schema1);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.Get("categoryurl", "friendlyurl");
            //assert
            Assert.AreEqual(1, result.Id);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
        }

        [TestMethod]
        public void Get_when_friendlyurl_and_categoryurl_is_not_found_return_null()
        {
            //arrange
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            //act
            var result = sut.Get("categoryurl", "friendlyurl");
            //assert
            Assert.AreEqual(null, result); 
            //cleanup
        }

        [TestMethod]
        public void Get_when_SchemaDefinition_is_not_null_return_definition_columns_ordered_by_DefaultDisplayWeight()
        {
            //arrange
            var ds = new DataSetSchema()
                {Title= "schemaname",
                    Definition = new DataSetSchemaDefinition()
                        {
                            Columns = new List<DataSetSchemaColumn>()
                                {
                                    new DataSetSchemaColumn() {Id = 30, DefaultDisplayWeight = 2},
                                    new DataSetSchemaColumn() {Id = 29, DefaultDisplayWeight = 0},
                                    new DataSetSchemaColumn() {Id = 33, DefaultDisplayWeight = 1},
                                }
                        }
                };
            _repositoryDataSetSchema.Add(ds);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            
            //act
            var result = sut.Get("schemaname");

            //assert
            Assert.AreEqual(result.Definition.Columns[0].Id, 29);

        }

        [TestMethod]
        public void Search_when_searchterm_is_found_in_title_return_results()
        {
            //arrange
            var ds2 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = new DateTime(1998, 1, 1) } };
            var schema1 = new DataSetSchema() { Id = 1, Title = "title1" , DataSets = ds2, IsApproved = true, Category = new Category(){IsDisabled = false,}};
            _repositoryDataSetSchema.Add(schema1);

            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            
            //act
            var result = sut.Search("title");
            //assert
            Assert.AreEqual("title1", result[0].Title);
            //cleanup
        }

        [TestMethod]
        public void Search_when_searchterm_is_found_in_description_return_results()
        {
            //arrange
            var ds2 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = new DateTime(1998, 1, 1) } };
            var schema1 = new DataSetSchema() { Id = 1, Title="", Description = "description and more", DataSets = ds2, IsApproved = true, Category = new Category() { IsDisabled = false, } };
            _repositoryDataSetSchema.Add(schema1);

            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            var result = sut.Search("descripti");
            //assert
            Assert.AreEqual("description and more", result[0].Description);
            //cleanup
        }


        [TestMethod]
        public void Search_when_searchterm_is_found_in_shortdescription_return_results()
        {
            //arrange
            var ds2 = new List<DataSetDetail>() { new DataSetDetail() { DateCreated = new DateTime(1998, 1, 1) } };
            var schema1 = new DataSetSchema() { Id = 1, Title = "", Description = "", ShortDescription = "my short description",DataSets = ds2, IsApproved = true, Category = new Category() { IsDisabled = false, } };
            _repositoryDataSetSchema.Add(schema1);

            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            var result = sut.Search("descripti");
            //assert
            Assert.AreEqual("my short description", result[0].ShortDescription);
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "A schema named sampletitle already exists")]
        public void Create_when_datasetschema_title_exists_will_throw_exception_a_schema_already_exist()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 1, Title = "sampletitle"};
            _repositoryDataSetSchema.Add(schema1);

            var schematoAdd = new DataSetSchema() {Title = "sampletitle"};
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            sut.Create(schematoAdd);
            //assert - no need as throwing exception
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
        }

        [TestMethod]
        public void Create_when_schema_is_externaldata_will_save_in_schema_definition_in_repository()
        {
            //arrange

            var schematoAdd = new DataSetSchema() { Id = 10, Title = "sampletitle", IsExternalData = true};
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            sut.Create(schematoAdd);
            var result = _repositoryDataSetSchema.GetQuery().FirstOrDefault(x => x.Id == 10);
            //assert
            Assert.AreEqual("sampletitle", result.Title);
            //cleanup
            _repositoryDataSetSchema.Delete(schematoAdd);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "A table named sampletitle2 already exists in the database")]
        public void
            Create_when_SqlTableExists_with_the_tablename_throws_an_exception_a_table_named_already_exist_in_the_database
            ()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaDefinitionService>();
            mock.Setup(x => x.SqlTableExists("DS_sampletitle2")).Returns(true);
            _dataSetSchemaDefinitionService = mock.Object;  
                var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                            _repositoryDataSetDetail, _sqlTableUtility);
            var schematoAdd = new DataSetSchema() { Id = 11, Title = "sampletitle2" };

            //act
            sut.Create(schematoAdd);
            //assert
            //cleanup
            _dataSetSchemaDefinitionService = null;

        }

        [TestMethod]
        
        public void
            Create_when_no_errors_will_add_schema_with_new_schemadefinition_with_tablename
            ()
        {
            //arrange
            var mock = new Mock<IDataSetSchemaDefinitionService>();
            mock.Setup(x => x.SqlTableExists("DS_sampletitle2")).Returns(false);
            _dataSetSchemaDefinitionService = mock.Object;
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                                        _repositoryDataSetDetail, _sqlTableUtility);
            var schematoAdd = new DataSetSchema() { Id = 11, Title = "sampletitle2" };

            //act
            sut.Create(schematoAdd);
            var result = _repositoryDataSetSchema.GetQuery().FirstOrDefault(x => x.Id == 11);
            //assert
            Assert.AreEqual("DS_sampletitle2", result.Definition.TableName);
            //cleanup
            _dataSetSchemaDefinitionService = null;

        }

        [TestMethod]
        public void Save_when_original_schema_definitionfromurl_is_null_schema_title_is_saved()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 11, Title = "sampletitle" , SchemaDefinitionFromUrl = null};
            _repositoryDataSetSchema.Add(schema1);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);
            var schematoAdd = new DataSetSchema() { Id = 11, Title = "changedtitle" };
            //act
           sut.Save(schematoAdd);
            var result = _repositoryDataSetSchema.GetQuery().FirstOrDefault(x => x.Id == 11);
            //assert
            Assert.AreEqual("changedtitle", result.Title);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);
        }

        [TestMethod]
        public void Delete_when_original_schema_is_found_the_schema_is_marked_as_disabled()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 11, Title = "sampletitle", Category = new Category(){Title = "categorytitle"}};
            _repositoryDataSetSchema.Add(schema1);
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);
            
            //act
            sut.Delete("categorytitle", "sampletitle");

            var result = _repositoryDataSetSchema.GetQuery().FirstOrDefault(x => x.Id == 11);
            //assert
            Assert.AreEqual(true, result.IsDisabled);
            //cleanup
            _repositoryDataSetSchema.Delete(schema1);

        }

        [TestMethod]
        public void DeleteAll_when_original_schema_is_found_the_schema_is_Removed_from_repository()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 11, Title = "sampletitle", Category = new Category() { Title = "categorytitle" } };
            _repositoryDataSetSchema.Add(schema1);
            _sqlTableUtility = new Mock<ISqlTableUtility>().Object;
            var sut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            sut.DeleteAll("categorytitle", "sampletitle");

            var result = _repositoryDataSetSchema.GetQuery().FirstOrDefault(x => x.Id == 11);
            //assert
            Assert.AreEqual(null, result);
            //cleanup
            

        }


        [TestMethod]
        public void
            Approve_when_categoryurl_and_schema_url_is_found_will_set_schema_IsApproved_true()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 13, Title = "schemaurl", Category = new Category() { Title = "categoryurl" }, IsApproved = false};
            _repositoryDataSetSchema.Add(schema1);

            var mut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            mut.Approve("categoryurl", "schemaurl");
            var result = _repositoryDataSetSchema.GetQuery().First(x => x.Id == 13);
            //assert
            Assert.AreEqual(true, result.IsApproved);
            //cleanup
        }


        [TestMethod]
        public void
            Approve_when_categoryurl_and_schema_url_is_not_found_will_not_set_schema_IsApproved()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 13, Title = "schemaurl", Category = new Category() { Title = "categoryurl" }, IsApproved = false };
            _repositoryDataSetSchema.Add(schema1);

            var mut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            mut.Approve("categoryurl2", "schemaurl");
            var result = _repositoryDataSetSchema.GetQuery().First(x => x.Id == 13);
            //assert
            Assert.AreEqual(false, result.IsApproved);
            //cleanup
        }


        [TestMethod]
        public void
            Approve_when_categoryurl_and_schema_url_is_found_will_set_schema_IsDisabled_false()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 13, Title = "schemaurl", Category = new Category() { Title = "categoryurl" }, IsDisabled = true };
            _repositoryDataSetSchema.Add(schema1);

            var mut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            mut.Approve("categoryurl", "schemaurl");
            var result = _repositoryDataSetSchema.GetQuery().First(x => x.Id == 13);
            //assert
            Assert.AreEqual(false, result.IsDisabled);
            //cleanup
        }


        [TestMethod]
        public void
            Approve_when_categoryurl_and_schema_url_is_not_found_will_not_set_schema_IsDisabled()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 13, Title = "schemaurl", Category = new Category() { Title = "categoryurl" }, IsDisabled = true };
            _repositoryDataSetSchema.Add(schema1);

            var mut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            mut.Approve("categoryurl2", "schemaurl");
            var result = _repositoryDataSetSchema.GetQuery().First(x => x.Id == 13);
            //assert
            Assert.AreEqual(true, result.IsDisabled);
            //cleanup
        }


        [TestMethod]
        public void
            Enable_when_categoryurl_and_schema_url_is_found_will_set_schema_IsDisabled_false()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 13, Title = "schemaurl", Category = new Category() { Title = "categoryurl" }, IsDisabled = true };
            _repositoryDataSetSchema.Add(schema1);

            var mut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            mut.Enable("categoryurl", "schemaurl");
            var result = _repositoryDataSetSchema.GetQuery().First(x => x.Id == 13);
            //assert
            Assert.AreEqual(false, result.IsDisabled);
            //cleanup
        }

        [TestMethod]
        public void
            Enable_when_categoryurl_and_schema_url_is_not_found_will_not_set_schema_IsDisabled()
        {
            //arrange
            var schema1 = new DataSetSchema() { Id = 13, Title = "schemaurl", Category = new Category() { Title = "categoryurl" }, IsDisabled = true };
            _repositoryDataSetSchema.Add(schema1);

            var mut = new DataSetSchemaService(_repositoryDataSetSchema, _dataSetSchemaDefinitionService,
                            _repositoryDataSetDetail, _sqlTableUtility);

            //act
            mut.Enable("categoryurl2", "schemaurl");
            var result = _repositoryDataSetSchema.GetQuery().First(x => x.Id == 13);
            //assert
            Assert.AreEqual(true, result.IsDisabled);
            //cleanup
        }


    }
}
