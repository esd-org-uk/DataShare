using System;
using System.Collections;
using System.Threading;
using System.Web;
using DS.DL.DataContext.Base.Interfaces;
using StructureMap;

namespace DS.DL.DataContext.Base
{
    public static class UnitOfWork
    {
        private const string HTTPCONTEXTKEY = "DS.Repository.HttpContext.Key";
        
        private static IUnitOfWorkFactory _unitOfWorkFactory;
        private static readonly Hashtable _threads = new Hashtable();

        public static void Commit()
        {
            var unitOfWork = GetUnitOfWork();
            if (unitOfWork != null)
            {
                unitOfWork.Commit();
            }
        }

        public static IUnitOfWork Current 
        {
            get
            {
                var unitOfWork = GetUnitOfWork();
                if (unitOfWork == null)
                {
                    _unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
                    unitOfWork = _unitOfWorkFactory.Create();
                    SaveUnitOfWork(unitOfWork);
                }
                return unitOfWork;
            }
        }

        private static IUnitOfWork GetUnitOfWork()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(HTTPCONTEXTKEY))
                {
                    return (IUnitOfWork)HttpContext.Current.Items[HTTPCONTEXTKEY];
                }
                return null;
            }
            var thread = Thread.CurrentThread;
            if (string.IsNullOrEmpty(thread.Name))
            {
                thread.Name = Guid.NewGuid().ToString();
                return null;
            }
            lock (_threads.SyncRoot)
            {
                return (IUnitOfWork)_threads[Thread.CurrentThread.Name];
            }
        }

        private static void SaveUnitOfWork(IUnitOfWork unitOfWork)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[HTTPCONTEXTKEY] = unitOfWork;
            }
            else
            {
                lock(_threads.SyncRoot)
                {
                    _threads[Thread.CurrentThread.Name] = unitOfWork;
                }
            }
        }
    }
}
