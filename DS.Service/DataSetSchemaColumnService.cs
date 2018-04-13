using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DS.Domain;
using DS.Domain.Interface;
using DS.Extensions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using StructureMap;

namespace DS.Service
{
    public class DataSetSchemaColumnService : IDataSetSchemaColumnService
    {
        private readonly IRepository<DataSetSchemaColumn> _repository;
        private IRepository<DataSetSchema> _repositoryDataSetSchema;
        private IDataSetSchemaColumnSqlRepo _dataSetSchemaColumnSqlRepo;
        

        public DataSetSchemaColumnService(IRepository<DataSetSchemaColumn> repository, IRepository<DataSetSchema> repositoryDataSetSchema, 
            IDataSetSchemaColumnSqlRepo dataSetSchemaColumnSqlRepo
        )
        {
            _repository = repository;
            _repositoryDataSetSchema = repositoryDataSetSchema;
            _dataSetSchemaColumnSqlRepo = dataSetSchemaColumnSqlRepo;
        }

        public  DataSetSchemaColumn Get(string categoryName, string schemaName, string columnName)
        {

            var schema = _repositoryDataSetSchema.GetQuery()
                              .Include(s => s.Category)
                              .Include(s => s.Definition.Columns).ToList().FirstOrDefault(s => s.Category.FriendlyUrl == categoryName && s.FriendlyUrl == schemaName);
            
            if(schema == null) throw new Exception("The schema " + schemaName + " does not exist");
            
            var column =  schema.Definition.Columns.FirstOrDefault(c => c.ColumnName == columnName);
            
            if (column == null) throw new Exception("The column " + columnName + " does not exist");

            column.IsStandardisedSchemaUrl = schema.IsStandardisedSchemaUrl ?? false;
            return column;
        }
        
        

        public  void Create(DataSetSchemaColumn schemaCol)
        {
            


            if (schemaCol.Type.ToLower() == "lat/lng")
            {
                var originalTitle = schemaCol.Title ?? "";
                var originalColumnName = schemaCol.ColumnName ?? "";
                
                if (originalColumnName.EndsWith("Latitude") || originalColumnName.EndsWith("Longitude"))
                {
                    //AddColumn(schemaCol, DataType.Float);
                    AddColumn(schemaCol, "Float");
                    return;
                }
                
                //create field for Latitude
                schemaCol.Title = originalTitle + " Latitude";
                schemaCol.ColumnName = originalColumnName + " Latitude";
                AddColumn(schemaCol, "Float");
                //AddColumn(schemaCol, DataType.Float);
                
                //create field for longitude
                schemaCol.Title = originalTitle + " Longitude";
                schemaCol.ColumnName = originalColumnName + " Longitude";
                AddColumn(schemaCol, "Float");
                //AddColumn(schemaCol, DataType.Float);    

            }
            else
            {
                //var sqlDataType = new DataType();
                
                switch (schemaCol.Type.ToLower())
                {
                    case "text":
                        //sqlDataType = DataType.NVarChar(schemaCol.MaxSize);
                        AddColumn(schemaCol, "NVarChar", schemaCol.MaxSize);
                        break;
                    case "number":
                        //sqlDataType = DataType.Float;
                        AddColumn(schemaCol, "Float");
                        break;
                    case "datetime":
                        //sqlDataType = DataType.DateTime;
                        AddColumn(schemaCol, "DateTime");
                        break;
                    case "currency":
                        //sqlDataType = DataType.Float;
                        AddColumn(schemaCol, "Float");
                        break;
                    case "image":
                        //sqlDataType = DataType.NVarChar(1024);
                        AddColumn(schemaCol, "NVarChar", 1024);
                        break;
                    case "url":
                        //sqlDataType = DataType.NVarChar(2048);
                        AddColumn(schemaCol, "NVarChar", 2048);
                        break;
                    default:
                        AddColumn(schemaCol, "");
                        break;

                }
                
            }
        }

        public  void Save(DataSetSchemaColumn columnData)
        {
            var column = _repository.GetQuery().Include(s => s.SchemaDefinition).FirstOrDefault(c => c.Id == columnData.Id);

            if (column.Type == "Text" && columnData.MaxSize > column.MaxSize)
            {
                //update the colsize in the database
                UpdateColumnMaxSize(column, columnData.MaxSize);
            }
            
            column.HelpText = columnData.HelpText ?? "";
            column.IsDefaultSort = columnData.IsDefaultSort;
            column.DefaultSortDirection = columnData.IsDefaultSort ? columnData.DefaultSortDirection : "";
            column.IsFilterable = columnData.IsFilterable;
            column.IsShownInitially = columnData.IsShownInitially;
            column.IsTotalisable = columnData.IsTotalisable;
            
            
            if (Convert.ToBoolean(column.IsStandardisedSchemaUrl)){
                _repository.SaveChanges();
                return;
            }

            column.Title = columnData.Title;
            column.IsRequired = columnData.IsRequired;
            column.MaxSize = columnData.MaxSize;
            column.MinNumber = columnData.MinNumber;
            column.MaxNumber = columnData.MaxNumber;
            column.MinCurrency = columnData.MinCurrency;
            column.MaxCurrency = columnData.MaxCurrency;
            column.MinDate = columnData.MinDate;
            column.MaxDate = columnData.MaxDate;
            column.LinkedDataUri = columnData.LinkedDataUri == null ? "" : (columnData.LinkedDataUri.Contains("http://") || columnData.LinkedDataUri.Contains("https://")) ? columnData.LinkedDataUri : String.Format("http://{0}", columnData.LinkedDataUri);
            _repository.SaveChanges();            
        }

        public  void Delete(DataSetSchemaColumn column)
        {
            //Delete column from sql table
            if (_dataSetSchemaColumnSqlRepo.CheckSqlColumnExists(column.SchemaDefinition.TableName, column.ColumnName)){
                _dataSetSchemaColumnSqlRepo.DeleteColumn(column);
            }

            //Delete column from schema def
            _repository.Delete(column);
            _repository.SaveChanges();
        }

        public  bool SqlColumnExists(string tableName, string columnName)
        {
            return _dataSetSchemaColumnSqlRepo.CheckSqlColumnExists(tableName, columnName);
        }

        public string SaveSorting(string[] columnsAndIndexes, string categoryName, string schemaName)
        {
            //var schema = _repositoryDataSetSchema.GetQuery()
            //      .Include(s => s.Category)
            //      .ToList().FirstOrDefault(s => s.Category.FriendlyUrl == categoryName && s.FriendlyUrl == schemaName);

            foreach (var a in columnsAndIndexes)
            {
                var colId = Convert.ToInt32(a.Split('=')[0]);
                var displayWeight = Convert.ToInt32(a.Split('=')[1]);
                var column = _repository.GetQuery().FirstOrDefault(x => x.Id == colId);
                if (column != null)
                {
                    
                    column.DefaultDisplayWeight = displayWeight;
                    if (Convert.ToInt32(column.MaxSize) == 0) column.MaxSize = 1;
                    
                    _repository.SaveChanges();

                }
            }
            
            //return "{ \"message\": \"Sorting saved!\"}";
            return "Sorting saved";
        }


        private  void AddColumn(DataSetSchemaColumn column, string sqlDataType, int maxLength = 0)
        {
            var columnName = string.IsNullOrEmpty(column.ColumnName) ? column.Title.RemovePunctuationAndSpacing(false) : column.ColumnName.RemovePunctuationAndSpacing(false);
            if (!SqlColumnExists(column.SchemaDefinition.TableName, columnName) 
                && !column.SchemaDefinition.Columns.Exists(c => c.ColumnName == columnName))
            {
                column.ColumnName = columnName;
              
                //add physical column to  table
                _dataSetSchemaColumnSqlRepo.AddColumn(column, sqlDataType, maxLength);
                
                //add entry to definition table
                _repository.Add(column);
                _repository.SaveChanges();
            }
            else throw new Exception(String.Format("A column named {0} already exists in {1}.", columnName, column.SchemaDefinition.TableName));
        }

        private  void UpdateColumnMaxSize(DataSetSchemaColumn column, int newSize)
        {
            if (!SqlColumnExists(column.SchemaDefinition.TableName, column.ColumnName)) return;

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                var dbTable = db.Tables[column.SchemaDefinition.TableName];
                dbTable.Columns[column.ColumnName].DataType = DataType.NVarChar(newSize);
                dbTable.Alter();
            }
        }
    }
}
