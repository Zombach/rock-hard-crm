using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRM.DAL.Repositories
{
    public class CommissionFeeRepository : BaseRepository, ICommissionFeeRepository
    {
        private const string _insertCommissionFeeProcedure = "dbo.CommissionFee_Insert";
        private const string _getAllCommissionFeesProcedure = "dbo.CommissionFee_SelectAll";
        private const string _getCommissionFeesByAccountIdProcedure = "dbo.CommissionFee_SelectByAccountId";
        private const string _getCommissionFeesByLeadIdProcedure = "dbo.CommissionFee_SelectByLeadId";
        private const string _getCommissionFeesByPeriodProcedure = "dbo.CommissionFee_SelectByPeriod";
        private const string _getCommissionFeesByRoleIdProcedure = "dbo.CommissionFee_SelectByRole";

        public CommissionFeeRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public int AddCommissionFee(CommissionFeeDto dto)
        {
            return _connection.QuerySingleOrDefault<int>(
                _insertCommissionFeeProcedure,
                new
                {
                    dto.LeadId,
                    dto.AccountId,
                    dto.TransactionId,
                    dto.Role,
                    dto.Amount
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<CommissionFeeDto> GetAllCommissionFees()
        {
            return _connection
                .Query<CommissionFeeDto, Role, CommissionFeeDto>(
                    _getAllCommissionFeesProcedure,
                    (commissionFee, role) =>
                    {
                        commissionFee.Role = role;
                        return commissionFee;
                    },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByAccountId(int accountId)
        {
            return _connection
                .Query<CommissionFeeDto, Role, CommissionFeeDto>(
                    _getCommissionFeesByAccountIdProcedure,
                    (commissionFee, role) =>
                    {
                        commissionFee.Role = role;
                        return commissionFee;
                    },
                    new { accountId },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByLeadId(int leadId)
        {
            return _connection
                .Query<CommissionFeeDto, Role, CommissionFeeDto>(
                    _getCommissionFeesByLeadIdProcedure,
                    (commissionFee, role) =>
                    {
                        commissionFee.Role = role;
                        return commissionFee;
                    },
                    new { leadId },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByPeriod(TimeBasedAcquisitionDto dto)
        {
            return _connection
                .Query<CommissionFeeDto, Role, CommissionFeeDto>(
                    _getCommissionFeesByPeriodProcedure,
                    (commissionFee, role) =>
                    {
                        commissionFee.Role = role;
                        return commissionFee;
                    },
                    new
                    {
                        dto.From,
                        dto.To
                    },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByRole(int requiredRole)
        {
            return _connection
                .Query<CommissionFeeDto, Role, CommissionFeeDto>(
                    _getCommissionFeesByRoleIdProcedure,
                    (commissionFee, role) =>
                    {
                        commissionFee.Role = role;
                        return commissionFee;
                    },
                    new { role = requiredRole },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }
    }
}