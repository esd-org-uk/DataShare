using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DS.Domain.Interface;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using StructureMap;

namespace DS.Service
{
    public class DataSetSchemaDefinitionService : IDataSetSchemaDefinitionService
    {
        private readonly IRepository<DataSetSchemaDefinition> _repository;
      
        public DataSetSchemaDefinitionService(IRepository<DataSetSchemaDefinition> repository)
        {
            _repository = repository;
        }
      
        public bool SqlTableExists(string tableName)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                return db.Tables.Contains(tableName);
            }
        }

        public  void CreateSqlTable(DataSetSchemaDefinition schema)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataShareContext"].ConnectionString))
            {
                cn.Open();
                var server = new Server(new ServerConnection(cn));
                var db = server.Databases[cn.Database];
                var newTable = new Table(db,schema.TableName);
                
                //create Id column
                var idColumn = new Column(newTable, "Id")
                                   {
                                       DataType = DataType.Int,
                                       Nullable = false,
                                       Identity = true,
                                       IdentitySeed = 1,
                                       IdentityIncrement = 1
                                   };
                newTable.Columns.Add(idColumn);

                //create index
                var index = new Index(newTable, string.Format("PK_{0}", schema.TableName))
                                {IndexKeyType = IndexKeyType.DriPrimaryKey};
                index.IndexedColumns.Add(new IndexedColumn(index,"Id"));
                newTable.Indexes.Add(index);

                //create DataSetDetailId column
                newTable.Columns.Add(new Column(newTable, "DataSetDetailId")
                                              {
                                                  DataType = DataType.Int,
                                                  Nullable = false
                                              });

                //create sql table
                newTable.Create();

            }
        }

       
        public  void Delete(int definitionId)
        {
            var definition = _repository.GetQuery()
                                       .Include(s => s.Columns)
                                       .SingleOrDefault(d => d.Id == definitionId);
            if (definition == null) return;

            _repository.Delete(definition);
            _repository.SaveChanges();
        }
    }
}
