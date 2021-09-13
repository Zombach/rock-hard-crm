using CRM.DAL.Enums;
using System.Collections.Generic;

namespace CRM.Business.IdentityInfo
{
    public class LeadIdentityInfo
    {
        public int LeadId { get; set; }
        public List<Role> Roles { get; set; }
        public string UserName { get; set; }
    }
}