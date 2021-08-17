using CRM.DAL.Models;
using Dapper;
using System.Data;

namespace CRM.DAL.Repositories
{
    public class AccountRepository : BaseRepository
    {
        private const string _addAccountProcedure = "";
        private const string _deleteAccountProcedure = "";
        public AccountRepository() { }

        public int AddAccount (AccountDto dto)
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
    }
}
