using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CRM.API.Extensions
{
    public static class BaseControllerExtensions
    {
        public static int GetLeadId(this ControllerBase controller)
        {
            return Convert.ToInt32(controller.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public static List<Role> GetLeadRoles(this Controller controller)
        {
            return controller.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => (Role)Enum.Parse(typeof(Role), c.Value)).ToList();
        }

        public static LeadIdentityInfo GetLeadIdAndRoles(this Controller controller)
        {
            var leadId = Convert.ToInt32(controller.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var roles = controller.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => (Role)Enum.Parse(typeof(Role), c.Value)).ToList();
            return new LeadIdentityInfo
            {
                LeadId = leadId,
                Roles = roles
            };
        }
    }
}