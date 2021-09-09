using CRM.DAL.Models;

namespace CRM.DAL.Repositories
{
    public interface IAccountRepository
    {
        int AddAccount(AccountDto dto);
        void DeleteAccount(int id);
        void RestoreAccount(int id);
        AccountDto GetAccountById(int id);
    }
}