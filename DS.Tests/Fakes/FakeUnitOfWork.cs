using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain.Interface;

namespace DS.Tests.Fakes
{
    public class MemoryUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private MemoryUnitOfWork _unitOfWork = new MemoryUnitOfWork();

        public IUnitOfWork Begin()
        {
            return _unitOfWork;
        }

        public IUnitOfWork Create()
        {
            return _unitOfWork;
        }
    }

    public class MemoryUnitOfWork : IUnitOfWork
    {
        private Dictionary<Type, object> _containerByType = new Dictionary<Type, object>();


        public void Dispose()
        {
            _containerByType.Clear();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }

  
    public class MemoryRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private List<TEntity> _entities = new List<TEntity>();

        public IQueryable<TEntity> GetSatisfying(Expression<Func<TEntity, bool>> specification)
        {
            return _entities.Where(specification.Compile()).AsQueryable();
        }

        //public DbConnection Connection { get; private set; }
        public string DbConnectionString { get { return "this-is-a-fake-no-connectionstring-needed"; } }

        public IQueryable<TEntity> GetQuery()
        {
            return _entities.AsQueryable();
        }

        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            //return _entities.Where(filter);
            return _entities;
        }

        public IEnumerable<TEntity> Query<TFectchedCollection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, IEnumerable<TFectchedCollection>>> fetchCollection)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Query<TFectchedChild>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TFectchedChild>> fetchChild)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Query<TFectchedChild>(Expression<Func<TEntity, TFectchedChild>> fetchChild)
        {
            return _entities.AsQueryable();
        }

        public void ExecuteRawSql(string sql)
        {
            return;
        }

        public TReturn ExecuteRawSql<TReturn>(string sql)
        {
            throw new NotImplementedException();
        }

        public TReturn ExecuteRawSql<TReturn>(string sql, object parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _entities.AsQueryable();
        }

        public IEnumerable<TEntity> Find(Func<TEntity, bool> @where)
        {
            throw new NotImplementedException();
        }

        public TEntity Single(Func<TEntity, bool> @where)
        {
            throw new NotImplementedException();
        }

        public TEntity First(Func<TEntity, bool> @where)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void Attach(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            //throw new NotImplementedException();
        }
    }
}
