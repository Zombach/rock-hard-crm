using CRM.Core;
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
        private const string _getCommissionFeesByPeriodProcedure = "dbo.CommissionFee_SeatchingByPeriod";
        private const string _getAllCommissionFeesProcedure = "dbo.CommissionFee_SelectAll";
        private const string _getCommissionFeesByAccountIdProcedure = "dbo.CommissionFee_SelectByAccountId";
        private const string _getCommissionFeesByLeadIdProcedure = "dbo.CommissionFee_SelectByLeadId";
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
                    Amount = dto.CommissionAmount
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<CommissionFeeDto> SearchingCommissionFeesForThePeriod(TimeBasedAcquisitionDto dto)
        {
            return _connection
                .Query<CommissionFeeDto>(
                    _getCommissionFeesByPeriodProcedure,
                    new
                    {
                        dto.From,
                        dto.To,
                        dto.LeadId,
                        dto.AccountId,
                        dto.Role
                    },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetAllCommissionFees()
        {
            return _connection
                .Query<CommissionFeeDto>(
                    _getAllCommissionFeesProcedure,
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByAccountId(int accountId)
        {
            return _connection
                .Query<CommissionFeeDto>(
                    _getCommissionFeesByAccountIdProcedure,
                    new { accountId },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByLeadId(int leadId)
        {
            return _connection
                .Query<CommissionFeeDto>(
                    _getCommissionFeesByLeadIdProcedure,
                    new { leadId },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<CommissionFeeDto> GetCommissionFeesByRole(int requiredRole)
        {
            return _connection
                .Query<CommissionFeeDto>(
                    _getCommissionFeesByRoleIdProcedure,
                    new { role = requiredRole },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }
    }
}