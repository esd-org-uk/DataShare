using System.Collections.Generic;
using DS.Domain;
using DS.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class SqlOrderByColumnAndDirectionFormatterTests
    {
        [TestMethod]
        public void CheckOrderByDirection_when_order_direction_is_NOT_null_and_NOT_empty_returns_orderDirection()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            //action
            var orderdirection = "notEmpty";
            var schema = new DataSetSchema();
            var result = formatter.CheckOrderByDirection(schema, orderdirection);
            //assert
            Assert.AreEqual("notEmpty", result);


        }

        [TestMethod]
        public void CheckOrderByDirection_when_schema_is_not_null_and_definition_is_not_null_and_schema_definition_columns_is_not_null_returns_ASC()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            //action
            var orderdirection = "";
            var schema = new DataSetSchema(){Definition = new DataSetSchemaDefinition()
                {
                    Columns = new List<DataSetSchemaColumn>()
                }};
            var result = formatter.CheckOrderByDirection(schema, orderdirection);
            //assert
            Assert.AreEqual("ASC", result);


        }

        [TestMethod]
        public void CheckOrderByDirection_when_schema_is_not_null_and_definition_is_not_null_and_schema_definition_columns_is_null_returns_ASC()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            //action
            var orderdirection = "";
            var schema = new DataSetSchema()
            {
                Definition = new DataSetSchemaDefinition(){}
            };
            var result = formatter.CheckOrderByDirection(schema, orderdirection);
            //assert
            Assert.AreEqual("ASC", result);


        }
        [TestMethod]
        public void CheckOrderByDirection_when_schema_is_null_and_orderdirection_is_empty_returns_ASC()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            //action
            var orderdirection = "";
            DataSetSchema schema = null;
            
            var result = formatter.CheckOrderByDirection(schema, orderdirection);
            //assert
            Assert.AreEqual("ASC", result);


        }


        [TestMethod]
        public void
            CheckOrderByColumn_when_orderBycolumn_is_null_and_columns_is_not_null_returns_default_sort_column_enclosed_by_brackets
            ()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            string orderbyColumn = null;
             var schema = new DataSetSchema(){Definition = new DataSetSchemaDefinition()
                 {
                     Columns = new List<DataSetSchemaColumn>(){new DataSetSchemaColumn(){ColumnName = "col1"}}
                 }};
            //act
            var result = formatter.CheckOrderByColumn(schema, orderbyColumn);
            //assert
            Assert.AreEqual("[col1]", result);
            //clean up

        }

        [TestMethod]
        public void
            CheckOrderByColumn_when_orderBycolumn_is_empty_and_columns_is_not_null_returns_default_sort_column_enclosed_by_brackets
            ()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            string orderbyColumn = "";
            var schema = new DataSetSchema()
            {
                Definition = new DataSetSchemaDefinition()
                {
                    Columns = new List<DataSetSchemaColumn>() { new DataSetSchemaColumn() { ColumnName = "col1" } }
                }
            };
            //act
            var result = formatter.CheckOrderByColumn(schema, orderbyColumn);
            //assert
            Assert.AreEqual("[col1]", result);
            //clean up

        }


        [TestMethod]
        public void
            CheckOrderByColumn_when_orderBycolumn_is_NOTnullOrEmpty_and_orderByColumn_does_not_startWith_bracket_returns_orderBy_enclosed_by_brackets
            ()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            string orderbyColumn = "col1";
            var schema = new DataSetSchema();
            //act
            var result = formatter.CheckOrderByColumn(schema, orderbyColumn);
            //assert
            Assert.AreEqual("[col1]", result);
            //clean up

        }


        [TestMethod]
        public void
            CheckOrderByColumn_when_orderBycolumn_is_NOT_nullOrEmpty_and_orderByColumn_starts_with_and_datasetcolumns_are_empty_returns_orderBycolumn
            ()
        {
            //arrange
            var formatter = new SqlOrderByColumnAndDirectionFormatter();
            string orderbyColumn = "[col1_v2]";
            var schema = new DataSetSchema();
            //act
            var result = formatter.CheckOrderByColumn(schema, orderbyColumn);
            //assert
            Assert.AreEqual("[col1_v2]", result);
            //clean up

        }
    }
}
