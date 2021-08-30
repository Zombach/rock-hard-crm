using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public interface ILeadService
    {
        LeadDto AddLead(LeadDto dto);
        void DeleteLeadById(int id);
        List<LeadDto> GetAllLeads();
        LeadDto GetLeadById(int id);
        LeadDto UpdateLead(int id, LeadDto dto);
        int AddAccount(AccountDto dto);
        void DeleteAccount(int id);
    }
}