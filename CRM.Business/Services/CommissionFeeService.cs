using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public class CommissionFeeService : ICommissionFeeService
    {
        private readonly ICommissionFeeRepository _commissionFeeRepository;

        public CommissionFeeService(ICommissionFeeRepository commissionFeeRepository)
        {
            _commissionFeeRepository = commissionFeeRepository;
        }

        public int AddCommissionFee(CommissionFeeDto dto)
        {
            return _commissionFeeRepository.AddCommissionFee(dto);
        }

        public List<CommissionFeeDto> GetAllCommissionFees()
        {
            return _commissionFeeRepository.GetAllCommissionFees();
        }

        public List<CommissionFeeDto> GetCommissionFeesByAccountId(int accountId)
        {
            return _commissionFeeRepository.GetCommissionFeesByAccountId(accountId);
        }

        public List<CommissionFeeDto> GetCommissionFeesByLeadId(int leadId)
        {
            return _commissionFeeRepository.GetCommissionFeesByLeadId(leadId);
        }

        public List<CommissionFeeDto> GetCommissionFeesByPeriod(TimeBasedAcquisitionDto dto)
        {
            return _commissionFeeRepository.GetCommissionFeesByPeriod(dto);
        }

        public List<CommissionFeeDto> GetCommissionFeesByRole(Role role)
        {
            return _commissionFeeRepository.GetCommissionFeesByRole((int)role);
        }
    }
}