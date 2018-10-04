using DS.Domain;
using DS.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DS.Service
{
    public class DebugInfoService : IDebugInfoService
    {
        private readonly IRepository<DebugInfo> _repository;

        public DebugInfoService(IRepository<DebugInfo> repository)
        {
            _repository = repository;
        }

        public  IEnumerable<DebugInfo> GetAll()
        {
            var typeToCheck = Convert.ToString( DebugInfoTypeEnum.Developer);
            return _repository.GetQuery().Where(d => d.Type != typeToCheck).OrderByDescending(d => d.DateCreated).ToList();
        }

        public  IEnumerable<DebugInfo> Get(DebugInfoTypeEnum type)
        {
            var typeToCheck = Convert.ToString(type);
            return typeToCheck == "All" ? GetAll() : _repository.GetQuery().Where(d => d.Type == typeToCheck).OrderByDescending(d => d.DateCreated);
        }

        public  void Add(DebugInfo info)
        {
            info.CreatedBy = "";
            info.UpdatedBy = "";
            _repository.Add(info);
            _repository.SaveChanges();
        }
    }
}
