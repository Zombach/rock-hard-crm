using CRM.DAL.Enums;

namespace CRM.Business.IdentityInfo
{
    public static class LeadIdentityInfoExtensions
    {
        public static bool IsAdmin(this LeadIdentityInfo leadInfo)
        {
            return leadInfo.Roles.Contains(Role.Admin);
        }

        public static bool IsRegular(this LeadIdentityInfo leadInfo)
        {
            return leadInfo.Roles.Contains(Role.Regular);
        }

        public static bool IsVip(this LeadIdentityInfo leadInfo)
        {
            return leadInfo.Roles.Contains(Role.Vip);
        }
        
        public static string GetName(this LeadIdentityInfo leadInfo)
        {
            leadInfo.UserName = "Тимур добавь получение мыла юзера";
            return leadInfo.UserName;
        }
    }
}