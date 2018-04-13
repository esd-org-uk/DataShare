using System;
using System.Collections.Generic;
using System.Linq;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class EsdFunctionServiceTests
    {
        private IRepository<SchemaESDFunctionServiceLink> _repository;
        private IGetFunctionServiceXmlContent _functionServiceXmlContent;

        [TestInitialize]
        public void TestInit()
        {
            _repository = new MemoryRepository<SchemaESDFunctionServiceLink>();
            
            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();

              }

           );
 
        }


        [TestMethod]
        public void GetFunctions_returns_type_of_list_EsdFunctions()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>");
            _functionServiceXmlContent = mock.Object;
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "<xml>not empty</xml>";
            //act
            var result = sut.GetFunctions();
            //assert
            Assert.AreEqual(typeof(List<EsdFunction>), result.GetType());

            //cleanup
            _functionServiceXmlContent = null;
        }


        [TestMethod]
        public void GetServices_returns_type_of_list_EsdService()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>");
            _functionServiceXmlContent = mock.Object;
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "<xml>not empty</xml>";
            //act
            var result = sut.GetServices();
            //assert
            Assert.AreEqual(typeof(List<EsdService>), result.GetType());

            //cleanup
            _functionServiceXmlContent = null;
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Xml content is not in valid format")]
        public void ProcessXmlContent_when_xmlcontent_is_null_throws_exception_esdfunctionlistnotloaded()
        {
            //arrange
            string nullStr = null;
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns(nullStr);
            _functionServiceXmlContent = mock.Object;
             
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = null;
            //act
            //sut.ProcessXmlContent();
            //assert
            

            //cleanup
            _functionServiceXmlContent = null;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Xml content is not in valid format")]
        public void ProcessXmlContent_when_xmlcontent_is_whitespace_throws_exception_esdfunctionlistnotloaded()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("                                          ");
            _functionServiceXmlContent = mock.Object;
             
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "                                          ";
            //act
            //sut.ProcessXmlContent();
            //assert


            //cleanup
            _functionServiceXmlContent = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Xml content is not in valid format")]
        public void ProcessXmlContent_when_xmlcontent_is_empty_throws_exception_esdfunctionlistnotloaded()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns(string.Empty);
            _functionServiceXmlContent = mock.Object;
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = string.Empty;
            //act
            //sut.ProcessXmlContent();
            //assert


            //cleanup
            _functionServiceXmlContent = null;
        }


        [TestMethod]
        [ExpectedException(typeof(Exception), "Unable to find functions/function in xml")]
        public void ProcessXmlContent_when_xmlnodes_is_xpathfunctions_returns_zero_count_nodes_throws_exception_unabletofindfunctionsinxml()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<xml>blank xml</xml>");
            _functionServiceXmlContent = mock.Object;
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "<xml>blank xml</xml>";
            //act
            //sut.ProcessXmlContent();
            //assert


            //cleanup
        }

        [TestMethod]
        public void SaveLinkedFunctionServices_will_delete_all_previous_SchemaESDFunctionServiceLink_with_given_schema_id()
        {
            //arrange
            var schema1 = new SchemaESDFunctionServiceLink() {SchemaId = 1};
            _repository.Add(schema1);

            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>");
            _functionServiceXmlContent = mock.Object;
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "<xml>blank xml</xml>";
            //act
            sut.SaveLinkedFunctionServices(1, new List<string>());
            var repoResult = _repository.GetQuery().Where(x => x.SchemaId == 1).ToList();
            //assert
            Assert.AreEqual(0, repoResult.Count);
            //cleanup
            _repository.Delete(schema1);
            _functionServiceXmlContent = null;
        }

        [TestMethod]
        public void SaveLinkedFunctionServices_when_list_of_esdIds_is_not_empty_will_add_each_new_schemaesdfunctionservicelink_to_repository()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>");
            _functionServiceXmlContent = mock.Object;
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //act
            sut.SaveLinkedFunctionServices(1, new List<string>(){"1","2","3"});
            var repoResult = _repository.GetQuery().Where(x => x.SchemaId == 1).ToList();
            //assert
            Assert.AreEqual(3, repoResult.Count);
            //cleanup
        }

        [TestMethod]
        public void
            GetLinkedFunctionsServices_when_there_are_no_linked_esdfunctionservices_returns_empty_list_of_esdfunctionserviceentity
            ()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>");
            _functionServiceXmlContent = mock.Object;

            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //act
            var result = sut.GetLinkedFunctionsServices(1);
            //assert
            Assert.AreEqual(0, result.Count);
            //cleanup

        }
        [TestMethod]
        public void
            GetLinkedFunctionsServices_when_there_are_linked_esdfunction_returns_linked_list_of_esdfunctionserviceentity
            ()
        {
            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>");
            _functionServiceXmlContent = mock.Object;
            var linked = new SchemaESDFunctionServiceLink() { SchemaId = 1, EsdFunctionServiceId = "Function1" };
            _repository.Add(linked);
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type></Function></Functions>";
            //sut.ProcessXmlContent();
            //act
            var result = sut.GetLinkedFunctionsServices(1);
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repository.Delete(linked);
        }


        [TestMethod]
        public void
            GetLinkedFunctionsServices_when_there_are_linked_esdService_returns_linked_list_of_esdfunctionserviceentity
            ()
        {

            //arrange
            var mock = new Mock<IGetFunctionServiceXmlContent>();
            mock.Setup(x => x.GetFunctionServiceXmlContent()).Returns("<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type>" +
                            "<Services><Service><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Service</Type></Service></Services>" +
                             "</Function></Functions>");
            _functionServiceXmlContent = mock.Object;
            var linked = new SchemaESDFunctionServiceLink() { SchemaId = 1, EsdFunctionServiceId = "Service1" };
            _repository.Add(linked);
            var sut = new EsdFunctionService(_repository, _functionServiceXmlContent);
            //sut.XmlContent = "<Functions><Function><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Function</Type>" +
            //                "<Services><Service><Identifier>1</Identifier><Label>Advice and benefits</Label><URI><![CDATA[http://id.esd.org.uk/function/1]]></URI><Description></Description><Created>2013-04-03T14:05:17.270</Created><Modified>2013-04-03T14:05:17.270</Modified><Type>Service</Type></Service></Services>" + 
            //                 "</Function></Functions>" ;
            //sut.ProcessXmlContent();
            //act
            var result = sut.GetLinkedFunctionsServices(1);
            //assert
            Assert.AreEqual(1, result.Count);
            //cleanup
            _repository.Delete(linked);
            
        }
    }
}
