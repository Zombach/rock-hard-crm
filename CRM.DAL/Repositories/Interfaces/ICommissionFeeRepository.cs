using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.DAL.Repositories
{
    public interface ICommissionFeeRepository
    {
        Task<int> AddCommissionFeeAsync(CommissionFeeDto dto);
        Task<List<CommissionFeeDto>> GetAllCommissionFeesAsync();
        Task<List<CommissionFeeDto>> GetCommissionFeesByAccountIdAsync(int accountId);
        Task<List<CommissionFeeDto>> GetCommissionFeesByLeadIdAsync(int leadId);
        Task<List<CommissionFeeDto>> SearchingCommissionFeesForThePeriodAsync(TimeBasedAcquisitionDto dto);
        Task<List<CommissionFeeDto>> GetCommissionFeesByRoleAsync(int requiredRole);
    }
}