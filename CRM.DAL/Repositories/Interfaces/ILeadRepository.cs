using CRM.DAL.Models;
using SqlKata;
using System.Collections.Generic;

namespace CRM.DAL.Repositories
{
    public interface ILeadRepository
    {
        int AddLead(LeadDto lead);
        void UpdateLead(LeadDto lead);
        void UpdateLeadRole(LeadDto lead);
        int DeleteLead(int id);
        LeadDto GetLeadById(int id);
        LeadDto GetLeadByEmail(string email);
        List<LeadDto> GetAllLeads();
        List<LeadDto> GetLeadsByFilters(SqlResult sqlResult);
    }
}