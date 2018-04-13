using System;
using System.Collections.Generic;
using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using StructureMap;

namespace DS.Service
{
    public  class DebugInfoService : IDebugInfoService
    {
        private IRepository<DebugInfo> _repository;
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
            if (typeToCheck == "All")
                return GetAll();
            return _repository.GetQuery().Where(d => d.Type == typeToCheck).OrderByDescending(d => d.DateCreated);
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
