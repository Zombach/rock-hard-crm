using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        Task<int> AddAccountAsync(AccountDto accountDto, LeadIdentityInfo leadInfo);
        Task DeleteAccountAsync(int accountId, int leadId);
        Task RestoreAccountAsync(int accountId, int leadId);
        Task<AccountBusinessModel> GetAccountWithTransactionsAsync(int accountId, LeadIdentityInfo leadInfo);
        Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo);
        Task<AccountBusinessModel> GetLeadBalanceAsync(int leadId, LeadIdentityInfo leadInfo);
        List<TransactionBusinessModel> GetTransactionsByAccountIdsForTwoMonths(List<int> accountIds, LeadIdentityInfo leadInfo);
    }
}