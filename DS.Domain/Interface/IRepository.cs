using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace DS.Domain.Interface
{
    public interface IRepository<T> where T: class
    {
        //DbConnection Connection { get;}
        String DbConnectionString { get; }
        IQueryable<T> GetQuery();
        IEnumerable<T> Query(Expression<Func<T, bool>> filter);
        IEnumerable<T> Query<TFectchedCollection>(Expression<Func<T, bool>> filter, Expression<Func<T, IEnumerable<TFectchedCollection>>> fetchCollection);
        IEnumerable<T> Query<TFectchedChild>(Expression<Func<T, bool>> filter, Expression<Func<T, TFectchedChild>> fetchChild);
        IEnumerable<T> Query<TFectchedChild>(Expression<Func<T, TFectchedChild>> fetchChild);

        void ExecuteRawSql(string sql);
        TReturn ExecuteRawSql<TReturn>(string sql);
        TReturn ExecuteRawSql<TReturn>(string sql, object parameters);

        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> where);
        T Single(Func<T, bool> where);
        T First(Func<T, bool> where);

        void Delete(T entity);
        void Add(T entity);
        void Attach(T entity);
        void SaveChanges();
    }
}
