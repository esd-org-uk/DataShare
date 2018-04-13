using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using DS.DL.DataContext;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Domain.WcfRestService;
using DS.Service;
using DS.Service.WcfRestService;
using DS.Tests.Fakes;
using DS.WebUI.Controllers;
using DS.WebUI.Controllers.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests
{
    [TestClass]
    public class ControllerTests
    {
        private IDataSetSchemaService _dataSetSchemaService;
        private IDataSetSchemaDefinitionService _dataSetSchemaDefinitionService;

        //private int councilId = 1;
        private IContactService _contactService;
        private IDataSetDetailService _dataSetDetailService;
        private ISystemConfigurationService _systemConfigurationService;
        private ICategoryService _categoryService;
        #region Setup

        [TestInitialize]
        public void SetUp()
        {

            var mock3 = new Mock<ISystemConfigurationService>();
            mock3.Setup(x => x.GetSystemConfigurations()).Returns(new SystemConfigurationObject() { SettingId = 1 });
            _systemConfigurationService = mock3.Object;

            var schemas = new List<DataSetSchema>(){new DataSetSchema(){Title = "datasetschema1"}};
            var mock = new Mock<ICategoryService>();
            mock.Setup(x => x.GetAll(false)).Returns(new List<Category>() { new Category() { Title = "SampleCat1", Schemas = schemas} });
            mock.Setup(x => x.GetByFriendlyUrl("samplecat1", false)).Returns(new List<DataSetSchema>() { new DataSetSchema() { Title = "sampledatasetschema1" } });
            mock.Setup(x => x.GetByFriendlyUrlIsOnline("samplecat1")).Returns(new RestSchema[]{new RestSchema(), });
            _categoryService = mock.Object;


            var mock2 = new Mock<IDataSetSchemaService>();
            mock2.Setup(x => x.GetByFriendlyUrl("samplecat1", "sampledatasetschema1", false)).Returns(new DataSetDetail[] { new DataSetDetail() { Title = "datasetdetail1" }, });
            mock2.Setup(x => x.Get("sampledatasetschema1")).Returns(new DataSetSchema() { Description = "emptydesc", Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() } });
            mock2.Setup(x => x.Repository.GetQuery())
                 .Returns(new List<DataSetSchema>() { new DataSetSchema() { Title = "datasetschema1", IsDisabled = false, IsApproved = true, Category = new Category() { Title = "categoryds1", IsDisabled = false, } } }.AsQueryable);
            _dataSetSchemaService = mock2.Object;

            var datatable = new DataTable("datatablename");
            datatable.Columns.Add(new DataColumn("TransactionDate", typeof (DateTime)));
            datatable.Columns.Add(new DataColumn("Amount", typeof (double)));
            datatable.Columns.Add(new DataColumn("Service", typeof(string)));
            

            var row = datatable.NewRow();
            row["TransactionDate"] = new DateTime(2009, 11 , 30);
            row["Service"] = "Children's Services - Commissioning & Social Work";
            row["Amount"] = 1000;
            datatable.Rows.Add(row);

            //var row2 = datatable.NewRow();
            //row2["TransactionDate"] = new DateTime(2009, 11, 30);
            //row2["Amount"] = 1000;
            //datatable.Rows.Add(row2);

            var mock4 = new Mock<IDataSetDetailService>();
            mock4.Setup(x => x.GetData("datasetdetail1", "sampledatasetschema1")).Returns(new DataTable());
            mock4.SetReturnsDefault(new ViewControllerData()
                {
                    Count = 1,
                    Data= datatable
                });
            
            _dataSetDetailService = mock4.Object;
            
            // Setup StructureMap to determine the concrete repository pattern to use.
            ObjectFactory.Initialize(
               x =>
               {
                   //x.For<IUnitOfWorkFactory>().Use<EFUnitOfWorkFactory>();
                   x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();
                   x.For(typeof(IRepository<>)).Use(typeof(MemoryRepository<>));
                   
                   x.For<ICategoryService>().Use(_categoryService);
                   x.For<IDataSetSchemaService>().Use(_dataSetSchemaService);
                   x.For<IDataSetDetailService>().Use(_dataSetDetailService);
                   x.For<IDataTableSerializer>().Use<CustomDataTableToJsonSerializer>().Named("json");
                   x.For<IDataTableSerializer>().Use<CustomDataTableToXmlSerializer>().Named("xml");
               }
            );

            //// Select an Entity Framework model to use with the factory.
            //EFUnitOfWorkFactory.SetObjectContext(() => new DataShareContext());

            ////Never recreate the database
            //Database.SetInitializer<DataShareContext>(null);
            //_dataSetDetailService = new DataSetDetailService(_systemConfigurationService, ObjectFactory.GetInstance<IRepository<DataSetDetail>>()
            //    , ObjectFactory.GetInstance<IDataSetSchemaService>()
            //    , ObjectFactory.GetInstance<IDataSetDetailSqlRepo>()
            //    );
            //_dataSetSchemaDefinitionService = new DataSetSchemaDefinitionService(ObjectFactory.GetInstance<IRepository<DataSetSchemaDefinition>>());
            //_dataSetSchemaService = new DataSetSchemaService(ObjectFactory.GetInstance<IRepository<DataSetSchema>>(), 
            //    _dataSetSchemaDefinitionService
            //    , ObjectFactory.GetInstance<IRepository<DataSetDetail>>()
            //    , ObjectFactory.GetInstance<ISqlTableUtility>());
            //_contactService = new ContactService(ObjectFactory.GetInstance<IRepository<Contact>>());
            //_categoryService = new CategoryService(ObjectFactory.GetInstance<IRepository<Category>>(), new FakeCacheProvider());
        }
        #endregion



        #region Home

        #endregion

        #region Download DataSet
        ///moved to DownloadControllerTests
        //[TestMethod]
        //public void Download_Returns_List_Of_Categories()
        //{
        //    var customersController = new DownloadController(_dataSetSchemaService, _dataSetDetailService, _categoryService, _systemConfigurationService);

        //    // Act
        //    var result = customersController.Index(null, null, null, null, null, null, null, null) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result, "Should have returned a ViewResult");
        //    Assert.AreEqual("Index", result.ViewName, "View name should have been Index");

        //    var data = (IList<Category>)result.Model;
        //    Assert.IsTrue(data.Count > 0, "Shuuld have returned a list of categories");
        //}
        ///moved to DownloadControllerTests
        //[TestMethod]
        //public void Download_By_Category_Returns_List_Of_Schemas()
        //{
        //    var customersController = new DownloadController(_dataSetSchemaService, _dataSetDetailService, _categoryService, _systemConfigurationService);

        //    var firstCategory = _categoryService.GetAll(false)[0];
        //    // Act
        //    var result = customersController.Index(firstCategory.FriendlyUrl, null, null, null, null, null, null, null) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result, "Should have returned a ViewResult");
        //    Assert.AreEqual("Schema", result.ViewName, "View name should have been Schema");

        //    var data = (IList<DataSetSchema>)result.Model;
        //    Assert.IsTrue(data != null && data.Count > 0, "Shuuld have returned a list of categories");
        //}
        
        ///moved to downloadcontrollertests
        //[TestMethod]
        //public void Download_By_Schema_Returns_List_Of_DataSets()
        //{
        //    var customersController = new DownloadController(_dataSetSchemaService, _dataSetDetailService, _categoryService, _systemConfigurationService);

        //    var firstCategory = _categoryService.GetAll(false)[0];
        //    var schemas = _categoryService.GetByFriendlyUrl(firstCategory.FriendlyUrl, false);
        //    // Act
        //    var result = customersController.Index(firstCategory.FriendlyUrl, schemas[0].FriendlyUrl, null, null, null, null, null, null) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result, "Should have returned a ViewResult");
        //    Assert.AreEqual("DataSet", result.ViewName, "View name should have been DataSet");

        //    var data = (IList<DataSetDetail>)result.Model;
        //    Assert.IsTrue(data != null && data.Count > 0, "Shuuld have returned a list of DataSets");
        //}
        ///moved to downloadcontrollertests
        //[TestMethod]
        //public void Download_DataSet_As_Xml_Returns_XmlResult()
        //{
        //    var customersController = new DownloadController(_dataSetSchemaService, _dataSetDetailService, _categoryService, _systemConfigurationService);

        //    var firstCategory = _categoryService.GetAll(false)[0];
        //    var schema = _categoryService.GetByFriendlyUrl(firstCategory.FriendlyUrl, false)[0];
        //    var dataset = _dataSetSchemaService.GetByFriendlyUrl(firstCategory.FriendlyUrl, schema.FriendlyUrl)[0];

        //    // Act
        //    var result = customersController.Index(firstCategory.FriendlyUrl, schema.FriendlyUrl, dataset.FriendlyUrl, "xml", null, null, null, null) as XmlResult;

        //    // Assert
        //    Assert.IsNotNull(result, "Should have returned an XmlResult");
        //    Assert.IsTrue(((XmlDocument)result.ObjectToSerialize).HasChildNodes, "Xml document was empty");
        //}


        ///this test does not make any sense. - 
        //[TestMethod]
        //public void Download_DataSet_Filter_Returns_A_List_Of_DataSetDetail()
        //{
        //    var customersController = new DownloadController(_dataSetSchemaService, _dataSetDetailService, _categoryService, _systemConfigurationService);

        //    var firstCategory = _categoryService.GetAll(false)[0];
        //    var schema = _categoryService.GetByFriendlyUrl(firstCategory.FriendlyUrl, false)[0];
        //    var dataset = _dataSetSchemaService.GetByFriendlyUrl(firstCategory.FriendlyUrl, schema.FriendlyUrl)[0];

        //    // Act
        //    var result = customersController.Index(firstCategory.FriendlyUrl, schema.FriendlyUrl, dataset.FriendlyUrl, "xml", "a", schema.Id, null, null) as ViewResult;
        //    var data = (IList<DataSetDetail>)result.Model;

        //    // Assert
        //    Assert.IsNotNull(result, "Should have returned an XmlResult");
        //    Assert.IsTrue(data != null && data.Count > 0, "Should have returned a list of DataSetDetail");
        //}


        #endregion

        #region View DataSetFile

        [TestMethod]
        public void View_DataSet_Filter_By_Column()
        {
            //var results = DataSetDetailService.SearchSchema(1,"Payment", "Category", "Local Government");
            //Assert.IsTrue(results.Descendants("Root").ToList().Count() > 0, "The dataset should of returned results"); 
        }
        #endregion

        #region API WCF Rest tests

        [TestMethod]
        public void Api_GetCategory_Returns_List()
        {
            var service = new DataShareService();
            var result = service.GetCategory();

            // Assert
            Assert.IsNotNull(result, "Should have returned IList<RestCategory> ");
            Assert.IsNotNull(result.Count() > 0, "Should have returned IList<RestCategory> ");
        }

        [TestMethod]
        public void Api_GetSchemas_Returns_List()
        {
            var firstCategory = _categoryService.GetAll(false)[0];

            var service = new DataShareService();
            var result = service.GetSchemas(firstCategory.FriendlyUrl);

            // Assert
            Assert.IsNotNull(result, "Should have returned IList<RestCategory> ");
            Assert.IsNotNull(result.Count() > 0, "Should have returned IList<RestSchema> ");

        }

        [TestMethod]
        public void Api_SearchSchemaTextContains_Returns_Data_Containing_SearchText()
        {
            var firstCategory = _categoryService.GetAll(false)[0];

            var service = new DataShareService();
            var result = service.SearchSchemaTextContains(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Service", "Commissioning");

            using (var reader = new StreamReader(result))
            {
                var xml = XElement.Parse(reader.ReadToEnd());

                var shouldHaveData = xml.Descendants("Service").Where(p => p.Value.ToLower().Contains("commissioning")).ToList();
                var shouldBeEmpty = xml.Descendants("Service").Where(p => !p.Value.ToLower().Contains("commissioning")).ToList();

                Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned records containg misc ");
                Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
            }
        }

        //[TestMethod]
        //public void Api_SearchSchemaDate_Returns_Data_With_Date_SearchText()
        //{
        //    var firstCategory = _categoryService.GetAll(false)[0];

        //    var service = new DataShareService();
            
        //    var result = service.SearchSchemaDate(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "TransactionDate", "30/11/2009", "30/11/2009");

        //    using (var reader = new StreamReader(result))
        //    {
        //        var xml = XElement.Parse(reader.ReadToEnd());

        //        var shouldHaveData = xml.Descendants("TransactionDate").Where(p => p.Value.Contains("2009-11-30")).ToList();
        //        var shouldBeEmpty = xml.Descendants("TransactionDate").Where(p => !p.Value.Contains("2009-11-30")).ToList();

        //        Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned records with a transaction date equal to 30/11/2009");
        //        Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
        //    }
        //}


        [TestMethod]
        public void Api_SearchSchemaTextEquals_Returns_Data_EqualTo_SearchTextl()
        {
            var firstCategory = _categoryService.GetAll(false)[0];

            var service = new DataShareService();
            var result = service.SearchSchemaTextEquals(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Service", "Children's Services - Commissioning & Social Work");

            using (var reader = new StreamReader(result))
            {
                var xml = XElement.Parse(reader.ReadToEnd());

                var shouldHaveData = xml.Descendants("Service").Where(p => p.Value.Contains("Children's Services - Commissioning & Social Work")).ToList();
                var shouldBeEmpty = xml.Descendants("Service").Where(p => !p.Value.Contains("Children's Services - Commissioning & Social Work")).ToList();

                Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned records equal to Children's Services - Commissioning & Social Work ");
                Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
            }
        }

        [TestMethod]
        public void Api_SearchSchemaNumberEquals_Returns_Numbers_EqualTo_SearchTextl()
        {
            var firstCategory = _categoryService.GetAll(false)[0];

            var service = new DataShareService();
            var result = service.SearchSchemaNumberEquals(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Amount", "1000");

            using (var reader = new StreamReader(result))
            {
                var xml = XElement.Parse(reader.ReadToEnd());

                var shouldHaveData = xml.Descendants("Amount").Where(p => p.Value == "1000").ToList();
                var shouldBeEmpty = xml.Descendants("Amount").Where(p => p.Value != "1000").ToList();

                Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned records matching searchtext ");
                Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing for records that do nothave searchtext  ");
            }
        }
        ////todo create test for this
        //[TestMethod]
        //public void Api_SearchSchemaNumberGreaterThan_Returns_Numbers_Greater_Than_SearchText()
        //{
        //    var firstCategory = _categoryService.GetAll(false)[0];

        //    var service = new DataShareService();
        //    var result = service.SearchSchemaNumberGreaterThan(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Amount", "1000");

        //    using (var reader = new StreamReader(result))
        //    {
        //        var xml = XElement.Parse(reader.ReadToEnd());

        //        var shouldHaveData = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) > 1000).Take(1).ToList();
        //        var shouldBeEmpty = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) <= 1000).ToList();

        //        Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned amounts greater than 1000");
        //        Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
        //    }
        //}

        [TestMethod]
        public void Api_SearchSchemaNumberGreaterThanEqualTo_Returns_Numbers_Greater_Than_Or_Equal_To_SearchText()
        {
            var firstCategory = _categoryService.GetAll(false)[0];

            var service = new DataShareService();
            var result = service.SearchSchemaNumberGreaterThanEqualTo(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Amount", "1000");

            using (var reader = new StreamReader(result))
            {
                var xml = XElement.Parse(reader.ReadToEnd());

                var shouldHaveData = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) == 1000).ToList();
                shouldHaveData.AddRange(xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) > 1000).Take(1).ToList());
                var shouldBeEmpty = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) < 1000).ToList();

                Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned amounts greater than or equal to 1000");
                Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
            }
        }

        //todo recreate test for this
        //[TestMethod]
        //public void Api_SearchSchemaNumberLessThan_Returns_Numbers_Less_Than_SearchText()
        //{
        //    var firstCategory = _categoryService.GetAll(false)[0];

        //    var service = new DataShareService();
        //    var result = service.SearchSchemaNumberLessThan(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Amount", "1000");

        //    using (var reader = new StreamReader(result))
        //    {
        //        var xml = XElement.Parse(reader.ReadToEnd());

        //        var shouldHaveData = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) < 1000).Take(1).ToList();
        //        var shouldBeEmpty = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) >= 1000).ToList();

        //        Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned amounts less than 1000");
        //        Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
        //    }
        //}

        [TestMethod]
        public void Api_SearchSchemaNumberLessThanEqualTo_Returns_Numbers_Less_Than_Or_Equal_To_SearchText()
        {
            var firstCategory = _categoryService.GetAll(false)[0];

            var service = new DataShareService();
            var result = service.SearchSchemaNumberLessThanEqualTo(firstCategory.FriendlyUrl, firstCategory.Schemas[0].FriendlyUrl, "xml", "Amount", "1000");

            using (var reader = new StreamReader(result))
            {
                var xml = XElement.Parse(reader.ReadToEnd());

                var shouldHaveData = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) <= 1000).ToList();
                var shouldBeEmpty = xml.Descendants("Amount").Where(p => Convert.ToInt32(p.Value) > 1000).ToList();

                Assert.IsTrue(shouldHaveData.Count() > 0, "Should have returned amounts less than or equal to 1000");
                Assert.IsTrue(shouldBeEmpty.Count() == 0, "Should return nothing");
            }
        }

        #endregion

        #region Contact tests
        // not testing action / or state - the state is tested in the service tests not the mvc layer.
        //[TestMethod]
        //public void ContactUs_Saves_Contact()
        //{
        //                var contact = new Contact
        //                      {
        //                          FromName = "Test",
        //                          FromEmail = "test@test.com",
        //                          Message = "Test"
        //                      };

  
        //    var contactUsController = new ContactUsController(_contactService, _categoryService, _systemConfigurationService);


        //    //Act
        //    var result = contactUsController.Index(contact);

        //    //Assert
        //    Assert.AreNotEqual(0, contact.Id, "Contact Id not created on save");
        //}
        #endregion
    }
}
