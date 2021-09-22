using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.DAL.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private const string _addAccountProcedure = "dbo.Account_Insert";
        private const string _deleteAccountProcedure = "dbo.Account_Delete";
        private const string _restoreAccountProcedure = "dbo.Account_Restore";
        private const string _selectByIdAccountProcedure = "dbo.Account_SelectById";
        private const string _selectAllByListIdAccountProcedure = "dbo.Account_SelectByListId";
        private const string _accountIdType = "dbo.AccountIdType";

        public AccountRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public async Task<int> AddAccountAsync(AccountDto dto)
        {
            return await _connection.QueryFirstOrDefaultAsync<int>(
                _addAccountProcedure,
                new
                {
                    dto.LeadId,
                    dto.Currency
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task DeleteAccountAsync(int id)
        {
            await _connection.ExecuteAsync(
                _deleteAccountProcedure,
                new { id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task RestoreAccountAsync(int id)
        {
            await _connection.ExecuteAsync(
                _restoreAccountProcedure,
                new { id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<AccountDto> GetAccountByIdAsync(int id)
        {
            AccountDto result;
            return (await _connection.QueryAsync<AccountDto, Currency, AccountDto>(
                _selectByIdAccountProcedure,
                (accountDto, currency) =>
                {
                    result = accountDto;
                    result.Currency = currency;
                    return result;
                },
                new { id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure))
              .FirstOrDefault();
        }

        public async Task<List<AccountDto>> GetAccountsByListIdAsync(List<int> accountsId)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id");

            foreach (var id in accountsId)
            {
                dt.Rows.Add(id);
            }

            return (await _connection.QueryAsync<AccountDto, Currency, AccountDto>(
                _selectAllByListIdAccountProcedure,
                (accountDto, currency) =>
                {
                    accountDto.Currency = currency;
                    return accountDto;
                },
                new { tblIds = dt.AsTableValuedParameter(_accountIdType) },
                commandType: CommandType.StoredProcedure))
                .ToList();
        }
    }
}