using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using StructureMap;

namespace DS.Service
{
    #region Services

    public class UserAdminService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public static IRepository<Group> Repository
        {
            get
            {
                return ObjectFactory.GetInstance<IRepository<Group>>();
            }
        }

        public UserAdminService()
            : this(null)
        {
        }
         
        public UserAdminService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email, string userRole)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            MembershipCreateStatus status;
            var user = _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            if (status == MembershipCreateStatus.Success)
            {
                Roles.AddUserToRole(user.UserName, userRole);
                //TODO: Add user to groups
            }
            return status;
        }

        public bool DeleteUser(string userName)
        {
            try
            {
                _provider.DeleteUser(userName, true);
                //TODO: Remove user from groups
                return true;
            }
            catch(Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return false;
            }
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return false;
            }
            catch (MembershipPasswordException ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                return false;
            }
        }

        public MembershipUser Get(string userName)
        {
            return _provider.GetUser(userName, false);
        }

        public MembershipUser Get(Guid membershipUserId)
        {
            return _provider.GetUser(membershipUserId, false);
        }

        public EditUserModel GetUserForEdit(string userName)
        {
            var membershipUser = _provider.GetUser(userName, false);
            return new EditUserModel
                       {
                           UserName = userName,
                           Email = membershipUser.Email,
                           Role = Roles.GetRolesForUser(userName)[0]
                           };
        }

        public void UpdateUser(EditUserModel user)
        {
            var originalUser = _provider.GetUser(user.UserName, false);
            if(!string.IsNullOrEmpty(user.Password))
            {
                var tempPassword = originalUser.ResetPassword();
                originalUser.ChangePassword(tempPassword, user.Password);
            }
            if (originalUser.Email != user.Email)
            {
                originalUser.Email = user.Email;
                _provider.UpdateUser(originalUser);
            }
            var currentRole = Roles.GetRolesForUser(originalUser.UserName)[0];
            if (currentRole != user.Role)
            {
                Roles.RemoveUserFromRole(user.UserName,currentRole);
                Roles.AddUserToRole(user.UserName,user.Role);
            }
            //TODO: Amend user's group membership
        }

        public MembershipUserCollection GetUsers()
        {
            return Membership.GetAllUsers();
        }

        public IList<Group> GetAllGroups()
        {
            return Repository.GetQuery()
                            .OrderBy(g => g.Title).ToList();
        }

        public Group GetGroup(int id)
        {
            return Repository.GetQuery().FirstOrDefault(g => g.Id == id);
        }

        public void CreateGroup(Group group)
        {
            Repository.Add(group);
            Repository.SaveChanges();
        }

        public void DeleteGroup(Group group)
        {
            Repository.Delete(group);
            Repository.SaveChanges();
        }

        public void UpdateGroup(Group group)
        {
            var originalGroup = Repository.GetQuery().First(g => g.Id == @group.Id);
            originalGroup.Title = group.Title;
            originalGroup.Description = group.Description;
            Repository.SaveChanges();
        }

    }

    #endregion



}
