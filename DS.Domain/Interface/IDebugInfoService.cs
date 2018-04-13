using System.Collections.Generic;

namespace DS.Domain.Interface
{
    public interface IDebugInfoService
    {
        IEnumerable<DebugInfo> GetAll();
        IEnumerable<DebugInfo> Get(DebugInfoTypeEnum type);
        void Add(DebugInfo info);
    }
}