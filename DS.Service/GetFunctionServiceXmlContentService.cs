using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS.Domain.Interface;

namespace DS.Service
{
    public class GetFunctionServiceXmlContentService :IGetFunctionServiceXmlContent
    {
        private IFileSystem _fileSystem;
        private string _filepath;

        public GetFunctionServiceXmlContentService(IFileSystem fileSystem, string filepath)
        {
            _fileSystem = fileSystem;
            _filepath = filepath;
        }

        public string GetFunctionServiceXmlContent()
        {
            return _fileSystem.ReadAllText(_filepath);
        }
    }
}
