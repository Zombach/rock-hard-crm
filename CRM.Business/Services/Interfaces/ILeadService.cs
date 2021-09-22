using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface ILeadService
    {
        Task<LeadDto> AddLeadAsync(LeadDto dto);
        Task<LeadDto> UpdateLeadAsync(int leadId, LeadDto dto);
        Task<LeadDto> UpdateLeadRoleAsync(int leadId, Role role);
        Task DeleteLeadAsync(int leadId);
        Task<LeadDto> GetLeadByIdAsync(int leadId, LeadIdentityInfo leadInfo);
        Task<List<LeadDto>> GetAllLeadsAsync();
        Task<int> AddTwoFactorKeyToLeadAsync(int leadId, string key);
        Task<string> GetTwoFactorKeyAsync(int leadId);
    }
}