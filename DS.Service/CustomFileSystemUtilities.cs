using System;
using DS.Domain.Interface;

namespace DS.Service
{
    public class CustomFileSystemUtilities : IFileSystem
    {

        public string ReadAllText(string filepath)
        {
            if(System.IO.File.Exists(filepath))
                return System.IO.File.ReadAllText(filepath);
            return "";
        }
    }
}