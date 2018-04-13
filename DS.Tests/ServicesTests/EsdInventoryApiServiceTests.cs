using System;
using System.Collections.Generic;
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
    public class EsdInventoryApiServiceTests
    {
        private IRepository<Category> _repositoryCategory;
        private IEsdFunctionService _esdfunctionservice;

        [TestInitialize]
        public void TestInit()
        {
            _repositoryCategory = new MemoryRepository<Category>();
            //_fakecacheprovider = new FakeCacheProvider();
            ObjectFactory.Initialize(
                x => { x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>(); }
                );
        }

        [TestMethod]
        public void
            GetInventoryDataset_for_each_category_when_schema_is_not_null_and_count_more_than_zero_returns_list_of_inventorydataset_equals_to_count_of_schema
            ()
        {
            //arrange
            var schemas = new List<DataSetSchema>() {new DataSetSchema() {Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true}};
            var cat = new Category() {Schemas = schemas, Title = "cattitle"};
            _repositoryCategory.Add(cat);
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();

            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repositoryCategory.Delete(cat);
        }

        [TestMethod]
        public void
            GetInventoryDataset_for_each_category_when_schema_is_null_returns_empty_list_of_inventorydataset()
        {
            //arrange
            
            var cat = new Category() { Schemas = null, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();

            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup
            _repositoryCategory.Delete(cat);

        }
        [TestMethod]
        public void
            GetInventoryDataset_for_each_category_when_schema_is_not_null_and_schema_count_zero_returns_empty_list_of_inventorydataset()
        {
            //arrange
            var schemas = new List<DataSetSchema>();
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup
            _repositoryCategory.Delete(cat);

        }


        [TestMethod]
        public void
            GetInventoryDataset_when_linkedfunction_type_is_Function_returns_subjects_url_for_function()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true } };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() {new EsdFunctionServiceEntity() {Type = "Function", Url="http://functionurl"}});
            _esdfunctionservice = mock.Object;
                var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual("http://functionurl", result[0].Subjects[0].Function);
            //cleanup
            _repositoryCategory.Delete(cat);

        }
        [TestMethod]
        public void
            GetInventoryDataset_when_linkedfunction_type_is_not_Function_returns_subjects_url_function_empty_string()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true } };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() { new EsdFunctionServiceEntity() { Type = "NotFunction", Url = "http://functionurl" } });
            _esdfunctionservice = mock.Object;
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual("", result[0].Subjects[0].Function);
            //cleanup
            _repositoryCategory.Delete(cat);

        }


        [TestMethod]
        public void
            GetInventoryDataset_when_linkedfunction_type_is_Service_returns_subjects_url_for_service()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true } };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() { new EsdFunctionServiceEntity() { Type = "Service", Url = "http://serviceurl" } });
            _esdfunctionservice = mock.Object;
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual("http://serviceurl", result[0].Subjects[0].Service);
            //cleanup
            _repositoryCategory.Delete(cat);

        }
        [TestMethod]
        public void
            GetInventoryDataset_when_linkedfunction_type_is_not_Service_returns_subjects_url_empty_string_for_service()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true } };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() { new EsdFunctionServiceEntity() { Type = "NotService", Url = "http://serviceurl" } });
            _esdfunctionservice = mock.Object;
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual("", result[0].Subjects[0].Service);
            //cleanup
            _repositoryCategory.Delete(cat);

        }

        [TestMethod]
        public void
            GetInventoryDataset_when_linkedfunction_type_is_Service_returns_subjects_scheme_as_subjectservice()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true } };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() { new EsdFunctionServiceEntity() { Type = "Service"} });
            _esdfunctionservice = mock.Object;
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual("subject.service", result[0].Subjects[0].Scheme);
            //cleanup
            _repositoryCategory.Delete(cat);

        }
        [TestMethod]
        public void
            GetInventoryDataset_when_linkedfunction_type_is_not_Service_returns_subject_scheme_subjectfunction()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", IsDisabled = false, IsApproved = true } };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() { new EsdFunctionServiceEntity() { Type = "NotService"} });
            _esdfunctionservice = mock.Object;
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual("subject.function", result[0].Subjects[0].Scheme);
            //cleanup
            _repositoryCategory.Delete(cat);

        }

        [TestMethod]
        public void
            GetInventoryDataset_when_schema_datasets_is_null_datelastuploadedto_is_null_returns_modified_with_value_datetime_1900_1_1()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle" , DataSets = null, IsApproved = true, IsDisabled = false} };
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var mock = new Mock<IEsdFunctionService>();
            mock.Setup(x => x.GetLinkedFunctionsServices(1))
                .Returns(new List<EsdFunctionServiceEntity>() { new EsdFunctionServiceEntity() { Type = "NotService" } });
            _esdfunctionservice = mock.Object;
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual(new DateTime(1900,1,1),  result[0].Modified);
            //cleanup
            _repositoryCategory.Delete(cat);

        }


        [TestMethod]
        public void GetInventoryDataset_when_scehma_is_disabled_excludes_disabled_schema_from_the_return_list()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", DataSets = null, IsDisabled = true, IsApproved = true},
            new DataSetSchema() { Id = 2, Title = "schematitle", DataSets = null, IsDisabled = false, IsApproved = true}};
            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repositoryCategory.Delete(cat);

        }

        [TestMethod]
        public void GetInventoryDataset_when_scehma_is_Isapprovied_includes_approved_schema_to_the_return_list()
        {
            //arrange
            var schemas = new List<DataSetSchema>() { new DataSetSchema() { Id = 1, Title = "schematitle", DataSets = null, IsApproved = true},
            new DataSetSchema() { Id = 2, Title = "schematitle", DataSets = null, IsApproved = true},
            new DataSetSchema() { Id = 3, Title = "schematitle", DataSets = null, IsApproved = false}};

            var cat = new Category() { Schemas = schemas, Title = "cattitle" };
            _repositoryCategory.Add(cat);
            var sut = new EsdInventoryApiService(_esdfunctionservice, _repositoryCategory);
            //act
            var result = sut.GetInventoryDataset();
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
            _repositoryCategory.Delete(cat);

        }

    }
}
