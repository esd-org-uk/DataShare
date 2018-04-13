using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace DS.Tests.Fakes
{
    public class FakeMembershipUser : MembershipUser
    {
        public FakeMembershipUser()
        {

        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            return oldPassword != newPassword;
        }

    }
}
