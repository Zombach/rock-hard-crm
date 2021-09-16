﻿using CRM.DAL.Enums;

namespace CRM.Business.IdentityInfo
{
    public class LeadIdentityInfo
    {
        public int LeadId { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
    }
}