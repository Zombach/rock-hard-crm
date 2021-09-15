using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public interface ILeadService
    {
        LeadDto AddLead(LeadDto dto);
        LeadDto UpdateLead(int leadId, LeadDto dto);
        LeadDto UpdateLeadRole(int leadId, Role role);
        void DeleteLead(int leadId);
        LeadDto GetLeadById(int leadId, LeadIdentityInfo leadInfo);
        List<LeadDto> GetAllLeads();
        void ChangeRoleForLeads(List<LeadDto> listLeadDtos);
    }
}