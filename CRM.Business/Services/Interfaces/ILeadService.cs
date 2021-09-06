using CRM.DAL.Models;
using System.Collections.Generic;
using CRM.Business.IdentityInfo;

namespace CRM.Business.Services
{
    public interface ILeadService
    {
        LeadDto AddLead(LeadDto dto);
        void DeleteLeadById(int leadId);
        List<LeadDto> GetAllLeads();
        LeadDto GetLeadById(int leadId, LeadIdentityInfo leadInfo);
        LeadDto UpdateLead(int leadId, LeadDto dto);
    }
}