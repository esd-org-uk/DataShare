using System;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class XmlToObjectServiceTests
    {
        [TestInitialize]
        public void Testinit()
        {
            
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Unable to download xml from ")]
        public void GetXmlFromUrl_when_url_isnull_throw_exception_with_message_unable_to_download_from_url()
        {
            //arrange
            var service = new XmlToObjectService();

            //act
            var result = service.GetXmlFromUrl(null);
            //assert
            
            //cleanup
        }
        [TestMethod]
        [ExpectedException(typeof(Exception), "Unable to download xml from ")]
        public void GetXmlFromUrl_when_url_isempty_throw_exception_with_message_unable_to_download_from_url()
        {
            //arrange
            var service = new XmlToObjectService();

            //act
            var result = service.GetXmlFromUrl("");
            //assert

            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid url at dummyurl")]
        public void GetXmlFromUrl_when_url_not_valid_url_throw_exception_with_message_invalid_url_at_url()
        {
            //arrange
            var service = new XmlToObjectService();

            //act
            var result = service.GetXmlFromUrl("dummyurl");
            //assert

            //cleanup
        }

        [TestMethod]
        public void ConvertXml_will_return_object_afterdeserializeXml()
        {
            //arrange
            var xml = "<PurchaseOrder><Address><FirstName>George</FirstName></Address></PurchaseOrder>";
            var obj = new PurchaseOrder();
            var service = new XmlToObjectService();
            //act
            var result = service.ConvertXml<PurchaseOrder>(xml);
            //assert
            Assert.AreEqual(typeof(PurchaseOrder), result.GetType());
            //cleanup
        }
      
    }
}
