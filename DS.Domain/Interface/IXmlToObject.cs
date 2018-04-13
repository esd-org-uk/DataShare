using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.Domain.Interface
{
    public interface IXmlToObjectService
    {
        T ConvertXml<T>(string xml);

        string GetXmlFromUrl(string url);
    }
}
