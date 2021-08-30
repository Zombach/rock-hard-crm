using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.DAL.Repositories
{
    public interface ILeadRepository
    {
        int DeleteLeadById(int id);
        List<LeadDto> GetAllLeads();
        LeadDto GetLeadByEmail(string email);
        LeadDto GetLeadById(int id);
        void UpdateLead(LeadDto dto);
    }
}