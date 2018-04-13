using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.Domain.Interface
{
    public interface IFileSystem
    {

        string ReadAllText(string filepath);
    }
}
