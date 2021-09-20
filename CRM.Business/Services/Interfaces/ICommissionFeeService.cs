using CRM.DAL.Enums;
using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface ICommissionFeeService
    {
        Task<int> AddCommissionFeeAsync(CommissionFeeDto dto);
        Task<List<CommissionFeeDto>> GetAllCommissionFeesAsync();
        Task<List<CommissionFeeDto>> GetCommissionFeesByAccountIdAsync(int accountId);
        Task<List<CommissionFeeDto>> GetCommissionFeesByLeadIdAsync(int leadId);
        Task<List<CommissionFeeDto>> SearchingCommissionFeesForThePeriodAsync(TimeBasedAcquisitionDto dto);
        Task<List<CommissionFeeDto>> GetCommissionFeesByRoleAsync(Role role);
    }
}