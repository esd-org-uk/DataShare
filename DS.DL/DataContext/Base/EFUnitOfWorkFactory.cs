using System;
using System.Data.Entity;
using DS.DL.DataContext.Base.Interfaces;

namespace DS.DL.DataContext.Base
{
    public class EFUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private static Func<DbContext> _objectContextDelegate;
        private static readonly Object _lockObject = new object();

        public static void SetObjectContext(Func<DbContext> objectContextDelegate)
        {
            _objectContextDelegate = objectContextDelegate;
        }

        public IUnitOfWork Create()
        {
            DbContext context;
            lock (_lockObject)
            {
                 context = _objectContextDelegate();
            }
            return new EFUnitOfWork(context);
        }
    }
}
