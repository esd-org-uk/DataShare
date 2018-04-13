using System;
using System.Collections.Generic;
using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class DataSetSchemaDefinitionServiceTests
    {
        private MemoryRepository<DataSetSchemaDefinition> _repository;

        [TestInitialize]
        public void TestInit()
        {
            _repository = new MemoryRepository<DataSetSchemaDefinition>();

            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();

              }

           );
        }
        /// <summary>
        /// not too sure if this test is needed but i'm only testing for interaction that the if fires. 
        /// </summary>
        [TestMethod]
        public void Delete_when_id_not_found_in_repository_and_definition_is_null()
        {

            //arrange
            var dsd = new DataSetSchemaDefinition() {Id = 2};
            _repository.Add(dsd);
            var beforeDelete = _repository.GetAll().Count();
            var mut = new DataSetSchemaDefinitionService(_repository);
            //act
            mut.Delete(1);
            var result = _repository.GetAll().Count();
            //assert
            Assert.AreEqual(beforeDelete, result);
            //cleanup
            _repository.Delete(dsd);

        }
    }
}
