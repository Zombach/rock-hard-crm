using CRM.DAL.Enums;

namespace CRM.Business.IdentityInfo
{
    public static class UserIdentityInfoExtensions
    {
        public static bool IsAdmin(this UserIdentityInfo userInfo)
        {
            return userInfo.Roles.Contains(Role.Admin);
        }

        public static bool IsRegular(this UserIdentityInfo userInfo)
        {
            return userInfo.Roles.Contains(Role.Regular);
        }

        public static bool IsVip(this UserIdentityInfo userInfo)
        {
            return userInfo.Roles.Contains(Role.Vip);
        }
    }
}