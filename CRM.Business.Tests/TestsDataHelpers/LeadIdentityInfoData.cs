using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class LeadIdentityInfoData
    {
        public static LeadIdentityInfo GetRegularLeadIdentityInfo()
        {
            return new ()
            {
                LeadId = 1,
                Role = Role.Regular
            };
        }

        public static LeadIdentityInfo GetRegularAnotherLeadIdentityInfo()
        {
            return new ()
            {
                LeadId = 12,
                Role = Role.Regular
            };
        }

        public static LeadIdentityInfo GetVipLeadIdentityInfo()
        {
            return new ()
            {
                LeadId = 2,
                Role = Role.Vip
            };
        }

        public static LeadIdentityInfo GetAdminLeadIdentityInfo()
        {
            return new ()
            {
                LeadId = 3,
                Role = Role.Admin
            };
        }
    }
}
