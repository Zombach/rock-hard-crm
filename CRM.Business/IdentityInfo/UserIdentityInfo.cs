using System.Collections.Generic;
using CRM.DAL.Enums;

namespace CRM.Business.IdentityInfo
{
    public class UserIdentityInfo
    {
        public int LeadId { get; set; }
        public List<Role> Roles { get; set; }
    }
}