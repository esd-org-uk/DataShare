using System;

namespace DS.DL.DataContext.Base.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}
