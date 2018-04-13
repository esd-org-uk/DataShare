using System;
using System.Web.Security;

namespace DS.Domain.Interface
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email, string userRole);
        bool DeleteUser(string userName);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        MembershipUser Get(string userName);
        MembershipUser Get(Guid membershipUserId);
        EditUserModel GetUserForEdit(string userName);
    }
}