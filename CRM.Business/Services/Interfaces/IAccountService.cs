using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto, UserIdentityInfo userIdentityInfo);
        void DeleteAccount(int id, UserIdentityInfo userIdentityInfo);
        AccountBusinessModel GetAccountWithTransactions(int id, UserIdentityInfo userIdentityInfo);
    }
}