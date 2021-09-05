﻿using CRM.DAL.Models;
using SqlKata;
using System.Collections.Generic;

namespace CRM.DAL.Repositories
{
    public interface ILeadRepository
    {
        int AddLead(LeadDto lead);
        int DeleteLeadById(int id);
        List<LeadDto> GetAllLeads();
        LeadDto GetLeadByEmail(string email);
        LeadDto GetLeadById(int id);
        void UpdateLead(LeadDto lead);
        List<LeadDto> GetLeadsByFilters(SqlResult sqlResult);
    }
}