using System.Collections.Generic;
using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class UserIdentityInfoData
    {
        public static UserIdentityInfo GetUserIdentityInfo(int id, List<Role> roles)
        {
            return new()
            {
                LeadId = id,
                Roles = roles
            };
        }
    }
}
