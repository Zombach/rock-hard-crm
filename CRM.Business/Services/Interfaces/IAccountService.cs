using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto, LeadIdentityInfo leadInfo);
        void DeleteAccount(int id, int leadId);
        AccountBusinessModel GetAccountWithTransactions(int accountId, LeadIdentityInfo leadInfo);
    }
}