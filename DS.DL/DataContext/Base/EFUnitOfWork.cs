using System;
using System.Data.Entity;
using DS.DL.DataContext.Base.Interfaces;

namespace DS.DL.DataContext.Base
{
    public class EFUnitOfWork : IUnitOfWork
    {
        public DbContext Context { get; private set; }

        public EFUnitOfWork(DbContext context)
        {
            Context = context;
            context.Configuration.LazyLoadingEnabled = true;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
