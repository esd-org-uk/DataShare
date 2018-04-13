using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Extensions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using StructureMap;

namespace DS.Service
{
    public class DataSetSchemaService : IDataSetSchemaService
    {
        private IDataSetSchemaDefinitionService _dataSetSchemaDefinitionService;
        private IRepository<DataSetSchema> _repository;
        
        private IRepository<DataSetDetail> _repositoryDataSetDetail;
        private ISqlTableUtility _sqlTableUtility;

        public DataSetSchemaService(IRepository<DataSetSchema> repository, 
            IDataSetSchemaDefinitionService dataSetSchemaDefinitionService
            ,IRepository<DataSetDetail>  repositoryDataSetDetail
            ,ISqlTableUtility sqlTableUtility
            )
        {
            _dataSetSchemaDefinitionService = dataSetSchemaDefinitionService;
            _repository = repository;
            _repositoryDataSetDetail = repositoryDataSetDetail;
            _sqlTableUtility = sqlTableUtility;
        }
        public IRepository<DataSetSchema> Repository
        {
            get
            {
                return _repository;
            }
        }

        public  IList<DataSetDetail> GetByFriendlyUrl(string categoryUrl, string schemaUrl, bool showAll = false)
        {
            var schema = showAll ? Repository.GetQuery()
                                   .Include(s => s.Category)
                                   .Include(s => s.DataSets)
                                   .ToList()
                                   .FirstOrDefault(s => s.Category.FriendlyUrl == categoryUrl && s.FriendlyUrl == schemaUrl)
                                : Repository.GetQuery()
                                   .Include(s => s.Category)
                                   .Include(s => s.DataSets)
                                   .ToList()
                                   .FirstOrDefault(s => s.Category.FriendlyUrl == categoryUrl && s.FriendlyUrl == schemaUrl && s.IsOnline);

            if (schema != null && schema.DataSets != null)
                return schema.DataSets.OrderByDescending(d => d.DateUpdated).ToList();

            return new List<DataSetDetail>();
        }

        /// <summary>
        /// used for DsService
        /// will return ALL datasetSchemas
        /// </summary>
        /// <returns></returns>
        public  IList<DataSetSchema> GetFullList()
        {
            return Repository.GetAll().ToList();
        }

        public  IList<DataSetSchema> GetAll()
        {
            return Repository.GetQuery()
                            .Include(s => s.Category)
                            .Include(s => s.DataSets).ToList()
                            .Where(s => s.IsOnline && (s.DataSets !=null && s.DataSets.Count > 0))
                            .OrderBy(s => s.Title).ToList();
        }

        public  IList<DataSetSchema> GetFeatured()
        {
            return Repository.GetQuery()
                             .Include(s => s.Category)
                             .Include(s => s.DataSets)
                             .Where(s => s.IsFeatured).ToList()
                             .Where(s => s.IsOnline && (s.DataSets != null && s.DataSets.Count > 0))
                             .OrderBy(s => s.DateUpdated).Take(3).ToList();
        }

        
        /// <summary>
        /// used in DSService - for email reminders
        /// </summary>
        /// <returns></returns>
        public  IList<DataSetSchema> GetOverDue()
        {
            var overDueDate = DateTime.Today.AddDays(-3);
            var regularUploads = Repository.GetQuery()
                             .Include(s => s.Category)
                             .Include(s => s.DataSets)
                             .Where(s => s.UploadFrequency > 0)
                             .ToList();
            var results = new List<DataSetSchema>();
            foreach (var schema in regularUploads)
            {
                var lastUpload = schema.DateLastUploadedTo.GetValueOrDefault(DateTime.Today);
                var nextReminderDate = lastUpload.AddDays(schema.UploadFrequency);
                if (nextReminderDate <= overDueDate && schema.DateLastReminded.GetValueOrDefault(DateTime.MinValue) < overDueDate && schema.IsOnline)
                {
                    results.Add(schema);
                }
            }
            return results;
        }

        public  DataSetSchema Get(int id)
        {
            var sch = Repository.GetQuery()
                .Include(s => s.Category)
                .Include(s => s.Definition)
                .Include(s => s.Definition.Columns).FirstOrDefault(c => c.Id == id);
            if (sch!= null && sch.Definition != null && sch.Definition.Columns != null)
                sch.Definition.Columns = sch.Definition.Columns.OrderBy(c => c.DefaultDisplayWeight ?? 99).ToList();

            return sch;
        }

        public  DataSetSchema Get(string categoryUrl, string friendlyUrl)
        {
            var sch = Repository.GetQuery()
                                .Include(s => s.Category)
                                .Include(s => s.Definition.Columns).ToList()
                                .FirstOrDefault(s=>s.Category.FriendlyUrl==categoryUrl && s.FriendlyUrl==friendlyUrl);
            if (sch != null && sch.Definition != null && sch.Definition.Columns != null)
                sch.Definition.Columns = sch.Definition.Columns.OrderBy(c => c.DefaultDisplayWeight ?? 99).ToList();
            return sch;
        }

        public  DataSetSchema Get(string friendlyUrl)
        {
            var result =  Repository.GetQuery()
                .Include(s => s.Category)
                .Include(s => s.Definition.Columns).ToList().FirstOrDefault(c => c.FriendlyUrl == friendlyUrl);
            if (result == null) return null;
            if (result.Definition != null && result.Definition.Columns != null)
                result.Definition.Columns = result.Definition.Columns.OrderBy(c => c.DefaultDisplayWeight ?? 99).ToList();
                
            return result;
        }

        public  IList<DataSetSchema> Search(string searchTerm)
        {
            var searchTerms = searchTerm.Split(' ');

            var schemas = Repository.GetQuery()
                            .Include(s => s.Category)
                            .Include(s => s.DataSets).ToList()
                            .Where(s => s.IsOnline && s.DataSets.Count > 0)
                            .OrderBy(s => s.Title).ToList();
            var results = new List<DataSetSchema>();
            foreach (var schema in schemas)
            {
                var added = false;
                for (var t = 0; t < searchTerms.Length && !added; t++)
                {
                    if (schema.Title.ToLower().Contains(searchTerms[t].ToLower()) 
                        || schema.Description.ToLower().Contains(searchTerms[t].ToLower()) 
                        || schema.ShortDescription.ToLower().Contains(searchTerms[t].ToLower()))
                    {
                        results.Add(schema);
                        added = true;
                    }
                }
            }
            return results;
        }

        public void Create(DataSetSchema schema)
        {
            //check schema exists
            if (_repository.GetQuery().Any(c => c.Title.ToLower() == schema.Title.ToLower()))
                throw new Exception(string.Format("A schema named {0} already exists", schema.Title));


            //only create schema def if files are uploaded not stored externally
            if (schema.IsExternalData)
            {
                _repository.Add(schema);
                _repository.SaveChanges();
                return;
            }
            var tableName = String.Format("DS_{0}", schema.Title.RemovePunctuationAndSpacing(false));
            if (_dataSetSchemaDefinitionService.SqlTableExists(tableName))
                throw new Exception(string.Format("A table named {0} already exists in the database", tableName));


            schema.Definition = new DataSetSchemaDefinition {TableName = tableName};
            _repository.Add(schema);
            _repository.SaveChanges();
            _dataSetSchemaDefinitionService.CreateSqlTable(schema.Definition);
        }

        public  void Save(DataSetSchema schemaData)
        {
            var schema = _repository.GetQuery().FirstOrDefault(s => s.Id == schemaData.Id);
            schema.UploadFrequency = schemaData.UploadFrequency;
            schema.OwnerEmail = schemaData.OwnerEmail;
            schema.Description = schemaData.Description;
            schema.IsExternalData = schemaData.IsExternalData;
            schema.IsFeatured = schemaData.IsFeatured;
            schema.IsAllDataOverwittenOnUpload = schemaData.IsAllDataOverwittenOnUpload;
            schema.ShortDescription = schemaData.ShortDescription;
            
            if(String.IsNullOrEmpty(schema.SchemaDefinitionFromUrl))schema.Title = schemaData.Title;

            _repository.SaveChanges();
        }

        public  void Delete(string categoryUrl, string schemaUrl)
        {
            var schema = _repository.GetQuery().Include(s => s.Category)
                .ToList().FirstOrDefault(c => c.Category.FriendlyUrl == categoryUrl && c.FriendlyUrl == schemaUrl);

            schema.IsDisabled = true;
            _repository.SaveChanges();
        }

        public  void DeleteAll(string categoryUrl, string schemaUrl)
        {
            var schema = _repository.GetQuery()
                                    .Include(s => s.Category)
                                    .Include(s => s.Definition)
                                    .ToList()
                                    .FirstOrDefault(c => c.Category.FriendlyUrl == categoryUrl && c.FriendlyUrl == schemaUrl);

            if (schema == null)
                return;


            //delete columns
            var tableName = "";
            if (schema.Definition != null)
            {
                tableName = schema.Definition.TableName;
                _repository.ExecuteRawSql(String.Format("DELETE FROM DS_DataSetSchemaColumns WHERE SchemaDefinition_Id = {0}", schema.Definition.Id));
                //delete previous dataset details
                _repositoryDataSetDetail.ExecuteRawSql(String.Format("DELETE FROM DS_DataSetDetails WHERE Schema_Id = {0} and Id != {1}", schema.Id, schema.Definition.Id));
                _repositoryDataSetDetail.ExecuteRawSql(String.Format("IF OBJECT_ID('dbo.{0}', 'U') IS NOT NULL  DELETE FROM {0} WHERE DataSetDetailId != {1}", schema.Definition.TableName, schema.Definition.Id));

                //delete definition
                _dataSetSchemaDefinitionService.Delete(schema.Definition.Id);


            }

            //Delete the schema sql table
            if(!String.IsNullOrEmpty(tableName) )_sqlTableUtility.DropTable(tableName);
 
            //delete schema
            _repository.Delete(schema);
            _repository.SaveChanges();
        }

        public  void Approve(string categoryUrl, string schemaUrl)
        {
            var schema = _repository.GetQuery().Include(s => s.Category)
                .ToList().FirstOrDefault(c => c.Category.FriendlyUrl == categoryUrl && c.FriendlyUrl == schemaUrl);
            
            if (schema == null) return;
            
            schema.IsApproved = true;
            schema.IsDisabled = false;
            _repository.SaveChanges();
        }

        public  void Enable(string categoryUrl, string schemaUrl)
        {
            var schema = _repository.GetQuery().Include(s => s.Category)
                .ToList().FirstOrDefault(c => c.Category.FriendlyUrl == categoryUrl && c.FriendlyUrl == schemaUrl);
            if (schema == null) return;
            schema.IsDisabled = false;
            _repository.SaveChanges();
        }
    }
}
