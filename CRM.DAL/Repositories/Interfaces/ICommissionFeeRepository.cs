using System.Collections.Generic;
using CRM.DAL.Models;

namespace CRM.DAL.Repositories
{
    public interface ICommissionFeeRepository
    {
        int AddCommissionFee(CommissionFeeDto dto);
        List<CommissionFeeDto> GetAllCommissionFees();
        List<CommissionFeeDto> GetCommissionFeesByAccountId(int accountId);
        List<CommissionFeeDto> GetCommissionFeesByLeadId(int leadId);
        List<CommissionFeeDto> GetCommissionFeesByPeriod(TimeBasedAcquisitionDto dto);
        List<CommissionFeeDto> GetCommissionFeesByRole(int requiredRole);
    }
}