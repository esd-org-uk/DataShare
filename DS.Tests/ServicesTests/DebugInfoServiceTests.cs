using System;
using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class DebugInfoServiceTests
    {
        private IRepository<DebugInfo> _debugInfoRepository;

        [TestInitialize]
        public void TestInit()
        {
            _debugInfoRepository = new MemoryRepository<DebugInfo>();
            
            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();

              }

           );
        }

        [TestMethod]
        public void GetAll_will_return_all_type_which_is_not_developer()
        {
            //arrange
            var debug1 = new DebugInfo() {Type = "Developer"};
            var debug2 = new DebugInfo() { Type = "All" };
            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.GetAll().ToList();
            //assert
            Assert.AreNotEqual("Developer", result[0].Type);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);

        }

        [TestMethod]
        public void GetAll_will_return_debug_info_ordered_by_created_date_latest_info_first()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "All" , DateCreated = new DateTime(2010, 10, 1)};
            var debug2 = new DebugInfo() { Type = "All", DateCreated = new DateTime(2012, 10, 1) };
            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.GetAll().ToList();
            //assert
            Assert.AreEqual(new DateTime(2012,10,1), result[0].DateCreated);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);

        }



        [TestMethod]
        public void Get_when_enum_type_is_all_returns_all_debug_info_not_developer()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "Developer" };
            var debug2 = new DebugInfo() { Type = "All" };
            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.All).ToList();
            //assert
            Assert.AreNotEqual("Developer", result[0].Type);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }



        [TestMethod]
        public void Get_when_enum_type_is_all_returns_debug_info_ordered_by_created_date_latest_info_first()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "All", DateCreated = new DateTime(2010, 10, 1) };
            var debug2 = new DebugInfo() { Type = "All", DateCreated = new DateTime(2012, 10, 1) }; 
            
            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.All).ToList();
            //assert
            Assert.AreEqual(new DateTime(2012, 10, 1), result[0].DateCreated);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }



        [TestMethod]
        public void Get_when_enum_type_is_EmailSent_returns_debug_info_selected_to_check()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "EmailSent" };
            var debug2 = new DebugInfo() { Type = "All"};

            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.EmailSent).ToList();
            //assert
            Assert.AreEqual("EmailSent", result[0].Type);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }
        [TestMethod]
        public void Get_when_enum_type_is_EmailSent_returns_debug_info_ordered_by_created_date_latest_info_first()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "EmailSent", DateCreated = new DateTime(2010, 10, 1) };
            var debug2 = new DebugInfo() { Type = "EmailSent", DateCreated = new DateTime(2012, 10, 1) }; 


            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.EmailSent).ToList();
            //assert
            Assert.AreEqual(new DateTime(2012, 10, 1), result[0].DateCreated);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }


        [TestMethod]
        public void Get_when_enum_type_is_FolderWatchTriggered_returns_debug_info_selected_to_check()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "FolderWatchTriggered" };
            var debug2 = new DebugInfo() { Type = "All" };

            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.FolderWatchTriggered).ToList();
            //assert
            Assert.AreEqual("FolderWatchTriggered", result[0].Type);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }
        [TestMethod]
        public void Get_when_enum_type_is_FolderWatchTriggered_returns_debug_info_ordered_by_created_date_latest_info_first()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "FolderWatchTriggered", DateCreated = new DateTime(2010, 10, 1) };
            var debug2 = new DebugInfo() { Type = "FolderWatchTriggered", DateCreated = new DateTime(2012, 10, 1) };


            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.FolderWatchTriggered).ToList();
            //assert
            Assert.AreEqual(new DateTime(2012, 10, 1), result[0].DateCreated);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }


        [TestMethod]
        public void Get_when_enum_type_is_Developer_returns_debug_info_selected_to_check()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "Developer" };
            var debug2 = new DebugInfo() { Type = "All" };

            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.Developer).ToList();
            //assert
            Assert.AreEqual("Developer", result[0].Type);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }
        [TestMethod]
        public void Get_when_enum_type_is_Developer_returns_debug_info_ordered_by_created_date_latest_info_first()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "Developer", DateCreated = new DateTime(2010, 10, 1) };
            var debug2 = new DebugInfo() { Type = "Developer", DateCreated = new DateTime(2012, 10, 1) };


            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.Developer).ToList();
            //assert
            Assert.AreEqual(new DateTime(2012, 10, 1), result[0].DateCreated);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }

        [TestMethod]
        public void Get_when_enum_type_is_Error_returns_debug_info_selected_to_check()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "Error" };
            var debug2 = new DebugInfo() { Type = "All" };

            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.Error).ToList();
            //assert
            Assert.AreEqual("Error", result[0].Type);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }
        [TestMethod]
        public void Get_when_enum_type_is_Error_returns_debug_info_ordered_by_created_date_latest_info_first()
        {
            //arrange
            var debug1 = new DebugInfo() { Type = "Error", DateCreated = new DateTime(2010, 10, 1) };
            var debug2 = new DebugInfo() { Type = "Error", DateCreated = new DateTime(2012, 10, 1) };


            _debugInfoRepository.Add(debug1);
            _debugInfoRepository.Add(debug2);

            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            var result = mut.Get(DebugInfoTypeEnum.Error).ToList();
            //assert
            Assert.AreEqual(new DateTime(2012, 10, 1), result[0].DateCreated);
            //cleanup
            _debugInfoRepository.Delete(debug1);
            _debugInfoRepository.Delete(debug2);
        }

        [TestMethod]
        public void Add_will_set_info_created_by_to_empty_string()
        {
            //arrange
            var debug1 = new DebugInfo() { Id=1, CreatedBy = "test-to-be-set-to-empty"};
            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            mut.Add(debug1);
            var result = _debugInfoRepository.GetQuery().First(x => x.Id == 1);
            //assert
            Assert.AreEqual("", result.CreatedBy);
            //cleanup
            _debugInfoRepository.Delete(debug1);
        }

        [TestMethod]
        public void Add_will_set_info_updated_by_to_empty_string()
        {
            //arrange
            var debug1 = new DebugInfo() { Id = 1, UpdatedBy = "test-to-be-set-to-empty" };
            var mut = new DebugInfoService(_debugInfoRepository);
            //act
            mut.Add(debug1);
            var result = _debugInfoRepository.GetQuery().First(x => x.Id == 1);
            //assert
            Assert.AreEqual("", result.UpdatedBy);
            //cleanup
            _debugInfoRepository.Delete(debug1);
        }
    }
}
