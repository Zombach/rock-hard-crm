using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Linq;

namespace CRM.DAL.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private const string _addAccountProcedure = "dbo.Account_Insert";
        private const string _deleteAccountProcedure = "dbo.Account_Delete";
        private const string _restoreAccountProcedure = "dbo.Account_Restore";
        private const string _selectByIdAccountProcedure = "dbo.Account_SelectById";

        public AccountRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public int AddAccount(AccountDto dto)
        {
            return _connection.QuerySingleOrDefault<int>(
                _addAccountProcedure,
                new
                {
                    dto.LeadId,
                    dto.Currency
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void DeleteAccount(int id)
        {
            _connection.Execute(
                _deleteAccountProcedure,
                new { id },
                commandType: CommandType.StoredProcedure
            );
        }

        public void RestoreAccount(int id)
        {
            _connection.Execute(
                _restoreAccountProcedure,
                new { id },
                commandType: CommandType.StoredProcedure
            );
        }

        public AccountDto GetAccountById(int id)
        {
            AccountDto result = default;
            return _connection.Query<AccountDto, Currency, AccountDto>(
                _selectByIdAccountProcedure,
                (accountDto, currency) =>
                {
                    result = accountDto;
                    result.Currency = currency;
                    return result;
                },
                new { id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure)
              .FirstOrDefault();
        }
    }
}