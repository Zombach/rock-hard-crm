using CRM.DAL.Enums;

namespace CRM.Business.IdentityInfo
{
    public static class LeadIdentityInfoExtensions
    {
        public static bool IsAdmin(this LeadIdentityInfo leadInfo)
        {
            return leadInfo.Roles.Contains(Role.Admin);
        }

        public static bool IsManager(this LeadIdentityInfo leadInfo)
        {
            return leadInfo.Roles.Contains(Role.Regular);
        }

        public static bool IsMethodist(this LeadIdentityInfo leadInfo)
        {
            return leadInfo.Roles.Contains(Role.Vip);
        }
    }
}