using CRM.DAL.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Common
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params Role[] roles) : base()
        {
            Roles = string.Join(",", roles);
            Roles += $"{Role.Admin}";
        }
    }
}
