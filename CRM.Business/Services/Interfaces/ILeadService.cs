using CRM.DAL.Models;
using System.Collections.Generic;
using CRM.Business.IdentityInfo;

namespace CRM.Business.Services
{
    public interface ILeadService
    {
        LeadDto AddLead(LeadDto dto);
        void DeleteLeadById(int id);
        List<LeadDto> GetAllLeads();
        LeadDto GetLeadById(int id);
        LeadDto UpdateLead(int id, LeadDto dto, UserIdentityInfo userIdentityInfo);
        int AddAccount(AccountDto dto);
        void DeleteAccount(int id);
        List<LeadDto> GetLeadsByFilters(LeadFiltersDto filter);
    }
}