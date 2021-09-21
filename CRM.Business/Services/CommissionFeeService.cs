using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public class CommissionFeeService : ICommissionFeeService
    {
        private readonly ICommissionFeeRepository _commissionFeeRepository;

        public CommissionFeeService(ICommissionFeeRepository commissionFeeRepository)
        {
            _commissionFeeRepository = commissionFeeRepository;
        }

        public async Task<int> AddCommissionFeeAsync(CommissionFeeDto dto)
        {
            return await _commissionFeeRepository.AddCommissionFeeAsync(dto);
        }

        public async Task<List<CommissionFeeDto>> GetAllCommissionFeesAsync()
        {
            return await _commissionFeeRepository.GetAllCommissionFeesAsync();
        }

        public async Task<List<CommissionFeeDto>> GetCommissionFeesByAccountIdAsync(int accountId)
        {
            return await _commissionFeeRepository.GetCommissionFeesByAccountIdAsync(accountId);
        }

        public async Task<List<CommissionFeeDto>> GetCommissionFeesByLeadIdAsync(int leadId)
        {
            return await _commissionFeeRepository.GetCommissionFeesByLeadIdAsync(leadId);
        }

        public async Task<List<CommissionFeeDto>> SearchingCommissionFeesForThePeriodAsync(TimeBasedAcquisitionDto dto)
        {
            return await _commissionFeeRepository.SearchingCommissionFeesForThePeriodAsync(dto);
        }

        public async Task<List<CommissionFeeDto>> GetCommissionFeesByRoleAsync(Role role)
        {
            return await _commissionFeeRepository.GetCommissionFeesByRoleAsync((int)role);
        }
    }
}