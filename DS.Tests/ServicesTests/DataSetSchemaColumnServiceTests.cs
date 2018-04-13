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
    public class DataSetSchemaColumnServiceTests
    {
        private IRepository<DataSetSchemaColumn> _repositoryDataSetSchemaColumn;
        private IRepository<DataSetSchema> _repositoryDataSetSchema;
        private IDataSetSchemaColumnSqlRepo _sqlrepo;

        [TestInitialize]
        public void TestInit()
        {
            _repositoryDataSetSchemaColumn = new MemoryRepository<DataSetSchemaColumn>();
            _repositoryDataSetSchema = new MemoryRepository<DataSetSchema>();
            //_fakecacheprovider = new FakeCacheProvider();
            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();

              }

           );
 
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "The schema schemaname does not exist")]
        public void Get_when_schema_does_not_exist_throws_exception_schema_does_not_exist()
        {
            //arrange
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            var result = service.Get("categoryname", "schemaname", "columnname");
            //assert - no need to assert as throwing error
            //cleanup
        }
        /// <summary>
        /// not needed? because it's not a logic.. it's an actual exception?
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_when_schema_definition_is_null_exist_throws_nullreferenceexception()
        {
            //arrange
            var cat = new DataSetSchema() { Title = "schemaname", Category = new Category() { Title = "categoryname" } };
            _repositoryDataSetSchema.Add(cat);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            var result = service.Get("categoryname", "schemaname", "columnname");
            //assert - no need to assert as throwing error
            //cleanup
        }
        /// <summary>
        /// not needed? because it's not a logic.. it's an actual exception?
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_when_schema_definition_columns_is_null_exist_throws_argumentnullexception()
        {
            //arrange
            var cat = new DataSetSchema() { Title = "schemaname", Definition = new DataSetSchemaDefinition(),Category = new Category() { Title = "categoryname" } };
            _repositoryDataSetSchema.Add(cat);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            var result = service.Get("categoryname", "schemaname", "columnname");
            //assert - no need to assert as throwing error
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "The column columnname does not exist")]
        public void Get_when_column_does_not_exist_throws_exception_column_does_not_exist()
        {
            //arrange
            var cat = new DataSetSchema() { Title = "schemaname", Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>() }, Category = new Category() { Title = "categoryname" } };
            _repositoryDataSetSchema.Add(cat);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            var result = service.Get("categoryname", "schemaname", "columnname");
            //assert - no need to assert as throwing error
            //cleanup
        }

        [TestMethod]
        public void Get_when_schema_isstandardisedschemaurl_is_null_returns_false()
        {
            //arrange
            var cat = new DataSetSchema() { Title = "schemaname", IsStandardisedSchemaUrl = null,Definition = new DataSetSchemaDefinition() { Columns = new List<DataSetSchemaColumn>(){new DataSetSchemaColumn(){ColumnName = "columnname"}} }, Category = new Category() { Title = "categoryname" } };
            _repositoryDataSetSchema.Add(cat);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            var result = service.Get("categoryname", "schemaname", "columnname");
            //assert
            Assert.AreEqual(false, result.IsStandardisedSchemaUrl);
            //cleanup
        }



        [TestMethod]
        public void Create_when_schema_column_type_is_long_lat_repository_inserts_2_columns_item()
        {
            //arrange
            var definition = new DataSetSchemaDefinition(){TableName = "tablename", Columns = new List<DataSetSchemaColumn>()};
            var col = new DataSetSchemaColumn(){Type = "lat/lng", SchemaDefinition = definition};
            var mock = new Mock<IDataSetSchemaColumnSqlRepo>();
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "Latitude")).Returns(false);
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "Longitude")).Returns(false);
            _sqlrepo = mock.Object;
                var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Create(col);
            var result = _repositoryDataSetSchemaColumn.GetAll().ToList();
            //assert
            Assert.AreEqual(2, result.Count);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(col);
        }
        [TestMethod]
        public void Create_when_schema_column_type_not_latlng_repository_inserts_1_column_item()
        {
            //arrange
            var definition = new DataSetSchemaDefinition() { TableName = "tablename", Columns = new List<DataSetSchemaColumn>() };
            var col = new DataSetSchemaColumn() { Type = "", SchemaDefinition = definition };
            var mock = new Mock<IDataSetSchemaColumnSqlRepo>();
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "")).Returns(false);
            _sqlrepo = mock.Object;
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Create(col);
            var result = _repositoryDataSetSchemaColumn.GetAll().ToList();
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(col);
        }
    

        [TestMethod]
        [ExpectedException(typeof(Exception), "A column named Latitude already exists in tablename.")]
        public void Create_when_schema_column_name_exist_throws_exception_a_column_columnname_already_exists_in_tablename()
        {
            //arrange
            var definition = new DataSetSchemaDefinition() { TableName = "tablename", Columns = new List<DataSetSchemaColumn>() };
            var col = new DataSetSchemaColumn() { Type = "lat/lng", SchemaDefinition = definition };
            var mock = new Mock<IDataSetSchemaColumnSqlRepo>();
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "Latitude")).Returns(true);
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "Longitude")).Returns(false);
            _sqlrepo = mock.Object;
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Create(col);
            //assert -- throwing exception
            
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "A column named Latitude already exists in tablename.")]
        public void Create_when_schema_column_name_exists_in_exiting_schemadefinition_columns_throws_exception_a_column_columnname_already_exists_in_tablename()
        {
            //arrange
            var definition = new DataSetSchemaDefinition() { TableName = "tablename", Columns = new List<DataSetSchemaColumn>() {new DataSetSchemaColumn(){ColumnName = "Latitude"}} };
            var col = new DataSetSchemaColumn() { Type = "lat/lng", SchemaDefinition = definition };
            var mock = new Mock<IDataSetSchemaColumnSqlRepo>();
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "Latitude")).Returns(false);
            mock.Setup(x => x.CheckSqlColumnExists("tablename", "Longitude")).Returns(false);
            _sqlrepo = mock.Object;
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Create(col);
            //assert -- throwing exception

            //cleanup
        }

        [TestMethod]
        public void Save_when_column_type_text_and_maxsize_is_larger_than_current_max_size_updates()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() {Id = 1, MaxSize = 100};
            var colToChange = new DataSetSchemaColumn() { Id=1, MaxSize  = 300};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(300, originalCol.MaxSize);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_column_helptext_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1};
            var colToChange = new DataSetSchemaColumn() { Id = 1, HelpText = "changed-help-text"};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("changed-help-text", originalCol.HelpText);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_helptext_is_null_column_helptext_is_emptystring()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1};
            var colToChange = new DataSetSchemaColumn() { Id = 1, HelpText = null };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("", originalCol.HelpText);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_column_isdefaultsort_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsDefaultSort = true};
            var colToChange = new DataSetSchemaColumn() { Id = 1, IsDefaultSort = false};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(false, originalCol.IsDefaultSort);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_column_isfilterable_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsFilterable = true };
            var colToChange = new DataSetSchemaColumn() { Id = 1, IsFilterable = false };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(false, originalCol.IsFilterable);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }


        [TestMethod]
        public void Save_isshowninitially_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsShownInitially = true };
            var colToChange = new DataSetSchemaColumn() { Id = 1, IsShownInitially = false };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(false, originalCol.IsShownInitially);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_column_istotalisable_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsTotalisable = true };
            var colToChange = new DataSetSchemaColumn() { Id = 1, IsTotalisable = false };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(false, originalCol.IsTotalisable);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_title_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, Title = "unchanged-title"};
            var colToChange = new DataSetSchemaColumn() { Id = 1, Title = "changed-title"};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("unchanged-title", originalCol.Title);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_title_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, Title = "unchanged-title" };
            var colToChange = new DataSetSchemaColumn() { Id = 1, Title = "changed-title" };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("changed-title", originalCol.Title);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_isRequired_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, IsRequired = true};
            var colToChange = new DataSetSchemaColumn() { Id = 1, IsRequired = false};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(false, originalCol.IsRequired);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }


        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_isRequired_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, IsRequired = true };
            var colToChange = new DataSetSchemaColumn() { Id = 1, IsRequired = false };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(true, originalCol.IsRequired);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_maxsize_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MaxSize = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxSize = 300};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(200, originalCol.MaxSize);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_maxsize_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MaxSize = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxSize = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(300, originalCol.MaxSize);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_minnumber_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MinNumber = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MinNumber = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(300, originalCol.MinNumber);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_minnumber_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MinNumber = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MinNumber = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(200, originalCol.MinNumber);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_maxnumber_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MaxNumber = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxNumber = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(300, originalCol.MaxNumber);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_maxnumber_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MaxNumber = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxNumber = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(200, originalCol.MaxNumber);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_mincurrency_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MinCurrency = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MinCurrency = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(300, originalCol.MinCurrency);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_mincurrency_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MinCurrency = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MinCurrency = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(200, originalCol.MinCurrency);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_maxcurrency_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MaxCurrency = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxCurrency = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(300, originalCol.MaxCurrency);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_maxcurrency_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MaxCurrency = 200 };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxCurrency = 300 };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(200, originalCol.MaxCurrency);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_mindate_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MinDate = new DateTime(1900,1,1) };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MinDate = new DateTime(2000, 1, 1) };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(new DateTime(2000, 1, 1), originalCol.MinDate);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_mindate_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MinDate = new DateTime(1900, 1, 1) };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MinDate = new DateTime(2000, 1, 1) };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(new DateTime(1900, 1, 1), originalCol.MinDate);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_column_maxdate_is_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, MaxDate = new DateTime(1900, 1, 1) };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxDate = new DateTime(2000, 1, 1) };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(new DateTime(2000, 1, 1), originalCol.MaxDate);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_maxdate_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, MaxDate = new DateTime(1900, 1, 1) };
            var colToChange = new DataSetSchemaColumn() { Id = 1, MaxDate = new DateTime(2000, 1, 1) };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual(new DateTime(1900, 1, 1), originalCol.MaxDate);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_true_column_linkeddatauri_is_not_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = true, LinkedDataUri = "unchanged-linkeduri" };
            var colToChange = new DataSetSchemaColumn() { Id = 1, LinkedDataUri = "changed-linkeduri" };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("unchanged-linkeduri", originalCol.LinkedDataUri);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_and_linkeddatauri_is_null_column_linkeddatauri_is_empty_string()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, LinkedDataUri = "unchanged-linkeduri" };
            var colToChange = new DataSetSchemaColumn() { Id = 1, LinkedDataUri = null};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("", originalCol.LinkedDataUri);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_and_linkeddataurivalue_contains_http_column_linkeddatauri_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, LinkedDataUri = "unchanged-linkeduri" };
            var colToChange = new DataSetSchemaColumn() { Id = 1, LinkedDataUri = "http://test" };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("http://test", originalCol.LinkedDataUri);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_and_linkeddataurivalue_contains_https_column_linkeddatauri_updated()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, LinkedDataUri = "unchanged-linkeduri" };
            var colToChange = new DataSetSchemaColumn() { Id = 1, LinkedDataUri = "https://test" };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("https://test", originalCol.LinkedDataUri);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }
        [TestMethod]
        public void Save_when_column_isstandardisedurl_is_false_and_linkeddataurivalue_does_not_contain_http_or_https_column_linkeddatauri_is_prefixed_with_http()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, LinkedDataUri = "unchanged-linkeduri" };
            var colToChange = new DataSetSchemaColumn() { Id = 1, LinkedDataUri = "test" };
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Save(colToChange);
            //assert 
            Assert.AreEqual("http://test", originalCol.LinkedDataUri);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
        }

        [TestMethod]
        public void Delete_will_remove_column_from_repository()
        {
            //arrange
            var originalCol = new DataSetSchemaColumn() { Id = 1, IsStandardisedSchemaUrl = false, ColumnName = "columnname", SchemaDefinition = new DataSetSchemaDefinition(){TableName = "tablename"}};
            _repositoryDataSetSchemaColumn.Add(originalCol);
            var mock = new Mock<IDataSetSchemaColumnSqlRepo>();
            mock.Setup(x => x.CheckSqlColumnExists(originalCol.SchemaDefinition.TableName, originalCol.ColumnName))
                .Returns(true);
            _sqlrepo = mock.Object;
            var service = new DataSetSchemaColumnService(_repositoryDataSetSchemaColumn, _repositoryDataSetSchema, _sqlrepo);
            //act
            service.Delete(originalCol);
            var result = _repositoryDataSetSchemaColumn.GetQuery().FirstOrDefault(x => x.Id == originalCol.Id);
            //assert 
            Assert.AreEqual(null, result);
            //cleanup
            _repositoryDataSetSchemaColumn.Delete(originalCol);
            _sqlrepo = null;
        }
    }
}
