using CRM.DAL.Enums;
using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public interface ICommissionFeeService
    {
        int AddCommissionFee(CommissionFeeDto dto);
        List<CommissionFeeDto> GetAllCommissionFees();
        List<CommissionFeeDto> GetCommissionFeesByAccountId(int accountId);
        List<CommissionFeeDto> GetCommissionFeesByLeadId(int leadId);
        List<CommissionFeeDto> SearchingCommissionFeesForThePeriod(TimeBasedAcquisitionDto dto);
        List<CommissionFeeDto> GetCommissionFeesByRole(Role role);
    }
}