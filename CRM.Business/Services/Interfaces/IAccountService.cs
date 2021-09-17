using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        int AddAccount(AccountDto accountDto, LeadIdentityInfo leadInfo);
        void DeleteAccount(int accountId, int leadId);
        void RestoreAccount(int accountId, int leadId);
        AccountBusinessModel GetAccountWithTransactions(int accountId, LeadIdentityInfo leadInfo);
        Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountId(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo);
        AccountBusinessModel GetLeadBalance(int leadId, LeadIdentityInfo leadInfo);
    }
}