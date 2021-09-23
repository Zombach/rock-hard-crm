using CRM.DAL.Models;
using SqlKata;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.DAL.Repositories
{
    public interface ILeadRepository
    {
        Task<int> AddLeadAsync(LeadDto lead);
        Task UpdateLeadAsync(LeadDto lead);
        Task UpdateLeadRoleAsync(LeadDto lead);
        Task<int> DeleteLeadAsync(int id);
        Task<LeadDto> GetLeadByIdAsync(int id);
        Task<LeadDto> GetLeadByEmailAsync(string email);
        Task<List<LeadDto>> GetAllLeadsAsync();
        Task UpdateLeadRoleBulkAsync(List<LeadDto> listLeadDtos);
        List<LeadDto> GetLeadsByFilters(SqlResult sqlResult);
        List<LeadDto> GetAllLeadsByBatches(int cursorId);
    }
}