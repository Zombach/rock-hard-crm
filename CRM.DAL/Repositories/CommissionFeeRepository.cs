using CRM.Core;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<int> AddCommissionFeeAsync(CommissionFeeDto dto)
        {
            return await _connection.QuerySingleOrDefaultAsync<int>(
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

        public async Task<List<CommissionFeeDto>> SearchingCommissionFeesForThePeriodAsync(TimeBasedAcquisitionDto dto)
        {
            return (await _connection
                .QueryAsync<CommissionFeeDto>(
                    _getCommissionFeesByPeriodProcedure,
                    new
                    {
                        dto.From,
                        dto.To,
                        dto.LeadId,
                        dto.AccountId,
                        dto.Role
                    },
                    commandType: CommandType.StoredProcedure))
                .ToList();
        }

        public async Task<List<CommissionFeeDto>> GetAllCommissionFeesAsync()
        {
            return (await _connection
                .QueryAsync<CommissionFeeDto>(
                    _getAllCommissionFeesProcedure,
                    commandType: CommandType.StoredProcedure))
                .ToList();
        }

        public async Task<List<CommissionFeeDto>> GetCommissionFeesByAccountIdAsync(int accountId)
        {
            return (await _connection
                .QueryAsync<CommissionFeeDto>(
                    _getCommissionFeesByAccountIdProcedure,
                    new { accountId },
                    commandType: CommandType.StoredProcedure))
                .ToList();
        }

        public async Task<List<CommissionFeeDto>> GetCommissionFeesByLeadIdAsync(int leadId)
        {
            return (await _connection
                .QueryAsync<CommissionFeeDto>(
                    _getCommissionFeesByLeadIdProcedure,
                    new { leadId },
                    commandType: CommandType.StoredProcedure))
                .ToList();
        }

        public async Task<List<CommissionFeeDto>> GetCommissionFeesByRoleAsync(int requiredRole)
        {
            return (await _connection
                .QueryAsync<CommissionFeeDto>(
                    _getCommissionFeesByRoleIdProcedure,
                    new { role = requiredRole },
                    commandType: CommandType.StoredProcedure))
                .ToList();
        }
    }
}