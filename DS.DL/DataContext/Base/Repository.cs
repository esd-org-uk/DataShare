using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain.Interface;

namespace DS.DL.DataContext.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbContext _dataContext;
        private IDbSet<T> _objectSet;

        private DbContext Context
        {
            get { return _dataContext ?? (_dataContext = GetCurrentUnitOfWork<EFUnitOfWork>().Context); }
        }

        private IDbSet<T> ObjectSet
        {
            get { return _objectSet ?? (_objectSet = Context.Set<T>()); }
        }

        public DbConnection Connection
        {
            get { return Context.Database.Connection; }
        }

        public TUnitOfWork GetCurrentUnitOfWork<TUnitOfWork>() where TUnitOfWork : IUnitOfWork
        {
            return (TUnitOfWork)UnitOfWork.Current;
        }

        public string DbConnectionString { get { return Context.Database.Connection.ConnectionString; } }

        public IQueryable<T> GetQuery()
        {
            return ObjectSet;
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> filter)
        {
            return ObjectSet.Where(filter);
        }

        // Query Expression with an Expression to Include() one child collection in the select
        public IEnumerable<T> Query<TFectchedCollection>(Expression<Func<T, bool>> filter, Expression<Func<T, IEnumerable<TFectchedCollection>>> fetchCollection)
        {
            return ObjectSet.Include(fetchCollection).Where(filter);
        }

        // Query Expression with an Expression to Include() one child object in the select
        public IEnumerable<T> Query<TFectchedChild>(Expression<Func<T, bool>> filter, Expression<Func<T, TFectchedChild>> fetchChild)
        {
            return ObjectSet.Include(fetchChild).Where(filter);
        }

        // Query Expression with an Expression to Include() one child object in the select
        public IEnumerable<T> Query<TFectchedChild>(Expression<Func<T, TFectchedChild>> fetchChild)
        {
            return ObjectSet.Include(fetchChild);
        }

        public void ExecuteRawSql(string sql)
        {
           Context.Database.ExecuteSqlCommand(sql);
        }

        public TReturn ExecuteRawSql<TReturn>(string sql)
        {
            var sqlQuery = Context.Database.SqlQuery<TReturn>(sql).FirstOrDefault();

            return sqlQuery;
        }

        public TReturn ExecuteRawSql<TReturn>(string sql, object parameters)
        {
            var sqlQuery = Context.Database.SqlQuery<TReturn>(sql, parameters).FirstOrDefault();

            return sqlQuery;
        }

        public IEnumerable<T> GetAll()
        {
            return GetQuery().ToList();
        }

        public IEnumerable<T> Find(Func<T, bool> where)
        {
            return ObjectSet.Where(where);
        }

        public T Single(Func<T, bool> where)
        {
            return ObjectSet.Single(where);
        }

        public T First(Func<T, bool> where)
        {
            return ObjectSet.First(where);
        }

        public void Delete(T entity)
        {
            ObjectSet.Remove(entity);
        }

        public void Add(T entity)
        {
            ObjectSet.Add(entity);
        }

        public void Attach(T entity)
        {
            ObjectSet.Attach(entity);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
