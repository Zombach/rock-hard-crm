﻿using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public interface ILeadService
    {
        LeadDto AddLead(LeadDto dto);
        void DeleteLeadById(int id);
        List<LeadDto> GetAllLeads();
        LeadDto GetLeadById(int id);
        LeadDto GetLeadById(string email);
        LeadDto UpdateLead(int id, LeadDto dto);
    }
}