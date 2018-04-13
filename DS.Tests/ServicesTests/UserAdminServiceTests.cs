using System;
using System.Linq;
using System.Web.Security;
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
    public class UserAdminServiceTests
    {
        private MembershipProvider _provider;
        private MembershipUser _fakeMembershipUser;
        private MemoryRepository<Group> _repositoryGroup;


        [TestInitialize]
        public void TestInit()
        {
     
            _repositoryGroup = new MemoryRepository<Group>();
            //_fakecacheprovider = new FakeCacheProvider();
            ObjectFactory.Initialize(
              x =>
              {
                  x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>();
                  x.For<IRepository<Group>>().Use(_repositoryGroup);
              }

           );

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ValidateUser_when_username_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ValidateUser(null, "password");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ValidateUser_when_username_is_emptystring_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ValidateUser("", "password");
            //assert
            //cleanup
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ValidateUser_when_password_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ValidateUser("username", null);
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ValidateUser_when_password_is_emptystring_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ValidateUser("username", "");
            //assert
            //cleanup
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void CreateUser_when_username_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.CreateUser(null, "password", "email", "role");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void CreateUser_when_username_is_empty_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.CreateUser("", "password", "email", "role");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void CreateUser_when_password_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.CreateUser("username", null, "email", "role");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void CreateUser_when_password_is_empty_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.CreateUser("username", "", "email", "role");
            //assert
            //cleanup
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void CreateUser_when_email_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.CreateUser("username", "password", null, "role");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void CreateUser_when_email_is_empty_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.CreateUser("username", "password", "", "role");
            //assert
            //cleanup
        }
 
        [TestMethod] 
        public void DeleteUser_when_an_exception_occurs_returns_false()
        {
            //arrange
            var mock = new Mock<MembershipProvider>();
            mock.Setup(x=>x.DeleteUser("username", true)).Throws(new Exception());
            _provider = mock.Object;
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.DeleteUser("username");
            //assert
            Assert.AreEqual(false, result);
            //cleanup
            _provider = null;
        }


        [TestMethod]
        public void DeleteUser_when_no_exception_occurs_returns_true()
        {
            //arrange
            var mock = new Mock<MembershipProvider>();
            mock.Setup(x => x.DeleteUser("username", true));
            _provider = mock.Object;
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.DeleteUser("username");
            //assert
            Assert.AreEqual(true, result);
            //cleanup
            _provider = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ChangePassword_when_username_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword(null, "oldpassword", "newpassword");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ChangePassword_when_username_is_emptystring_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("", "oldpassword", "newpassword");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ChangePassword_when_oldpassword_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", null, "newpassword");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ChangePassword_when_oldpassword_is_emptystring_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", "", "newpassword");
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ChangePassword_when_newpassword_is_null_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", "oldpassword", null);
            //assert
            //cleanup
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Value cannot be null or empty.")]
        public void ChangePassword_when_newpassword_is_emptystring_throw_exception()
        {

            //arrange
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", "oldpassword", "");
            //assert
            //cleanup
        }
        
        [TestMethod]
        public void ChangePassword_when_get_user_or_change_password_throws_argument_exception_returns_false()
        {
            //arrange
            var mock = new Mock<MembershipProvider>();
            mock.Setup(x => x.GetUser("username", true)).Throws(new ArgumentException());
            _provider = mock.Object;
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", "oldpassword", "newpassword");
            //assert
            Assert.AreEqual(false, result);
            //cleanup
            _provider = null;
        }
        [TestMethod]
        public void ChangePassword_when_get_user_or_change_password_throws_membershippasswordexception_exception_returns_false()
        {
            //arrange
            var mock = new Mock<MembershipProvider>();
            mock.Setup(x => x.GetUser("username", true)).Throws(new MembershipPasswordException());
            _provider = mock.Object;
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", "oldpassword", "newpassword");
            //assert
            Assert.AreEqual(false, result);
            //cleanup
            _provider = null;
        }

        [TestMethod]
        public void ChangePassword_when_no_exception_returns_change_password_status_result()
        {
            //arrange
            var mock = new Mock<MembershipProvider>();
            mock.Setup(x => x.GetUser("username", true)).Returns(new FakeMembershipUser());
            _provider = mock.Object;
            var mut = new UserAdminService(_provider);
            //act
            var result = mut.ChangePassword("username", "oldpassword", "newpassword");
            //assert
            Assert.AreEqual(true, result);
            //cleanup
            _provider = null;
        }


        [TestMethod]
        public void GetAllGroups_will_return_groups_sorted_by_title()
        {
            //arrange
            var group1 = new Group() {Title = "zTitle"};
            var group2 = new Group() {Title = "xTitle"};
            _repositoryGroup.Add(group1);
            _repositoryGroup.Add(group2);
            var mut = new UserAdminService(_provider);
            
            //act
            var result = mut.GetAllGroups();
            //assert
            Assert.AreEqual("xTitle", result[0].Title);
            //cleanup
            _repositoryGroup.Delete(group1);
            _repositoryGroup.Delete(group2);

        }

        [TestMethod]
        public void UpdateGroup_when_group_is_found_updates_group_title()
        {
           //arrange
            var originalGroup = new Group() {Id = 1, Title = "title"};
            var groupToUpdate = new Group() { Id = 1, Title = "updatedTitle" };
            _repositoryGroup.Add(originalGroup);
            var mut = new UserAdminService(_provider);
            //act
            mut.UpdateGroup(groupToUpdate);
            var result = _repositoryGroup.GetQuery().First(x => x.Id == 1);
            //assert
            Assert.AreEqual("updatedTitle", result.Title);
            //cleanup

        }

        [TestMethod]
        public void UpdateGroup_when_group_is_found_updates_group_Description()
        {
            //arrange
            var originalGroup = new Group() { Id = 1, Description = "description" };
            var groupToUpdate = new Group() { Id = 1, Description = "updateddescription" };
            _repositoryGroup.Add(originalGroup);
            var mut = new UserAdminService(_provider);
            //act
            mut.UpdateGroup(groupToUpdate);
            var result = _repositoryGroup.GetQuery().First(x => x.Id == 1);
            //assert
            Assert.AreEqual("updateddescription", result.Description);
            //cleanup
            _repositoryGroup.Delete(originalGroup);

        }
    }
}

