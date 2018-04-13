using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using DS.Domain.Interface;

namespace DS.Service
{
    public class XmlToObjectService : IXmlToObjectService
    {
        public T ConvertXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(new StringReader(xml));
        }

        public string GetXmlFromUrl(string url)
        {
            if(string.IsNullOrEmpty(url)) throw new Exception("Unable to download xml from " + url);
            
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) throw new Exception("Invalid url at " + url);


            var client = new System.Net.WebClient();
            //client.Proxy = new WebProxy(("10.4.246.76"), 3128);
            var result =  Encoding.UTF8.GetString(client.DownloadData(url));
            
            client.Dispose();
            return result;

        }
    }
}
