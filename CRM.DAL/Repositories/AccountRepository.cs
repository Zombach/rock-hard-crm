using CRM.DAL.Models;
using Dapper;
using System.Data;

namespace CRM.DAL.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private const string _addAccountProcedure = "dbo.Account_Insert";
        private const string _deleteAccountProcedure = "dbo.Account_Delete";
        public AccountRepository() { }

        public int AddAccount(AccountDto dto)
        {
            return _connection.QuerySingle<int>(
                _addAccountProcedure,
                new
                {
                    dto.LeadId,
                    dto.Currency,
                    dto.CreatedOn,
                    dto.Closed
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
    }
}