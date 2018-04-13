using System;
using DS.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class FormsAuthenticationServiceTests
    {
        [TestInitialize]
        public void TestInit()
        {
            
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void Signin_when_name_is_null_throws_new_argumentexception_value_cannot_be_null_or_empty()
        {
            //arrange
            var mut = new FormsAuthenticationService();
            //act
            mut.SignIn(null, false);
            //assert
            //cleanup
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void Signin_when_name_is_empty_throws_new_argumentexception_value_cannot_be_null_or_empty()
        {
            //arrange
            var mut = new FormsAuthenticationService();
            //act
            mut.SignIn(string.Empty, false);
            //assert
            //cleanup
        }
    }
}
