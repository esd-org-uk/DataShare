using DS.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class CustomFileSystemUtilitiesTests
    {
        [TestMethod]
        public void GetFunctionServicesXml_when_path_is_empty_returns_empty_string()
        {

            //arrange 
            var filepath = "";
            var mut = new CustomFileSystemUtilities();
            //act
            var result = mut.ReadAllText(filepath);
            //assert
            Assert.AreEqual("", result);
        }
    }
}
