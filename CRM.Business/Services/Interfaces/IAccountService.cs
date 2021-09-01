using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto, int leadId);
        void DeleteAccount(int id, int leadId);
        AccountBusinessModel GetAccountWithTransactions(int id, int leadId);
    }
}