using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public static Role GetLeadRoles(this Controller controller)
        {
            return (Role)Enum.Parse(typeof(Role), controller.User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        public static LeadIdentityInfo GetLeadInfo(this Controller controller)
        {
            var leadId = Convert.ToInt32(controller.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var role = (Role)Enum.Parse(typeof(Role), controller.User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            return new LeadIdentityInfo
            {
                LeadId = leadId,
                Role = role
            };
        }
    }
}