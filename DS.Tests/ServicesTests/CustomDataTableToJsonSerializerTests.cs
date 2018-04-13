using System;
using System.Data;
using System.ServiceModel.Web;
using DS.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class CustomDataTableToJsonSerializerTests
    {
        [TestMethod]
        public void GetStream_when_current_weboperationcontext_is_not_null_returns_outgoingresponse_as_json()
        {
            //arrange
            var dt = new DataTable("TableName");
            var mut = new CustomDataTableToJsonSerializer();
            //act
            mut.GetStream(dt);
            var weboperationcontextType = WebOperationContext.Current.OutgoingResponse.ContentType;
            //assert
            Assert.AreEqual("text/json", weboperationcontextType);
            //cleanup
        }
    }
}
